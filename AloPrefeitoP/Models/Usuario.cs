using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacilityCareApp.Models
{
    public class Usuario
    {
        [JsonPropertyName("vusuIdUsuario")]
        public int VusuIdUsuario { get; set; }

        [JsonPropertyName("vusuDtCriacao")]
        public DateTime? VusuDtCriacao { get; set; }

        [JsonPropertyName("vusuNmUsuario")]
        public string VusuNmUsuario { get; set; } = null!;

        [JsonPropertyName("vusuDsSenha")]
        public string VusuDsSenha { get; set; } = null!;

        [JsonPropertyName("vusuDsEmail")]
        public string? VusuDsEmail { get; set; }

        [JsonPropertyName("vusuNrCelular")]
        public string VusuNrCelular { get; set; } = null!;

        [JsonPropertyName("vusuDtNascimento")]
        public DateTime? VusuDtNascimento { get; set; }

        [JsonPropertyName("vusuDsObservacao")]
        public string? VusuDsObservacao { get; set; }

        [JsonPropertyName("vusuStUsuario")]
        public string VusuStUsuario { get; set; } = null!;

        [JsonPropertyName("vRecuperaSenha")]
        public string? VRecuperaSenha { get; set; }

        [JsonPropertyName("vusuDsSenhaConfirma")]
        public string? VusuDsSenhaConfirma { get; set; }

        [JsonPropertyName("vusuDtModificacao")]
        public DateTime? VusuDtModificacao { get; set; }

        [JsonPropertyName("vusuIdProfissional")]
        public int? VusuIdProfissional { get; set; }

        [JsonPropertyName("vusuNivelAcesso")]
        public int? VusuNivelAcesso { get; set; }

        [JsonPropertyName("vusuIdColaborador")]
        public int? VusuIdColaborador { get; set; }

        [JsonPropertyName("vusuIdPapel")]
        public int? VusuIdPapel { get; set; }

        [JsonPropertyName("vusuCpf")]
        public string? VusuCpf { get; set; }
    }

}
