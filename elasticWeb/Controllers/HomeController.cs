﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using elasticWeb.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace elasticWeb.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            //try
            //{
                int b = 1;
                int c = 1;
                int a = b / c;

                using (HttpClient client = new HttpClient())
                {
                    var result = await client.GetAsync("http://localhost:1453/api/logs");
                    var data = JsonConvert.DeserializeObject<List<string>>(result.Content.ReadAsStringAsync().Result);
                    return View(data);
                }
            //}
            /*catch (Exception ex)
            {
                Log log = new Log()
                {
                    PostDate = DateTime.Now,
                    message = ex.Message,
                    UserID = 1
                };
                using (HttpClient client = new HttpClient())
                {
                    var data = JsonConvert.SerializeObject(log);
                    HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                    await client.PostAsync("http://localhost:1453/api/logs", content);

                    var result = await client.GetAsync("http://localhost:1453/api/logs");
                    var data2 = JsonConvert.DeserializeObject<List<string>>(result.Content.ReadAsStringAsync().Result);
                    return View(data2);
                }
            }*/
        }
        [HttpPost]
        public async Task<IActionResult> Detail(string error)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync($"http://localhost:1453/api/logs/{error}");
                var data = JsonConvert.DeserializeObject<List<Log>>(result.Content.ReadAsStringAsync().Result);
                return View(data);
            }
        }
        static int A(string argument)
        {
            // Handle null argument.
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }
            // Handle invalid argument.
            if (argument.Length == 0)
            {
                throw new ArgumentException("Zero-length string invalid", "argument");
            }
            return argument.Length;
        }
        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";
            // try
            // {
            A(null);
            // }
            /* catch (Exception ex)
             {
                 Log log = new Log()
                 {
                     PostDate = DateTime.Now,
                     message = ex.Message,
                     UserID = 2
                 };
                 using (HttpClient client = new HttpClient())
                 {
                     var data = JsonConvert.SerializeObject(log);
                     HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                     await client.PostAsync("http://localhost:1453/api/logs", content);
                 }
             }*/            

            return View();
        }

        public async Task<IActionResult> Contact()
        {
             ViewData["Message"] = "Your contact page.";
            //try
            //{
            A("");
            //}
            /*catch (Exception ex)
            {
                Log log = new Log()
                {
                    PostDate = DateTime.Now,
                    message = ex.Message,
                    UserID = 3
                };
                using (HttpClient client = new HttpClient())
                {
                    var data = JsonConvert.SerializeObject(log);
                    HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                    await client.PostAsync("http://localhost:1453/api/logs", content);
                }
            }*/           

            return View();
        }

        public async Task<IActionResult> Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            Log log = new Log()
            {
                PostDate = DateTime.Now,
                message = exceptionFeature.Error.Message,
                UserID =GetUserIDByPage(exceptionFeature.Path.Replace("/Home/",""))
                };
            using (HttpClient client = new HttpClient())
            {
                var data = JsonConvert.SerializeObject(log);
                HttpContent content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                await client.PostAsync("http://localhost:1453/api/logs", content);
            }
            return View(exceptionFeature.Path.Replace("/Home/",""));
        }

        public int GetUserIDByPage(string page)
        {
            switch (page)
            {
                case "About":
                    {
                        return 3;
                    }
                case "Contact":
                    {
                        return 2;

                    }
                case "Index":
                    {
                        return 1;
                    }
                default:
                    {
                        return 1;
                    }
            }
        }
    }
}
