using System.Collections.Generic;
using ApiPessoa.Configuracao;
using Models;
using MongoDB.Driver;

namespace ApiPessoa.Service
{
    public class ApiPessoaService
    {
        private readonly IMongoCollection<Pessoa> _pessoa;

        public ApiPessoaService(IGRPessoaSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _pessoa = database.GetCollection<Pessoa>(settings.GRPessoaCollectionName);
        }

        public List<Pessoa> Get()
        {
            List<Pessoa> pessoas = new();

            pessoas = _pessoa.Find(pessoa => true).ToList();

            return pessoas;
        }

        public Pessoa Get(string id)
        {
            Pessoa pessoa = new();

            pessoa = _pessoa.Find<Pessoa>(pessoa => pessoa.Id == id).FirstOrDefault();

            return pessoa;
        }

        public Pessoa GetByName(string nome)
        {
            Pessoa pessoa = new();

            pessoa = _pessoa.Find<Pessoa>(pessoa => pessoa.Nome == nome).FirstOrDefault();

            return pessoa;
        }

        public Pessoa Create(Pessoa pessoa)
        {
            _pessoa.InsertOne(pessoa);

            return pessoa;
        }

        public void Update(string id, Pessoa personIn)
        {
            _pessoa.ReplaceOne(person => person.Id == id, personIn);
        }

        public void Remove(string id, Pessoa personIn)
        {
            _pessoa.DeleteOne(person => person.Id == personIn.Id);
        }
    }
}
    

