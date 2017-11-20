using Raa.AspNetCore.MongoDataContext.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Entities
{
    public class ApplicationRole : MongoIdentityRole
    {
        public ApplicationRole(string name) : base(name)
        {

        }
    }
}
