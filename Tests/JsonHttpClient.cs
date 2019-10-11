using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Tests
{
    class JsonHttpClient : HttpClient
    {
        public JsonHttpClient():base()
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            var response = await PostAsync(url, new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
                throw new Exception();

        }
        public async Task<T> PostAsJsonAsync<T>(string url,object data)
        {
            var response = await PostAsync(url, new StringContent(JsonSerializer.Serialize(data),System.Text.Encoding.UTF8,"application/json"));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                
                T d = JsonSerializer.Deserialize<T>(content);
                return d;
            }
            else
                throw new Exception(response.StatusCode+"  " +await response.Content.ReadAsStringAsync());
               
        }
    }
}
