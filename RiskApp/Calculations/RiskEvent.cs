using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskApp.Calculations
{
    public class RiskEvent
    {
        const double ε = 0.1;
        const int n = 100;

        public RiskEvent(Func<double, double> ρ)
        {
            this.ρ = ρ;
        }

        Func<double, double> ρ { get; }

        public double[] ρNet { get { return Enumerable.Range(0, n).Select(i => ρ(i * ε)).ToArray(); } }
        public double Weight { get { return Enumerable.Range(0, n).Sum(i => ρ(i * ε)) * ε; } }

        public RiskEvent Cache()
        {
            var array = ρNet;
            return new RiskEvent(x => array[(int)(x / ε)]);
        }

        public RiskEvent ApplyWeight(double weight)
        {
            return new RiskEvent(x => ρ(x) * weight / Weight);
        }

        public RiskEvent Sum(RiskEvent other)
        {
            return new RiskEvent(x => ρ(x) + other.ρ(x));
        }

        public RiskEvent Convolve(RiskEvent other)
        {
            return new RiskEvent(x => Enumerable.Range(0, (int)(x / ε)).Sum(i => ρ(i * ε) * other.ρ(x - i * ε) * ε)).Cache();
        }
    }
}