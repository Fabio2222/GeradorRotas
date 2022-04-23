using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeradorRotas.Frontend.Service
{
    public class ConnectToPessoaApi
    {
        private readonly static string _baseUri = "https://localhost:44378/api/";

        public async Task<List<Pessoa>> GetPeople()
        {
            List<Pessoa> pessoas = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiPesoa");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        pessoas = JsonConvert.DeserializeObject<List<Pessoa>>(responseBody);
                    }
                    else
                        pessoas = null;
                }

                return pessoas;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    pessoas[0].Error = exception.InnerException.Message;
                else
                    pessoas[0].Error = exception.StatusCode.ToString();

                return pessoas;
            }
        }

        public async Task<Pessoa> GetPersonById(string id)
        {
            Pessoa pessoa = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiPerson/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        pessoa = JsonConvert.DeserializeObject<Pessoa>(responseBody);
                    }
                    else
                        pessoa = null;
                }

                return pessoa;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    pessoa.Error = exception.InnerException.Message;
                else
                    pessoa.Error = exception.StatusCode.ToString();

                return pessoa;
            }
        }

        public async Task<Pessoa> GetPersonByName(string nome)
        {
            Pessoa pessoas = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiPessoa/nome/" + nome);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        pessoas = JsonConvert.DeserializeObject<Pessoa>(responseBody);
                    }
                    else
                        pessoas = null;
                }

                return pessoas;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    pessoas.Error = exception.InnerException.Message;
                else
                    pessoas.Error = exception.StatusCode.ToString();

                return pessoas;
            }
        }

        public async Task<Pessoa> CreateNewPerson(Pessoa pessoas)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(pessoas);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiPerson", content);

                    if (result.IsSuccessStatusCode)
                        return pessoas;
                    else
                        pessoas = null;

                    return pessoas;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    pessoas.Error = exception.InnerException.Message;
                else
                    pessoas.Error = exception.StatusCode.ToString();

                return pessoas;
            }
        }

        public async Task<Pessoa> EditPerson(Pessoa pessoa)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(pessoa);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PutAsync($"ApiPerson/{pessoa.Id}", content);

                    if (result.IsSuccessStatusCode)
                        return pessoa;
                    else
                        pessoa = null;

                    return pessoa;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    pessoa.Error = exception.InnerException.Message;
                else
                    pessoa.Error = exception.StatusCode.ToString();

                return pessoa;
            }
        }

        public async Task<Pessoa> RemovePerson(string id)
        {
            Pessoa pessoa = new();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var result = await client.DeleteAsync($"ApiPessoa/{id}");

                    if (result.IsSuccessStatusCode)
                        pessoa.Error = "ok";
                    else
                        pessoa.Error = "notSave"; // mudar

                    return pessoa;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    pessoa.Error = exception.InnerException.Message;
                else
                    pessoa.Error = exception.StatusCode.ToString();

                return pessoa;
            }
        }
    }
}
    

