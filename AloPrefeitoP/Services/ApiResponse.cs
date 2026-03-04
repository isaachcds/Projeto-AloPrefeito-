using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AloPrefeitoP.Services
{
  public  class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        //propriedade some de leitura (um getter)
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    }
}
