using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Models
{
    public class ReturnUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
