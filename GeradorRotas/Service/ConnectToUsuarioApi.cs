using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeradorRotas.Frontend.Service
{
    public class ConnectToUsuarioApi
    {
        private readonly static string _baseUri = "https://localhost:44309/api/"; //mudar

        public async Task<List<Usuario>> GetUsers()
        {
            List<Usuario> usuarios = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiUser");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        usuarios = JsonConvert.DeserializeObject<List<Usuario>>(responseBody);
                    }
                    else
                        usuarios = null;
                }

                return usuarios;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    usuarios[0].Error = exception.InnerException.Message;
                else
                    usuarios[0].Error = exception.StatusCode.ToString();

                return usuarios;
            }
        }

        public async Task<Usuario> GetUserByUsername(string username)
        {
            Usuario usuario = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiUser/login/" + username);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        usuario = JsonConvert.DeserializeObject<Usuario>(responseBody);
                    }
                    else
                        usuario = null;
                }

                return usuario;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    usuario.Error = exception.InnerException.Message;
                else
                    usuario.Error = exception.StatusCode.ToString();

                return usuario;
            }
        }

        public async Task<Usuario> CreateNewUser(Usuario usuario)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(usuario);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiUser", content);

                    if (result.IsSuccessStatusCode)
                        return usuario;
                    else
                        usuario = null;

                    return usuario;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    usuario.Error = exception.InnerException.Message;
                else
                    usuario.Error = exception.StatusCode.ToString();

                return usuario;
            }
        }
    }
}
    
