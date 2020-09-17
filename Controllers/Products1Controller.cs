using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace ProductMVCApp.Controllers
{
    public class Products1Controller : Controller
    {
        public IActionResult Index()
        {
            IEnumerable<Product> products = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44391/api/");
                //HTTP GET
                var responseTask = client.GetAsync("Products1");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Product>>();
                    readTask.Wait();
                    products = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    products = Enumerable.Empty<Product>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(products);


        }

        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(Product prod)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44391/api/Products1");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<Product>("Products1", prod);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(prod);
        }

    }

}