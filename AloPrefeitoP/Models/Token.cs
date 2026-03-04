using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AloPrefeitoP.Models
{
    class Token
    {
        [JsonPropertyName("accesstoken")]
        public string? Accesstoken { get; set; }

        [JsonPropertyName("tokentype")]
        public string? TokenType { get; set; }

        [JsonPropertyName("usuarioid")]
        public int? UsuarioId { get; set; }

        [JsonPropertyName("usuarionome")]
        public string? UsuarioNome { get; set; }

        [JsonPropertyName("usuarioidpessoa")]
        public int? UsuarioIdPessoa { get; set; }
        [JsonPropertyName("usuariophone")]
        public string? UsuarioPhone { get; set; }
        [JsonPropertyName("usuarioemail")]
        public string? UsuarioEmail { get; set; }

        [JsonPropertyName("usuarionascimento")]
        public string? UsuarioNasc { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime? Expiration { get; set; }
        

    }
}
