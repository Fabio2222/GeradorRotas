using GeradorRotas.Frontend.Configuracao;
using Models;
using MongoDB.Driver;
using System.Collections.Generic;

namespace GeradorRotas.Frontend.Service
{
    public class GRotasService
    {

        private readonly IMongoCollection<Arquivo> _arquivo;

        public GRotasService(IGRotasSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _arquivo = database.GetCollection<Arquivo>(settings.GRotasCollectionName);
        }

        public List<Arquivo> Get()
        {
            List<Arquivo> hgFile = new();

            hgFile = _arquivo.Find(hgFile => true).ToList();

            return hgFile;
        }

        public Arquivo Get(string id)
        {
            Arquivo hgFile = new();

            hgFile = _arquivo.Find(hgFile => hgFile.Id == id).FirstOrDefault();

            return hgFile;
        }

        public void Create(Arquivo hgFile)
        {
            _arquivo.InsertOne(hgFile);
        }

        public async void Delete(string id)
        {
            _arquivo.DeleteOne(equip => equip.Id == id);
        }
    }
}
    

