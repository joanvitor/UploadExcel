using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly Metodos.Metodos _metodos;

        public ProdutoController(CapgeminiContexto contexto)
        {
            _contexto = contexto;
            _metodos = new Metodos.Metodos();
        }

        // GET: api/<ProdutoController>
        [HttpGet]
        public IEnumerable<Produto> Get()
        {
            ICollection<Produto> produtos =
                _contexto
                .Produtos
                .ToList();
            return produtos;
        }

        [HttpPost("Upload")]
        public async Task<List<Produto>> Upload(List<IFormFile> files)
        {
            var form = Request.Form;

            foreach (var formFile in form.Files)
            {
                (var produtos, var lotevalido, var mensagemlote) = await _metodos.LerArquivoExcel(formFile);
                if (lotevalido)
                {
                    produtos.ForEach(p => _contexto.Produtos.AddAsync(p));
                    await _contexto.SaveChangesAsync();
                }
                else
                {
                    BadRequest();
                }
            }
            var listaprodutos = _contexto.Produtos.ToList();
            return listaprodutos;
        }

        // GET api/<ProdutoController>/5
        [HttpGet("{id}")]
        public Produto Get(int id)
        {
            var produto = _contexto
                .Produtos
                .SingleOrDefault(p => p.Id == id);
            return produto;
        }

        // POST api/<ProdutoController>
        [HttpPost]
        public void Post([FromBody] string value) // insert
        {
        }

        // PUT api/<ProdutoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) // alteração
        {
        }
    }
}
