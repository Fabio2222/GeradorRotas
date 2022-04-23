using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;

namespace GeradorRotas.Frontend.Service
{
    public class ConnectToCidadeApi
    {
        private readonly static string _baseUri = "https://localhost:44330/api/"; //mudar

        public async Task<List<Cidade>> GetCities()
        {
            List<Cidade> cidades = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiCidade");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        cidades = JsonConvert.DeserializeObject<List<Cidade>>(responseBody);
                    }
                    else
                        cidades = null;
                }

                return cidades;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    cidades[0].Error = exception.InnerException.Message;
                else
                    cidades[0].Error = exception.StatusCode.ToString();

                return cidades;
            }
        }

        public async Task<Cidade> GetCityById(string id)
        {
            Cidade cidade = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiCidade/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        cidade = JsonConvert.DeserializeObject<Cidade>(responseBody);
                    }
                    else
                        cidade = null;
                }

                return cidade;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    cidade.Error = exception.InnerException.Message;
                else
                    cidade.Error = exception.StatusCode.ToString();

                return cidade;
            }
        }

        public async Task<Cidade> CreateNewCity(Cidade cidade)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(cidade);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiCidade", content);

                    if (result.IsSuccessStatusCode)
                        return cidade;
                    else
                        cidade = null;

                    return cidade;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    cidade.Error = exception.InnerException.Message;
                else
                    cidade.Error = exception.StatusCode.ToString();

                return cidade;
            }
        }

        public async Task<Cidade> RemoveCidade(string id)
        {
            Cidade cidade = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var result = await client.DeleteAsync($"ApiCidade/{id}");

                    if (result.IsSuccessStatusCode)
                        cidade.Error = "ok";
                    else
                        cidade.Error = "notSave";

                    return cidade;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    cidade.Error = exception.InnerException.Message;
                else
                    cidade.Error = exception.StatusCode.ToString();

                return cidade;
            }
        }

    }
}
    

