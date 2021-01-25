namespace UploadExcel.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string DataEntrega { get; set; }
        public int Quantidade { get; set; }
        public double ValorUnidade { get; set; }
        public double ValorTotal { get; set; }
    }
}
