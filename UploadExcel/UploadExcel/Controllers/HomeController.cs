using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        [HttpPost("UploadArquivo")]
        public async Task<IActionResult> UploadArquivo(IFormCollection form)
        {
            var arquivos = form.Files;
            List<Produto> produtos = new List<Produto>();
            foreach(var item in arquivos)
            {
                if (item == null || item.Length == 0)
                {
                    return RedirectToAction("");
                }
                using (var memoryStream = new MemoryStream())
                {
                    await item.CopyToAsync(memoryStream).ConfigureAwait(false);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        var totalRows = package.Workbook.Worksheets[0].Dimension?.Rows;
                        var totalCollumns = package.Workbook.Worksheets[0].Dimension?.Columns;
                        for (int j = 2; j <= totalRows.Value; j++) // começa abaixo do titulo
                        {
                            Produto prod = new Produto();
                            for (int k = 1; k <= totalCollumns.Value; k++)
                            {
                                switch (k)
                                {
                                    case 1:
                                        prod.DataEntrega = package.Workbook.Worksheets[0].Cells[j, k].Value.ToString();
                                        break;
                                    case 2:
                                        prod.Nome = package.Workbook.Worksheets[0].Cells[j, k].Value.ToString();
                                        break;
                                    case 3:
                                        prod.Quantidade = int.Parse(package.Workbook.Worksheets[0].Cells[j, k].Value.ToString());
                                        break;
                                    case 4:
                                        prod.ValorUnidade = double.Parse(package.Workbook.Worksheets[0].Cells[j, k].Value.ToString());
                                        break;
                                }
                            }
                            produtos.Add(prod);
                        }
                    }
                }
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
