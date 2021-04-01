using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tourz_web.Models;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Tourz_web.Controllers
{
    public class HomeController : Controller

    {
        private readonly ILogger<HomeController> _logger;
        private string connectionString = "Server=172.16.160.21;Port=3306;Database=110078;Uid=110078;Pwd=nsRoUSEC;";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var names = GetNames();

            return View(names);
        }
        public List<string> GetNames()
        {
            string connectionString = "Server=172.16.160.21;Port=3306;Database=110078;Uid=110078;Pwd=nsRoUSEC;";

            List<string> names = new List<string>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("select * from tourz_events", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string Name = reader["Name"].ToString();

                        names.Add(Name);
                    }
                }
            }
            return names;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CountriesWeTour()
        {
            return View();
        }

        public IActionResult Gallery()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(PersonModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SavePerson(model);

            ViewData["formsuccess"] = "ök";

            return View();
        }

        private void SavePerson(PersonModel person)           
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tourz_contacts(fullname, email, msg) values(?fullname, ?email, ?msg)", conn);

                cmd.Parameters.Add("?fullname", MySqlDbType.Text).Value = person.fullname;
                cmd.Parameters.Add("?email", MySqlDbType.Text).Value = person.email;
                cmd.Parameters.Add("?msg", MySqlDbType.Text).Value = person.msg;
                cmd.ExecuteNonQuery();
            }   
            
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("insert into tourz_logindetails(email, password) values(?email, ?password)", conn);

                cmd.Parameters.Add("?email", MySqlDbType.Text).Value = person.email;
                cmd.Parameters.Add("?password", MySqlDbType.Text).Value = person.password;
            }
        }
        
        public IActionResult About()
        {
            return View();
        }
        public IActionResult login()
        {
            return View();
        }

        public IActionResult singup()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/HandleError/{code:int}")]
        public IActionResult HandleError(int code)
        {
            ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
            return View("~/Views/Home/404page.cshtml");
        }
    }
}
