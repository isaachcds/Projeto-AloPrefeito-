using System;
using System.Collections.Generic;
using System.Text;

namespace AloPrefeitoP.Models
{
    public class ChatSession
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public string Titulo => CriadoEm.ToString("dddd (dd/MM)", new System.Globalization.CultureInfo("pt-BR"));
    }
}
