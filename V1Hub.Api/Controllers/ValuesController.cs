using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using V1Hub.Api.Models;
using VersionOne.SDK.APIClient;

namespace V1Hub.Api.Controllers
{
    [Route("api/values/{action}")]
    public class ValuesController : ApiController
    {

        private static IServices GetServices(string token)
        {
            V1Connector connector = V1Connector
                .WithInstanceUrl("https://www4.v1host.com/LexisNexis")
                .WithUserAgentHeader("V1Hub", "1.0")
                .WithAccessToken(token)
                .Build();

            IServices svc = new Services(connector);
            return svc;
        }

        [HttpGet]
        public IEnumerable<Sprint> Sprints(string token)
        {
            var svc = GetServices(token);
            IAssetType iterationType = svc.Meta.GetAssetType("Timebox");

            Query query = new Query(iterationType);

            // indicate which attributes I need
            IAttributeDefinition nameAttr = iterationType.GetAttributeDefinition("Name");
            IAttributeDefinition beginDateAttr = iterationType.GetAttributeDefinition("BeginDate");
            IAttributeDefinition endDateAttr = iterationType.GetAttributeDefinition("EndDate");
            IAttributeDefinition stateAttr = iterationType.GetAttributeDefinition("State.Code");
            IAttributeDefinition scheduleAttr = iterationType.GetAttributeDefinition("Schedule.Name");

            query.Selection.Add(nameAttr);
            query.Selection.Add(beginDateAttr);
            query.Selection.Add(endDateAttr);

            // filter
            FilterTerm term = new FilterTerm(stateAttr);
            term.Equal("ACTV");
            query.Filter = term;

            query.Find = new QueryFind("New Lexis Sprint", new AttributeSelection(scheduleAttr));

            // get results
            var result = svc.Retrieve(query);

            return result.Assets
                         .Select(a => new Sprint
                         {
                             Oid = a.Oid.Token,
                             Title = a.GetAttribute(nameAttr).Value.ToString(),
                             StartDate = DateTime.Parse(a.GetAttribute(beginDateAttr).Value.ToString()),
                             EndDate = DateTime.Parse(a.GetAttribute(endDateAttr).Value.ToString())
                         });

        }

        [HttpGet]
        public IEnumerable<StoryInfo> Stories(string token, string sprint)
        {
            var svc = GetServices(token);

            // build query by specifying which attributes to use
            IAssetType storyType = svc.Meta.GetAssetType("Story");
            IAttributeDefinition teamAttr = storyType.GetAttributeDefinition("Team");
            IAttributeDefinition teamNameAttr = storyType.GetAttributeDefinition("Team.Name");
            IAttributeDefinition nameAttr = storyType.GetAttributeDefinition("Name");
            IAttributeDefinition descriptionAttr = storyType.GetAttributeDefinition("Description");
            IAttributeDefinition ownerNameAttr = storyType.GetAttributeDefinition("Owners");
            IAttributeDefinition numberAttr = storyType.GetAttributeDefinition("Number");
            IAttributeDefinition timeboxAttr = storyType.GetAttributeDefinition("Timebox");
            IAttributeDefinition timeboxNameAttr = storyType.GetAttributeDefinition("Timebox.Name");
            IAttributeDefinition linkUrlAttr = storyType.GetAttributeDefinition("Links.URL");
            IAttributeDefinition linkNameAttr = storyType.GetAttributeDefinition("Links.Name");

            Query query = new Query(storyType);
            query.Selection.Add(teamAttr);
            query.Selection.Add(teamNameAttr);
            query.Selection.Add(nameAttr);
            query.Selection.Add(descriptionAttr);
            query.Selection.Add(ownerNameAttr);
            query.Selection.Add(numberAttr);
            query.Selection.Add(timeboxAttr);
            query.Selection.Add(timeboxNameAttr);
            query.Selection.Add(linkUrlAttr);
            query.Selection.Add(linkNameAttr);

            // define filters
            FilterTerm timeBoxTerm = new FilterTerm(timeboxAttr);
            timeBoxTerm.Equal(sprint);
            
            FilterTerm ownerTerm = new FilterTerm(ownerNameAttr);
            ownerTerm.Equal(svc.LoggedIn.Token);

            query.Filter = new AndFilterTerm(timeBoxTerm, ownerTerm);

            // execute
            var result = svc.Retrieve(query);

            // transform
            return result.Assets
                         .Select(a => new StoryInfo
                         {
                             Oid = a.Oid.Token,
                             Title = a.GetAttribute(nameAttr).Value as string,
                             Description = a.GetAttribute(descriptionAttr).Value as string,
                             SprintId = a.GetAttribute(timeboxAttr).Value as string,
                             SprintName = a.GetAttribute(timeboxNameAttr).Value as string,
                             StoryNumber = a.GetAttribute(numberAttr).Value as string,
                             TeamId = a.GetAttribute(teamAttr).Value as string,
                             TeamName = a.GetAttribute(teamNameAttr).Value as string,
                             LinkURLs = a.GetAttribute(linkUrlAttr).Values as IEnumerable<object>,
                             LinkNames = a.GetAttribute(linkNameAttr).Values as IEnumerable<object>
                         });

        }

    }
}
