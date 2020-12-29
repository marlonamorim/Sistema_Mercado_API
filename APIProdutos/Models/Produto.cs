namespace APIProdutos.Models
{
    public class Produto
    {
        private string _codigoBarras;
        public string CodigoBarras
        {
            get => _codigoBarras;
            set => _codigoBarras = value?.Trim().ToUpper();
        }

        private string _nome;
        public string Nome
        {
            get => _nome;
            set => _nome = value?.Trim();
        }

        private string _imagem;
        public string Imagem 
        { 
            get => _imagem; 
            set => _imagem = value?.Trim(); 
        }

        public double Preco { get; set; }
    }
}