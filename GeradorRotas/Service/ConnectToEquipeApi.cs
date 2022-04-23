using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeradorRotas.Frontend.Service
{
    public class ConnectToEquipeApi
    {
        private readonly static string _baseUri = "https://localhost:44338/api/"; // mudar

        public async Task<List<Equipe>> GetEquipes()
        {
            List<Equipe> equipes = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquipe");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equipes = JsonConvert.DeserializeObject<List<Equipe>>(responseBody);
                    }
                    else
                        equipes = null;
                }

                return equipes;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equipes[0].Error = exception.InnerException.Message;
                else
                    equipes[0].Error = exception.StatusCode.ToString();

                return equipes;
            }
        }

        public async Task<Equipe> GetEquipById(string id)
        {
            Equipe equipe = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquip/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equipe = JsonConvert.DeserializeObject<Equipe>(responseBody);
                    }
                    else
                        equipe = null;
                }

                return equipe;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equipe.Error = exception.InnerException.Message;
                else
                    equipe.Error = exception.StatusCode.ToString();

                return equipe;
            }
        }

        public async Task<List<Equipe>> GetEquipByCity(string cidade)
        {
            List<Equipe> equipes = new();
            Equipe equipe = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquip/city/" + cidade);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equipes = JsonConvert.DeserializeObject<List<Equipe>>(responseBody);
                    }
                    else
                        equipes = null;
                }

                return equipes;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equipes[0].Error = exception.InnerException.Message;
                else
                    equipes[0].Error = exception.StatusCode.ToString();

                return equipes;
            }
        }

        public async Task<List<Equipe>> GetEquipsByEquipsName(List<string> equipesNome)
        {
            List<Equipe> equipes = new();
            Equipe equipe = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    foreach (var equipeNome in equipesNome)
                    {
                        HttpResponseMessage response = await client.GetAsync("ApiEquip/equip/" + equipeNome);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = response.Content.ReadAsStringAsync().Result;

                            equipe = JsonConvert.DeserializeObject<Equipe>(responseBody);
                        }
                        else
                            equipe = null;

                        equipes.Add(equipe);
                    }
                }

                return equipes;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equipes[0].Error = exception.InnerException.Message;
                else
                    equipes[0].Error = exception.StatusCode.ToString();

                return equipes;
            }
        }

        public async Task<Equipe> CreateNewEquip(Equipe equipe)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(equipe);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiEquip", content);

                    if (result.IsSuccessStatusCode)
                        return equipe;
                    else
                        equipe = null;

                    return equipe;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equipe.Error = exception.InnerException.Message;
                else
                    equipe.Error = exception.StatusCode.ToString();

                return equipe;
            }
        }

        public async Task<Equipe> RemoveEquip(string id)
        {
            Equipe equipe = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var result = await client.DeleteAsync($"ApiEquip/{id}");

                    if (result.IsSuccessStatusCode)
                        equipe.Error = "ok";
                    else
                        equipe.Error = "notSave";

                    return equipe;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equipe.Error = exception.InnerException.Message;
                else
                    equipe.Error = exception.StatusCode.ToString();

                return equipe;
            }
        }
    }
}
    

