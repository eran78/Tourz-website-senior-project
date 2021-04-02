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
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Tourz_web.Controllers
{
    public class HomeController : Controller

    {
        private readonly ILogger<HomeController> _logger;
        private string connectionString = "Server=informatica.st-maartenscollege.nl;Port=3306;Database=110078;Uid=110078;Pwd=nsRoUSEC;";
        private object wachtwoord;
        private object username;
        private object password;

        //private string connectionString = "Server=172.16.160.21;Port=3306;Database=110078;Uid=110078;Pwd=nsRoUSEC;";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public List<string> GetNames()
        {
         

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
            var names = GetNames();

            return View(names);
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
                MySqlCommand cmd = new MySqlCommand("insert into tourz_logindetails(username, password) values(?username, ?password)", conn);

                cmd.Parameters.Add("?username", MySqlDbType.Text).Value = person.username;
                cmd.Parameters.Add("?password", MySqlDbType.Text).Value = person.password;
            }
        }
        
        public IActionResult About()
        {
            return View();
        }
        public IActionResult login()
        {
            var klant = GetPersonByusername(username);

            if (klant.password != ComputeSha256Hash(password))
            {
                return View();
            }
            HttpContext.Session.SetInt32(
                "UserId",
                klant.Id);
            HttpContext.Session.SetString(
                "UserName",
                klant.username);
            return Redirect("/profiel");
        }

        private object ComputeSha256Hash(object password)
        {
            throw new NotImplementedException();
        }

        private object GetPersonByusername(object username)
        {
            throw new NotImplementedException();
        }


        public IActionResult signup()
        {
            return View();
        }

        public IActionResult Contact()
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

    private static string ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

}
