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
                    Propability = row.Field<double>(2)
                })
                .ToList();

            return Calculate(new RiskModel
            {
                Factors = factors,
                Events = events,
                Relations = relations
            });
        }

        static readonly Color[] colors = typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(p => p.GetValue(null)).Cast<Color>().ToArray();


        public RiskResult Calculate(RiskModel model)
        {
            var datas = (from factor in model.Factors
                         select new
                         {
                             name = factor.Name,
                             color = colors[factor.Id + 3],
                             data = (from r in model.Relations
                                     where r.Factor.Id == factor.Id
                                     select EnumerableExtensions.Fix(r.Event.Event, ev => ev.Convolve(r.Event.Event))
                                         .Take((int)Math.Ceiling(4 * factor.Frequency))
                                         .Select((rr, i) => rr.ApplyWeight(r.Propability * factor.Probability(i)))
                                         .Aggregate((r1, r2) => r1.Sum(r2)))
                                     .Aggregate((r1, r2) => r1.Sum(r2).Sum(r1.Convolve(r2)))
                                     .ApplyWeight(1).ρNet
                                     .Scan((double x, double y) => x + y)
                                     .Select(x => Math.Round(x * 10, 2))
                                     .TakeWhile(x => x < 100)
                                     .SkipEach(5)
                                     .ToList()
                         })
                        .ToList();

            var maxCount = datas.Max(data => data.data.Count);

            foreach (var data in datas)
                data.data.AddRange(Enumerable.Repeat(100d, maxCount - data.data.Count));

            return new RiskResult
            {
                Mesh = Enumerable.Range(0, maxCount)
                    .Select(i => (i * 0.5).ToString()).ToList(),
                Results = (from data in datas
                           let r = data.color.R * 2 / 3
                           let g = data.color.G * 2 / 3
                           let b = data.color.B * 2 / 3
                           select new ComplexDataset
                           {
                               Label = data.name,
                               Data = data.data,
                               FillColor = $"rgba({r},{g},{b},0.2)",
                               StrokeColor = $"rgba({r},{g},{b},1)",
                               PointColor = $"rgba({r},{g},{b},1)",
                               PointStrokeColor = "#fff",
                               PointHighlightFill = "#fff",
                               PointHighlightStroke = $"rgba({r},{g},{b},1)"
                           }).ToList(),
            };
        }
    }
}