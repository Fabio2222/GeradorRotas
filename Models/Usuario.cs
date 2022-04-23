using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Usuário")]
        public string Usuarionome { get; set; }

        [Display(Name = "Senha")]
        public string Senha { get; set; }
        public string Regra { get; set; } = "usuario";
        public string Error { get; set; } = string.Empty;
    }
}
