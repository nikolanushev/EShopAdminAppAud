using EShopAdminApp.Models;
using GemBox.Document;
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
    public class OrderController : Controller
    {
        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:44354/api/admin/GetAllActiveOrders";
            HttpResponseMessage response = client.GetAsync(URL).Result;
            var data = response.Content.ReadAsAsync<List<Order>>().Result;

            return View(data);
        }

        public IActionResult GetOrderDetails(int id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:44354/api/admin/GetOrderDetails";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;
            var data = response.Content.ReadAsAsync<Order>().Result;

            return View(data);
        }

        public FileResult SavePdf(int id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:44354/api/admin/GetOrderDetails";

            var model = new
            {
                Id = id
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;
            var directoryPath = Directory.GetCurrentDirectory();
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");

            var document = DocumentModel.Load(templatePath);
            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{Username}}", result.OrderedBy.Username);

            StringBuilder sb = new StringBuilder();
            double totalPrice = 0;
            foreach(var item in result.Products)
            {
                totalPrice += item.Quantity * item.Product.ProductPrice;
                sb.AppendLine(item.Product.ProductName+", quantity: "+item.Quantity+", price: $"+item.Product.ProductPrice);
            }
            document.Content.Replace("{{ProductList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$"+totalPrice.ToString());

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}
