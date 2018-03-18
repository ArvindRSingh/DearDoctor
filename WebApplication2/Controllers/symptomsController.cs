using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.Framework.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using LogLevel = Microsoft.Framework.Logging.LogLevel;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class symptomsController : Controller
    {
        readonly CloudStorageAccount storageAccount;
        readonly CloudTableClient tableClient;
        CloudTable table;

        public symptomsController()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create the table client.
            tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            table = tableClient.GetTableReference("symptoms");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
        }

        // GET: symptoms
        public ActionResult Index(string[] symptoms, AgeGroup? ageGroup, Gender? gender, Region? region)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, symptoms[0]);

            int symptomsIndex = 1;
            while (symptomsIndex != symptoms.Length)
            {
                filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.Or,
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, symptoms[1])
                    );
                symptomsIndex++;
            }

            filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, SymptomEntity.GetRowKey(ageGroup, gender, region))
                    );

            // Create the table query.
            TableQuery <SymptomEntity> rangeQuery = new TableQuery<SymptomEntity>()
                .Where(filter);

            // Loop through the results, displaying information about the entity.

            var entities = table.ExecuteQuery(rangeQuery).ToList();

            return View(entities);
        }

        // GET: symptoms/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: symptoms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: symptoms/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: symptoms/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: symptoms/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: symptoms/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: symptoms/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
