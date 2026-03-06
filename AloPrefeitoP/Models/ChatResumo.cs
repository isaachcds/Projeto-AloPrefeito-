namespace AloPrefeitoP.Models
{
    public class ChatResumo
    {
        public string ChatId { get; set; } = string.Empty;

        // título exibido na lista (normalmente a 1ª msg do usuário)
        public string Titulo { get; set; } = string.Empty;

        // data/hora da última mensagem
        public DateTime UltimaData { get; set; }

        public string UltimaDataTexto => UltimaData.Date == DateTime.Today
            ? UltimaData.ToString("HH:mm")
            : UltimaData.ToString("dd/MM");
    }
}