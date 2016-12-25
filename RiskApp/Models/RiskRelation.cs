using RiskApp.Calculations;

namespace RiskApp.Models
{
    public class RiskRelation
    {
        public RiskFactor Factor { get; set; }
        public RiskEvent Event { get; set; }
    }
}