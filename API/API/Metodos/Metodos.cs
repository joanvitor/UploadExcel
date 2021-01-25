using API.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace API.Metodos
{
    public class Metodos
    {
        public async Task<(List<Produto>, bool, string)> LerArquivoExcel(IFormFile arquivo, int? maiorId)
        {
            bool lotevalido = true;
            string mensagemLote = null;
            List<Produto> produtos = new List<Produto>();
            if (arquivo != null && arquivo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await arquivo.CopyToAsync(memoryStream).ConfigureAwait(false);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        var totalRows = package.Workbook.Worksheets[0].Dimension?.Rows;
                        var totalCollumns = package.Workbook.Worksheets[0].Dimension?.Columns;
                        for (int j = 2; j <= totalRows.Value; j++) // começa abaixo do titulo
                        {
                            Produto prod = new Produto();
                            prod.Id = ++maiorId;
                            for (int k = 1; k <= totalCollumns.Value; k++)
                            {
                                switch (k)
                                {
                                    case 1:
                                        //var data = package.Workbook.Worksheets[0].Cells[j, k].Value.ToString();
                                        var data = package.Workbook.Worksheets[0].Cells[j, k].Value.ToString();
                                        var array = data.Split('/', ':', ' '); // se não splitar é porque a data veio numerica  44656
                                        if (array.Length > 2) // houve split
                                        {
                                            var newDate = array[2] + "-" + array[1] + "-" + array[0];
                                            prod.DataEntrega = newDate;
                                        }
                                        else
                                        {
                                            var newDate = DateTime.FromOADate(double.Parse(data)).ToString();
                                            var dataarray = newDate.Split('/', ':', ' ');
                                            prod.DataEntrega = dataarray[2] + "-" + dataarray[1] + "-" + dataarray[0];
                                        }
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
                            (var valido, var mensagem) = ValidaProduto(prod);
                            if (lotevalido)
                                lotevalido = valido;
                            if (!string.IsNullOrEmpty(mensagem))
                                mensagemLote += $"\n {mensagem}";
                            produtos.Add(prod);
                        }
                    }
                }
                return (produtos, lotevalido, mensagemLote);
            }
            return (null, false, null);
        }

        private (bool, string) ValidaProduto(Produto produto)
        {
            bool valido = true;
            var mensagem = string.Empty;
            if(!produto.Id.HasValue)
            {
                mensagem = $"Produto da linha {produto.Id} sem identificação";
                valido = false;
            }
            if (string.IsNullOrEmpty(produto.Nome))
            {
                if(!valido)
                    mensagem += ", sem nome";
                else
                    mensagem = $"Produto da linha {produto.Id} sem nome";
                valido = false;
            } else if (produto.Nome.Length > 50)
            {
                if (!valido)
                    mensagem = ", com nome maior que 50 caracteres";
                else
                    mensagem = $"Produto da linha {produto.Id} com nome maior que 50 caracteres";
                valido = false;
            }
            if (string.IsNullOrEmpty(produto.DataEntrega))
            {
                if (!valido)
                    mensagem += ", sem data";
                else
                    mensagem = $"Produto da linha {produto.Id} sem data";
                valido = false;
            }
            var now = DateTime.Now;
            var dataProd = DateTime.Parse(produto.DataEntrega);

            if (DateTime.Compare(dataProd, now) <= 0)
            {
                if (!valido)
                    mensagem += ", e com a data de entrega é menor que o dia de hoje";
                else
                    mensagem = $"Produto da linha {produto.Id} com a data de entrega menor que o dia de hoje";
                valido = false;
            }
            if (!produto.Quantidade.HasValue)
            {
                if (!valido)
                    mensagem += ", sem quantidade";
                else
                    mensagem = $"Produto da linha {produto.Id} sem quantidade";
                valido = false;
            } else if (produto.Quantidade < 0)
            {
                if (!valido)
                    mensagem += ", com a quantidade menor que 0";
                else
                    mensagem = $"Produto da linha {produto.Id} com a quantidade menor que 0";
                valido = false;
            }
            if (!produto.ValorUnidade.HasValue)
            {
                if (!valido)
                    mensagem += ", sem valor";
                else
                    mensagem = $"Produto da linha {produto.Id} sem valor";
                valido = false;
            }
            if (!string.IsNullOrEmpty(mensagem))
                mensagem += " !!!";
            return (valido, mensagem);
        }
    }
}
