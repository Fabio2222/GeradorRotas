using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Arquivo
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Nome do Arquivo")]
        public string NomeArquivo { get; set; }
        public string CaminhoArquivo { get; set; }

        [Display(Name = "Serviço")]
        public string Servico { get; set; }

        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Display(Name = "Data de Geração")]
        public string Data { get; set; }
        public List<string> Colunas { get; set; } //é isso
        public List<string> Equipes { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
