using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V1Hub.Api.Models
{
    public class StoryInfo
    {
        public string Oid { get; set; }
        public string StoryNumber { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SprintId { get; set; }
        public string SprintName { get; set; }
        internal IEnumerable<object> LinkURLs { get; set; }
        internal IEnumerable<object> LinkNames { get; set; }

        public IEnumerable<Link> Links {
            get
            {
                List<Link> results = new List<Link>();

                if(LinkURLs != null && LinkNames != null)
                {
                    var urls = LinkURLs.ToArray();
                    var names = LinkNames.ToArray();

                    for (int i = 0; i < urls.Length; i++)
                    {
                        results.Add(new Link
                        {
                            Name = names[i].ToString(),
                            Url = urls[i].ToString()
                        });
                    }
                }

                return results.ToArray();
            }
        }
    }

    public class Link
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
