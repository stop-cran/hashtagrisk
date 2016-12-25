using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskApp.Models
{
    public class RiskModel
    {
        public List<RiskFactor> Factors { get; set; }
        public List<RiskEvent> Events { get; set; }
        public List<RiskRelation> Relations { get; set; }
    }
}