using System.Collections.Generic;
using ApiEquipe.Configuracao;
using Models;
using MongoDB.Driver;

namespace ApiEquipe.Service
{
    public class ApiEquipeService
    {
        private readonly IMongoCollection<Equipe> _equipe;

        public ApiEquipeService(IGREquipeSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _equipe = database.GetCollection<Equipe>(settings.GREquipeCollectionName);
        }

        public List<Equipe> Get()
        {
            List<Equipe> equipes = new();

            equipes = _equipe.Find(equipe => true).ToList();

            return equipes;
        }

        public Equipe Get(string id)
        {
            Equipe equipe = new();

            equipe = _equipe.Find<Equipe>(equipe => equipe.Id == id).FirstOrDefault();

            return equipe;
        }

        public List<Equipe> GetCidade(string cidade)
        {
            List<Equipe> equipes = new();

            equipes = _equipe.Find(equipe => equipe.Cidade == cidade).ToList();

            return equipes;
        }

        public Equipe GetEquipesByEquipeNome(string equipeNome) 
        {
            Equipe equipe = new();

            equipe = _equipe.Find<Equipe>(equipe => equipe.Nome == equipeNome).FirstOrDefault();

            return equipe;
        }

        public Equipe Create(Equipe equipe)
        {
            _equipe.InsertOne(equipe);

            return equipe;
        }

        public void Update(string id, Equipe equipeIn)
        {
            _equipe.ReplaceOne(equipe => equipe.Id == id, equipeIn);
        }

        public void Remove(string id, Equipe equipeIn)
        {
            _equipe.DeleteOne(equipe => equipe.Id == equipeIn.Id);
        }
    }
}
    

