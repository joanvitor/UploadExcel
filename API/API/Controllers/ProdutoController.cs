using API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        public readonly CapgeminiContexto _contexto;

        public ProdutoController(CapgeminiContexto contexto) => _contexto = contexto;

        // GET: api/<ProdutoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            ICollection<Produto> produtos = 
                _contexto
                .Produtos
                .Select(p => new Produto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    DataEntrega = p.DataEntrega,
                    Quantidade = p.Quantidade,
                    ValorUnidade = p.ValorUnidade
                }).ToList();

            return produtos;
            //return new string[] { "value1", "value2" };
        }

        // GET api/<ProdutoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProdutoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ProdutoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProdutoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
