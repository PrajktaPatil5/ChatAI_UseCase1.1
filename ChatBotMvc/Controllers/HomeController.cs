using ChatBotMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ChatBotMvc.Controllers
{
    public class HomeController : Controller
    {
        const string API_KEY = "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";
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

       public IActionResult Create()
        {

            return View();
        }

        public async Task<IActionResult> Get(string input)
         {

            var promptSections = new[]
    {
        "Prompt section 1",
        "Prompt section 2",
        // Add more prompt sections as needed
    };

            var messages = promptSections.Select(section => new
            {
                role = "user",
                content = section
            }).ToList();

            messages.Add(new
            {
                role = "user",
                content = input
            });


            var options = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = input
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

                string filePath = "C:\\Users\\prajkta.patil\\source\\repos\\ChatAI\\ChatBotMvc\\Controllers\\hobbie.txt";  // Specify the file path
                                                                                                                           // Save the input prompt and result in the file
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Input: {input}");

                sb.AppendLine($"Result: {result}");

                System.IO.File.WriteAllText(filePath, sb.ToString());

                return Json(result);
                

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return View();

        }
    }
}