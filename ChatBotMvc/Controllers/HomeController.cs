using ChatBotMvc.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace ChatBotMvc.Controllers
{
    public class HomeController : Controller
    {
        private string output_1;
       private static  int i = 1;
        const string API_KEY = "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";
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();
        private readonly IWebHostEnvironment _hostingEnvironment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }
       

      

        public ActionResult UploadFile(IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Get the file name
                string fileName = Path.GetFileName(fileUpload.FileName);

                // Define the path where you want to save the file
                string savePath = Path.Combine("C:\\Users\\prajkta.patil\\source\\repos\\ChatAI\\ChatBotMvc\\wwwroot\\Files\\", fileName);

                // Save the file to the specified path

                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    fileUpload.CopyTo(fileStream);
                }
                // Optionally, you can do further processing with the uploaded file here

                ViewBag.Message = "File uploaded successfully!";
            }
            else
            {
                ViewBag.Message = "Please select a file to upload.";
            }
            return RedirectToAction("Create");
        }
            public IActionResult Create(string file)
        {
            string filesDirectoryPath = Path.Combine(_hostingEnvironment.WebRootPath, "files");

            // Get the list of filenames from the directory
            var fileNames = Directory.GetFiles(filesDirectoryPath).Select(Path.GetFileName);

            ViewBag.FileNames = fileNames;
            if (file != null) { 
            string wwwRootPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwRootPath+ "\\Files\\", file);
            
          //  string filePath =filePaths;

                // Create a FileStream to open the file in read mode
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                    // Create a StreamReader to read the data from the file stream
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        // Use a StringBuilder to efficiently build the complete string
                        // in case the file is large
                        var stringBuilder = new System.Text.StringBuilder();

                        // Read the file line by line until the end of the file
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            stringBuilder.AppendLine(line);
                        }

                        // Get the complete string from the StringBuilder
                        string fileContent = stringBuilder.ToString();
                     
                        TempData["data"] = fileContent;
                        return Json(fileContent);
                    }
                   
                }
            }
            return View();
        }

        public async Task<IActionResult> Get(string input)
         {
          
            string data = TempData["data"] as string;
   

            var options = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content ="data:"+data+ "From the given data,"+ input
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
                //if (i == 1)
                //{
                //    output_1 = result;
                //}
                
                ViewBag.output = output_1;
                string filePath = "C:\\Users\\prajkta.patil\\source\\repos\\ChatAI\\ChatBotMvc\\Controllers\\Output.txt";
           
                string textToAppend = " Function "+ i + ":\n Input : \n" + input + "\n Result : \n" + result;


                // Create a FileStream in Append mode
                if (i == 1) {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                       
                        // Create a StreamWriter using the FileStream and specify the encoding
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        {
                            // Append the text to the file
                            streamWriter.WriteLine(textToAppend);
                        }
                    }
                }
                if (i == 2)
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                    {
                     
                        // Create a StreamWriter using the FileStream and specify the encoding
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        {
                            // Append the text to the file
                            streamWriter.WriteLine(textToAppend);
                        }
                    }
                }
                ViewBag.count = i;
                i++;
               
                TempData["data"]=result;
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