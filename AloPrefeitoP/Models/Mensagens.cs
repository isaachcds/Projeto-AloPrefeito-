using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AloPrefeitoP.Models
{
    public class Mensagens
    {
        [Key]
        public int MensagensId { get; set; }
        public string Nome { get; set; }
        public string Mensagem { get; set; }
        public DateTime Data { get; set; }

        // Identifica a conversa
        public string ChatId { get; set; } = "";

        // Diferencia usuário x IA para bolhas
        // false = usuário | true = IA
        public bool IsBot { get; set; }
    }
}
