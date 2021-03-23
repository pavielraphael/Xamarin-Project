using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PizzaIllico.Mobile.Dtos.Pizzas;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface IApiService
    {
        Task<TResponse> Get<TResponse>(string url);

        Task<string> Post<TResponse>(string url, string myJson);

        Task<string> PostLogin<TResponse>(string url, string myJson);
        Task<TResponse> GetGetUser<TResponse>(string url, string token);
        Task<TResponse> PatchAccount<TResponse>(string url, string myJson, string token);
        Task<string> PatchPwd<TResponse>(string url, string myJson, string token);

        Task<TResponse> GetPizzas<TResponse>(string url, long idShop);
        Task<string> PostCommand<TResponse>(string url, string token, long idShop, string myJson);
        Task<TResponse> GetOrders<TResponse>(string url, string token);
    }
    
    public class ApiService : IApiService
    {
	    private const string HOST = "https://pizza.julienmialon.ovh/";
        private readonly HttpClient _client = new HttpClient();
        
        public async Task<TResponse> Get<TResponse>(string url)
        {
	        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, HOST + url);

	        HttpResponseMessage response = await _client.SendAsync(request);

	        string content = await response.Content.ReadAsStringAsync();

	        return JsonConvert.DeserializeObject<TResponse>(content);
        }
        public async Task<TResponse> GetPizzas<TResponse>(string url, long idShop)
        {
            string newUrl = "";
            for (int i = 0; i < url.Length; i++)
            {
                newUrl += url[i];
                if (i == 12)
                {
                    newUrl += idShop.ToString();
                    i = 20;
                }

            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, HOST + newUrl);
            HttpResponseMessage response = await _client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse> GetOrders<TResponse>(string url, string token)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, HOST + url);
            request.Headers.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await _client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<TResponse> GetGetUser<TResponse>(string url, string token)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, HOST + url);
            request.Headers.Add("Authorization", "Bearer " + token);
            HttpResponseMessage response = await _client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<string> Post<TResponse>(string url, string myJson)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, HOST + url);

            HttpResponseMessage response = await _client.PostAsync(HOST + url, new StringContent(myJson, Encoding.UTF8, "application/json"));
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Reponse DATA : " + content);

            Console.WriteLine($"CODE ERREUR + {response.StatusCode}");


            return content;
        }

        public async Task<string> PostLogin<TResponse>(string url, string myJson)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, HOST + url);
            
            HttpResponseMessage response = await _client.PostAsync(HOST + url, new StringContent(myJson, Encoding.UTF8, "application/json"));
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Reponse DATA : " + content);

            Console.WriteLine($"CODE ERREUR + {response.StatusCode}");


            return content;
        }
        public async Task<string> PostCommand<TResponse>(string url, string token, long idShop, string myJson)
        {
            string newUrl = "";
            for (int i = 0; i < url.Length; i++)
            {
                newUrl += url[i];
                if (i == 12)
                {
                    newUrl += idShop.ToString();
                    i = 20;
                }
            }
            Console.WriteLine("JSON " + newUrl + " " + myJson + " " + token + " " + idShop + " ");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _client.PostAsync(HOST + newUrl, new StringContent(myJson, Encoding.UTF8, "application/json"));


            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Salut " + content);
            return content;
            
        }

        public async Task<TResponse> PatchAccount<TResponse>(string url, string myJson, string token)
        {
            StringContent contentJson = new StringContent(myJson, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new(new HttpMethod("PATCH"), HOST + url)
            {
                Content = contentJson
            };
            request.Headers.Add("Authorization", "Bearer " + token);

            HttpResponseMessage response = await _client.SendAsync(request);


            string content = await response.Content.ReadAsStringAsync();


            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<string> PatchPwd<TResponse>(string url, string myJson, string token)
        {
            StringContent contentJson = new StringContent(myJson, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new(new HttpMethod("PATCH"), HOST + url)
            {
                Content = contentJson
            };
            request.Headers.Add("Authorization", "Bearer " + token);

            HttpResponseMessage response = await _client.SendAsync(request);


            string content = await response.Content.ReadAsStringAsync();


            return content;
        }
    }
}