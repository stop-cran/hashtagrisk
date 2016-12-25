using System;

namespace RiskApp.Calculations
{
    public class RiskEventFactory
    {
        public RiskEvent CreateExtremum(double μ, double σ)
        {
            return new RiskEvent(x =>
            {
                double d = (μ - x) / σ;

                return Math.Exp(d - Math.Exp(d)) / σ;
            });
        }

        public RiskEvent CreateExtremumGeneral(double μ, double σ, double ξ)
        {
            return new RiskEvent(x =>
            {
                double d = (x - μ) / σ;

                // -Math.Pow(1 + ξ * d, -1 / ξ)
                return Math.Exp(-Math.Pow(1 + ξ * d, -1 / ξ)) * Math.Pow(1 + ξ * d, -1 / ξ - 1) / σ;
            });
        }

        public RiskEvent CreateExtremum(double uMin, double uMax, double pConf)
        {
            double x1 = Math.Log(Math.Log(2) - Math.Log(1 - pConf)),
                x2 = Math.Log(Math.Log(2) - Math.Log(1 + pConf));

            return CreateExtremum((uMax * x1 - uMin * x2) / (x1 - x2), (uMax - uMin) / (x1 - x2));
        }
    }
}