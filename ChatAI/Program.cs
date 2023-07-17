
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    const string API_KEY = "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";
    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {

     string prompts=   Console.ReadLine();
        await Generat(prompts);
    }

    static async Task Generat(string prompts)
    {
       
            var options = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompts
                    }
                },
                max_tokens = 3500,
                temperature = 0.2
            };

            var json = System.Text.Json.JsonSerializer.Serialize(options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", API_KEY);

            try
            {
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

            dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
            string result = jsonResponse.choices[0].message.content;
            Console.WriteLine("Chat Bot: " + result);


        }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        
    }

  
}