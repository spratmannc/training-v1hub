using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace V1Hub.Api.Models
{
    public class Sprint
    {
        public string Oid { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}