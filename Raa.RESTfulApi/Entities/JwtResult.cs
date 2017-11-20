using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Entities
{
    public class JwtResult
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
