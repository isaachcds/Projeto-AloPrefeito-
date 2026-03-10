namespace AloPrefeitoP.Models
{
    public class ChatBuscaResultado
    {
        public string ChatId { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Trecho { get; set; } = string.Empty;

        public DateTime DataMensagem { get; set; }
        public DateTime UltimaDataChat { get; set; }

        public bool IsBot { get; set; }

        public string OrigemTexto => IsBot ? "IA" : "Você";

        public string DataMensagemTexto =>
            DataMensagem.Date == DateTime.Today ? "Hoje" :
            DataMensagem.Date == DateTime.Today.AddDays(-1) ? "Ontem" :
            DataMensagem.ToString("dd/MM/yyyy");

        public string GrupoPorConversa => Titulo;

        public string GrupoPorData =>
            DataMensagem.Date == DateTime.Today ? "Hoje" :
            DataMensagem.Date == DateTime.Today.AddDays(-1) ? "Ontem" :
            DataMensagem.ToString("dd/MM/yyyy");
    }
}