using EShopAdminApp.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EShopAdminApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportUsers(IFormFile file)
        {
            string pathToUplod = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(pathToUplod)) 
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            List<User> users = getAllUsersFromFile(file.FileName);

            HttpClient client = new HttpClient();
            string URL = "https://localhost:44354/api/admin/ImportAllUsers";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            return RedirectToAction("Index", "Order");
        }

        private List<User> getAllUsersFromFile(string fileName)
        {
            string path = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";
            List<User> users = new List<User>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using(var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using(var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new Models.User
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmedPassword = reader.GetValue(2).ToString()
                        });
                    }
                }
                return users;
            }

        }
    }
}
