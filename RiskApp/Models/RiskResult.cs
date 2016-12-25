using Chart.Mvc.ComplexChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskApp.Models
{
    public class RiskResult
    {
        public List<ComplexDataset> Results { get; set; }
        public List<string> Mesh { get; set; }
    }
}