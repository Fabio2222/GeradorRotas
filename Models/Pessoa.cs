using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Pessoa
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }
        public string NomeEquipe { get; set; } = String.Empty;
        public string Error { get; set; } = String.Empty;
    }
}
