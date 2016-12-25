using RiskApp.Calculations;
using RiskApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RiskApp.Controllers
{
    public class RiskController : Controller
    {
        // GET: Risk
        public ActionResult Index()
        {
            return View();
        }

        // POST: Risk/Create
        [HttpPost]
        public async Task<ActionResult> Create(HttpPostedFileBase file)
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
                .Select(row => new RiskEventFactory().CreateExtremum(row.Field<double>(2), row.Field<double>(3), row.Field<double>(4)))
                .ToList();
            var relations = dataSet.Tables["Соотношения"]
                .AsEnumerable()
                .Select(row => new RiskRelation
                {
                    Factor = factors[(int)row.Field<double>(0) - 1],
                    Event = events[(int)row.Field<double>(1) - 1],
                })
                .ToList();

            return View("Result", new RiskModel
            {
                Factors = factors,
                Events = events,
                Relations = relations
            });
        }
    }
}
