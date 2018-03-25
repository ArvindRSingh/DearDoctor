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
    [Authorize]
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
        public ActionResult Index(string[] symptoms, AgeGroup? ageGroup, Gender? gender, Region? region, bool showAll = false)
        {
            TableQuery<SymptomEntity> rangeQuery;
            if (showAll)
            {
                rangeQuery = new TableQuery<SymptomEntity>();
            }
            else
            {
                if (symptoms == null || symptoms.Length == 0)
                {
                    return View(new List<SymptomEntity>());
                }

                var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, symptoms[0]);

                int symptomsIndex = 0;
                while (symptomsIndex != symptoms.Length)
                {
                    filter = TableQuery.CombineFilters(
                        filter,
                        TableOperators.Or,
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, symptoms[symptomsIndex])
                        );
                    symptomsIndex++;
                }

                filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, SymptomEntity.GetRowKey(ageGroup, gender, region))
                    );

                // Create the table query.
                rangeQuery = new TableQuery<SymptomEntity>()
                    .Where(filter);
            }


            // Loop through the results, displaying information about the entity.
            var entities = table.ExecuteQuery(rangeQuery).ToList();

            return View(entities);
        }

        // GET: search
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symptoms"></param>
        /// <param name="ageGroup"></param>
        /// <param name="gender"></param>
        /// <param name="region"></param>
        /// <param name="showAll"></param>
        /// <returns>SymptomSearchViewModel</returns>
        public ActionResult Search(SymptomSearchViewModelPure vm)
        {
            TableQuery<SymptomEntity> rangeQuery;
           
            
                if (vm.Symptoms == null)
                {
                    return View(new SymptomSearchViewModelPure());
                }
                else
                {
                    vm.Symptoms = vm.Symptoms.Where(x => x != "").Distinct().ToList();
                    if (vm.Symptoms.Count()== 0)
                    {
                        return View(new SymptomSearchViewModelPure());
                    }
                }

                var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vm.Symptoms[0]);
                int symptomsIndex = 1;
                while ( symptomsIndex != vm.Symptoms.Count)
                {
                    filter = TableQuery.CombineFilters(
                        filter,
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vm.Symptoms[symptomsIndex])
                        );
                    symptomsIndex++;
                }

                filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, SymptomEntity.GetRowKey(vm.AgeGroup, vm.Gender, vm.Region))
                    );

                // Create the table query.
                rangeQuery = new TableQuery<SymptomEntity>()
                    .Where(filter);
            


            // Loop through the results, displaying information about the entity.
            var entities = table.ExecuteQuery(rangeQuery).ToList();
            
            vm.Diagnosis = entities;
            if (vm.Symptoms ==null || vm.Symptoms.Count() == 0)
            {
                vm.Symptoms = new List<string>(5);
            }
             
            return View(vm);
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
        public ActionResult Create(SymptomEntity symptomEntity)
        {
            try
            {

                var result = table.Execute(TableOperation.InsertOrReplace(symptomEntity));

                return RedirectToAction("Index", new { showAll = true });
            }
            catch
            {
                return View();
            }
        }

        // GET: symptoms/Edit/5
        public ActionResult Edit(string symptom, AgeGroup ageGroup, Gender gender, Region region)
        {
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, symptom);
            filter = TableQuery.CombineFilters(
                    filter,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, SymptomEntity.GetRowKey(ageGroup, gender, region))
                    );
            new TableQuery().Where(filter);
            var rangeQuery = new TableQuery<SymptomEntity>()
                    .Where(filter);
            var entity = table.ExecuteQuery(rangeQuery).FirstOrDefault();
            return View(entity);
        }

        // POST: symptoms/Edit/5
        [HttpPost]
        public ActionResult Edit(SymptomEntity record)
        {
            try
            {
                var result = table.Execute(TableOperation.Replace(record));

                return RedirectToAction("Index", new { showAll=true});

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

                return RedirectToAction("Index", new { showAll = true });
            }
            catch
            {
                return View();
            }
        }
    }
}
