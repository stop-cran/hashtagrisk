using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskApp.Models
{
    public class RiskFactor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Frequency { get; set; }
    }
}