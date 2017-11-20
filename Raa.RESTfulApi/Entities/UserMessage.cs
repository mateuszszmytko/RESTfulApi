using MongoDB.Bson;
using Raa.AspNetCore.MongoDataContext.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raa.RESTfulApi.Entities
{
    public class UserMessage : IEntity<ObjectId>
    {
        public ObjectId Id { get; set ; }
        public string Message { get; set; }
        public ObjectId UserId { get; set; }
    }
}
