using Chart.Mvc.ComplexChart;
using RiskApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RiskApp.Calculations
{
    public class Calculator
    {
        static int Factorial(int i)
        {
            return i <= 1 ? 1 : i * Factorial(i - 1);
        }

        public Task<RiskResult> CalculateAsync(HttpPostedFileBase file)
        {
            return Task.Run(() => Calculate(file));
        }

        public RiskResult Calculate(HttpPostedFileBase file)
        {
            DataSet dataSet;

            using (var reader = Excel.ExcelReaderFactory.CreateBinaryReader(file.InputStream))
                dataSet = reader.AsDataSet();

            var factors = dataSet.Tables["Рисковые факторы"]
                .AsEnumerable()
                .Select(row => new RiskFactor
                {
                    Id = (int)row.Field<double>(0),
                    Name = row.Field<string>(1),
                    Frequency = row.Field<double>(2)
                })
                .ToList();
            var events = dataSet.Tables["Рисковые события"]
                .AsEnumerable()
                .Select(row => new Models.RiskEvent
                {
                    Id = (int)row.Field<double>(0),
                    Name = row.Field<string>(1),
                    Event = new RiskEventFactory().CreateExtremum(row.Field<double>(2), row.Field<double>(3), row.Field<double>(4))
                })
                .ToList();
            var relations = dataSet.Tables["Соотношения"]
                .AsEnumerable()
                .Select(row => new RiskRelation
                {
                    Factor = factors[(int)row.Field<double>(0) - 1],
                    Event = events[(int)row.Field<double>(1) - 1],
                })
                .ToList();

            return Calculate(new RiskModel
            {
                Factors = factors,
                Events = events,
                Relations = relations
            });
        }

        public RiskResult Calculate(RiskModel model)
        {
            return new RiskResult
            {
                Mesh = Enumerable.Range(0, 100).Select(i => (i * 0.1).ToString()).ToList(),
                Results = (from ev in model.Events
                           let color = (Color)typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)[ev.Id + 3].GetValue(null)
                           select new ComplexDataset
                           {
                               Label = ev.Name,
                               Data = (from r in model.Relations
                                       where r.Event.Id == ev.Id
                                       select Enumerable.Range(0, (int)(4 * r.Factor.Frequency))
                                           .Aggregate(new List<RiskEvent> { r.Event.Event },
                                               (ri, i) => ri.Concat(new[] { ri.Last().Convolve(r.Event.Event) }).ToList())
                                           .Select((rr, i) => rr.ApplyWeight(Math.Exp(-r.Factor.Frequency) * Math.Pow(r.Factor.Frequency, i) / Factorial(i)))
                                           .Aggregate((r1, r2) => r1.Sum(r2)))
                                    .Aggregate((r1, r2) => r1.Sum(r2))
                                    .ApplyWeight(1).ρNet
                                    .Aggregate(new List<double>(), (l, d) =>
                                    {
                                        l.Add(l.LastOrDefault() + d * 0.1);
                                        return l;
                                    })
                                    .Select(x => Math.Round(x * 100, 2))
                                    .ToList(),
                               FillColor = $"rgba({color.R / 2},{color.G / 2},{color.B / 2},0.2)",
                               StrokeColor = $"rgba({color.R / 2},{color.G / 2},{color.B / 2},1)",
                               PointColor = $"rgba({color.R / 2},{color.G / 2},{color.B / 2},1)",
                               PointStrokeColor = "#fff",
                               PointHighlightFill = "#fff",
                               PointHighlightStroke = $"rgba({color.R / 2},{color.G / 2},{color.B / 2},1)"
                           }).ToList(),
            };
        }
    }
}