using System.Collections.ObjectModel;

namespace AloPrefeitoP.Models
{
    public class GrupoBuscaResultado : ObservableCollection<ChatBuscaResultado>
    {
        public string TituloGrupo { get; }

        public GrupoBuscaResultado(string tituloGrupo, IEnumerable<ChatBuscaResultado> itens)
            : base(itens)
        {
            TituloGrupo = tituloGrupo;
        }
    }
}