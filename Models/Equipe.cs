using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Equipe
    {

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Display(Name = "Pessoas do Time")]
        public List<Pessoa> Pessoas { get; set; } = new();
        public string Error { get; set; } = String.Empty;
    }
}
