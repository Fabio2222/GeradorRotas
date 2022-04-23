using System.Collections.Generic;
using ApiCdade.Configuracao;
using Models;
using MongoDB.Driver;

namespace ApiCdade.Service
{
    public class ApiCidadeService
    {
        private readonly IMongoCollection<Cidade> _cidade;

        public ApiCidadeService(IGRCidadeSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _cidade = database.GetCollection<Cidade>(settings.GRCidadeCollectionName);
        }

        public List<Cidade> Get()
        {
            List<Cidade> _cidade = new();

            cidade = _cidade.Find(cidade => true).ToList();

            return cidade;
        }

        public Cidade Get(string id)
        {
            Cidade cidade = new();

            cidade = _cidade.Find<Cidade>(cidade => cidade.Id == id).FirstOrDefault();

            return cidade;
        }

        public Cidade GetName(string name)
        {
            Cidade cidade = new();

            cidade = _cidade.Find<Cidade>(cidade => cidade.Name == name).FirstOrDefault();

            return cidade;
        }

        public Cidade Create(Cidade cidade)
        {
            Cidade existsCidade = new();

            existsCidade = GetName(cidade.Name);

            if (existsCidade == null)
                _cidade.InsertOne(cidade);

            return cidade;
        }

        public void Update(string id, Cidade cidadeIn)
        {
            _cidade.ReplaceOne(cidade => cidade.Id == id, cidadeIn);
        }

        public void Remove(string id, Cidade ccidadeIn)
        {
            _cidade.DeleteOne(cidade => cidade.Id == cidadeIn.Id);
        }
    }
}