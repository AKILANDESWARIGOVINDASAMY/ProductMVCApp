using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityModel.Client;
using System.Text.Json;

namespace ProductMVCApp.Controllers
{
    public class Products1Controller : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Products1Controller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        // public IActionResult Index()
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = null;

            var httpClient = _httpClientFactory.CreateClient();

            var currentAccessToken = await HttpContext.GetTokenAsync("access_token");
            var responseFromTokenEndpoint = await httpClient.RequestTokenAsync(
                new TokenRequest
                {
                    Address =
                        "https://login.microsoftonline.com/99c6e427-2c54-4170-a99f-ceaee4ddd05d/oauth2/v2.0/token",
                    GrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                    ClientId = "77a8a9bb-97fa-4ff7-9e42-db2f6eb30057",
                    ClientSecret = "CE_5y_r7E9Pj-fa8-HfM6SxLOISA2-t_Ba",
                    Parameters =
                    {
                        { "assertion", currentAccessToken },
                        { "scope", "api://77a8a9bb-97fa-4ff7-9e42-db2f6eb30057/Fullaccess"}             
                    }
                });

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://localhost:44391/api/Products1");
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", responseFromTokenEndpoint.AccessToken);

            var response = await httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // error happened
                throw new Exception(response.ReasonPhrase);
            }

            var readTask = response.Content.ReadAsAsync<IList<Product>>();
            readTask.Wait();
            products = readTask.Result;
            return View(products);


            /*   using (var client = new HttpClient())
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
               }*/


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