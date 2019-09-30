using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests
{
    class JsonHttpClient : HttpClient
    {
        public JsonHttpClient():base()
        {
            DefaultRequestHeaders.Add("Content-Type", "application/json");
        }
        public async Task<T> GetAsJsonAsync<T>(string url)
        {
            var response = await GetAsync(url);
            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
            else
                throw new HttpRequestException();

        }
        public async Task PostAsJsonAsync(string url, object data)
        {
            var response = await PostAsync(url, new StringContent(JsonSerializer.Serialize(data)));
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException();

        }
        public async Task<T> PostAsJsonAsync<T>(string url,object data)
        {
            var response = await PostAsync(url, new StringContent(JsonSerializer.Serialize(data)));
            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
            else
                throw new HttpRequestException();
               
        }
    }
}
