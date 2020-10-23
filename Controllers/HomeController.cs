using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Cloud.BigQuery.V2;


namespace VisProject.Controllers
{
    public class HomeController : Controller
    {


        [HttpGet("")]
        public IActionResult Index()   
        {   
            string projectId = "INSERT_CLOUD_PROJECT_ID_HERE";
            var client = BigQueryClient.Create(projectId);
            string query = @"
                WITH root AS (
                    SELECT 
                        project_name,
                        dependency_name,
                    FROM 
                        `bigquery-public-data.libraries_io.dependencies`
                    WHERE
                        project_name = 'linux'
                    ),

                    second_level AS (
                    SELECT
                        project_name,
                        dependency_name,
                    FROM
                        `bigquery-public-data.libraries_io.dependencies`
                    WHERE
                        project_name IN (SELECT dependency_name FROM root)
                    ),

                    main_query AS (
                    SELECT 
                        root.project_name AS name,
                        root.dependency_name AS dependency,
                        second_level.dependency_name AS deep_dependency,
                    FROM 
                        root
                    LEFT JOIN second_level
                    ON root.dependency_name = second_level.project_name
                    GROUP BY 1, 2, 3
                    ORDER BY 1, 2, 3
                    )

                    SELECT 
                        CONCAT(name, ' ', dependency, ' ', deep_dependency) AS dependency_string
                    FROM
                    main_query";
            var result = client.ExecuteQuery(query, parameters: null);
            List<string> depList = new List<string>();
            foreach (BigQueryRow row in result)
            {
                if (row["dependency_string"] != null)
                {
                    string boop = row["dependency_string"].ToString();
                    depList.Add(boop);
                }
                else {}
            }
            string jsonString = JsonSerializer.Serialize(depList);
            ViewBag.jsonString = jsonString;
            string linux = "linux";
            ViewBag.uinput = linux;
            return View();
        }


        [HttpPost("query")]
         public IActionResult UserQuery(string uinput)
         {
            Console.WriteLine(uinput);
            string projectId = "visualizer-293219";
            var client = BigQueryClient.Create(projectId);
            string query = $@"
                WITH root AS (
                    SELECT 
                        project_name,
                        dependency_name,
                    FROM 
                        `bigquery-public-data.libraries_io.dependencies`
                    WHERE
                        project_name = '{uinput}'
                    ),

                    second_level AS (
                    SELECT
                        project_name,
                        dependency_name,
                    FROM
                        `bigquery-public-data.libraries_io.dependencies`
                    WHERE
                        project_name IN (SELECT dependency_name FROM root)
                    ),

                    main_query AS (
                    SELECT 
                        root.project_name AS name,
                        root.dependency_name AS dependency,
                        second_level.dependency_name AS deep_dependency,
                    FROM 
                        root
                    LEFT JOIN second_level
                    ON root.dependency_name = second_level.project_name
                    GROUP BY 1, 2, 3
                    ORDER BY 1, 2, 3
                    )

                    SELECT 
                        CONCAT(name, ' ', dependency, ' ', deep_dependency) AS dependency_string
                    FROM
                    main_query";
                var result = client.ExecuteQuery(query, parameters: null);
                List<string> depList = new List<string>();
                foreach (BigQueryRow row in result)
                {
                if (row["dependency_string"] != null)
                {
                    string boop = row["dependency_string"].ToString();
                    depList.Add(boop);
                }
                else {}
            }
            string jsonString = JsonSerializer.Serialize(depList);
            ViewBag.uinput = uinput;
            ViewBag.jsonString = jsonString;
            return View("index");
         }      
    }
}
