using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class CapgeminiContexto : DbContext
    {
        public CapgeminiContexto(DbContextOptions<CapgeminiContexto> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }
    }
}
