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
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var listaprodutos = _contexto.Produtos.ToList();
            int? maiorId = 0;
            foreach(var item in listaprodutos)
                if (item.Id >= maiorId)
                    maiorId = item.Id;

            var form = Request.Form;

            List<Produto> produtos = new List<Produto>(); ;
            bool lotevalido = true;
            string mensagemlote;

            foreach (var formFile in form.Files) (produtos, lotevalido, mensagemlote) = await _metodos.LerArquivoExcel(formFile, maiorId);

            if (lotevalido)
            {
                produtos.ForEach(p => _contexto.Produtos.AddAsync(p));
                await _contexto.SaveChangesAsync();
            }
            else
            {
                BadRequest();
            }
            return RedirectToAction(nameof(Get));
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
