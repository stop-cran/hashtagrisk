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

        public double Probability(int count)
        {
            return Math.Exp(-Frequency) * Math.Pow(Frequency, count) / Factorial(count);
        }

        static int Factorial(int i)
        {
            return i <= 1 ? 1 : Enumerable.Range(1, i - 1).Aggregate((x, y) => x * y);
        }
    }
}