using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UploadExcel.Models;

namespace UploadExcel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> VisualizarDados()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
            HttpResponseMessage resposta = await httpClient.GetAsync("produto");
            var dados = resposta.Content.ReadAsStringAsync().Result;
            var produtos = JsonConvert.DeserializeObject<List<Produto>>(dados);
            return View(produtos);
        }

        [HttpPost("UploadArquivo")]
        public async Task<IActionResult> UploadArquivo(IFormCollection form)
        {
            try
            {
                foreach (var item in form.Files)
                {
                    if (item != null)
                    {
                        using (var client = new HttpClient())
                        {
                            try
                            {
                                client.BaseAddress = new Uri("https://localhost:5001/api/produto/upload");
                                byte[] data;
                                using (var br = new BinaryReader(item.OpenReadStream()))
                                    data = br.ReadBytes((int)item.OpenReadStream().Length);
                                ByteArrayContent bytes = new ByteArrayContent(data);
                                MultipartFormDataContent multicontent = new MultipartFormDataContent();
                                multicontent.Add(bytes, "file", item.FileName);
                                var result = client.PostAsync(client.BaseAddress, multicontent).Result;
                                return StatusCode((int)result.StatusCode); //201 Created the request has been fulfilled, resulting in the creation of a new resource.
                            }
                            catch (Exception)
                            {
                                return StatusCode(500); // 500 is generic server error
                            }
                        }
                    }
                    return StatusCode(400); // 400 is bad request
                }
            }
            catch (Exception)
            {
                return StatusCode(500); // 500 is generic server error
            }
            return View(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
