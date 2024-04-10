using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portmoneu.Models.Helpers
{
    //generic response class, an upgrade from previous ResponseFlag
    public class ServiceResponse<T>
    {
        public T Data { get; set; } = default(T);
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
    }
}
