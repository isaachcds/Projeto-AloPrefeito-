namespace AloPrefeitoP.Models
{
    public class ChatBuscaResultado
    {
        public string ChatId { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Trecho { get; set; } = string.Empty;
        public DateTime UltimaData { get; set; }

        public string UltimaDataTexto =>
            UltimaData.Date == DateTime.Today ? "Hoje" :
            UltimaData.Date == DateTime.Today.AddDays(-1) ? "Ontem" :
            UltimaData.ToString("dd/MM/yyyy");
    }
}