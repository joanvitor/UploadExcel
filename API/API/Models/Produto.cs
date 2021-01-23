using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    [Table("tb_produto")]
    public class Produto
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("data_entrega")]
        public string DataEntrega { get; set; }
        [Column("quantidade")]
        public int Quantidade { get; set; }
        [Column("valor")]
        public double ValorUnidade { get; set; }
    }
}
