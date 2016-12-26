using System;
using System.Collections.Generic;
using System.Linq;

namespace RiskApp.Calculations
{
    public class RiskEvent
    {
        const double ε = 0.1;
        const double limit = 1e-10;

        public RiskEvent(Func<double, double> ρ)
        {
            this.ρ = ρ;
        }

        Func<double, double> ρ { get; }

        public double[] ρNet { get { return ρNetHelper().ToArray(); } }

        double Weight { get { return ρNet.Sum() * ε; } }

        IEnumerable<double> ρNetHelper()
        {
            for (int i = 0; ; i++)
            {
                var d = ρ(i * ε);

                if (i > 100 && d < limit || double.IsNaN(d))
                    yield break;
                yield return d;
            }
        }

        public RiskEvent Cache()
        {
            var array = ρNet;
            return new RiskEvent(x =>
            {
                int i = (int)(x / ε);
                double d = x / ε - i;

                return i < 0 || i >= array.Length - 1
                    ? 0
                    : array[i] * (1 - d) + array[i + 1] * d;
            });
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
            return new RiskEvent(x => Enumerable.Range(0, (int)(x / ε))
                .Sum(i => ρ(i * ε) * other.ρ(x - i * ε)) * ε).Cache();
        }
    }
}