using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUsuario.Configuracao;
using Models;
using MongoDB.Driver;

namespace ApiUsuario.Service
{
    public class ApiUsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuario;

        public ApiUsuarioService(IGRUsuarioSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _usuario = database.GetCollection<Usuario>(settings.GRUsuarioCollectionName);
        }

        public List<Usuario> Get()
        {
            List<Usuario> usuarios = new();

            usuarios = _usuario.Find(usuario => true).ToList();

            return usuarios;
        }

        public Usuario Get(string id)
        {
            Usuario usuario = new();

            usuario = _usuario.Find<Usuario>(usuario => usuario.Id == id).FirstOrDefault();

            return usuario;
        }

        public Usuario GetUserByUsername(string usuarionome)
        {
            Usuario usuario = new();

            usuario = _usuario.Find<Usuario>(usuario => usuario.Usuarionome == usuarionome).FirstOrDefault();

            return usuario;
        }

        public async Task<Usuario> Create(Usuario usuario)
        {
            var verifyUsuario = GetUserByUsername(usuario.Usuarionome);

            if (verifyUsuario != null)
            {
                usuario.Error = "temUsuario"; 

                return usuario;
            }

            _usuario.InsertOne(usuario);

            return usuario;
        }

        public void Update(string id, Usuario usuarioIn)
        {
            _usuario.ReplaceOne(usuario => usuario.Id == id, usuarioIn);
        }

        public void Remove(string id, Usuario usuarioIn)
        {
            _usuario.DeleteOne(usuario => usuario.Id == usuarioIn.Id);
        }
    }

}

