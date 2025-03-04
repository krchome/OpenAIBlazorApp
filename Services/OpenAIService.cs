using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace OpenAIBlazorApp.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "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"; // Add your OpenAI API key here

        public OpenAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            try
            {
                var requestData = new
                {
                    model = "gpt-3.5-turbo", // Updated model
                    messages = new[]
                    {
                                new { role = "system", content = "You are a helpful assistant." },
                                new { role = "user", content = prompt }
                        }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content = JsonContent.Create(requestData);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                    return json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    return $"Error: {response.StatusCode} - {error}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        public async Task<List<string>> GenerateImageAsync(string prompt)
        {
            var requestBody = new
            {
                prompt = prompt,
                n = 3, // Number of images
                size = "512x512"
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/images/generations", requestContent);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (json.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array)
                {
                    var imageUrls = dataArray.EnumerateArray()
                                             .Select(img => img.GetProperty("url").GetString())
                                             .Where(url => !string.IsNullOrEmpty(url))
                                             .ToList();

                    return imageUrls; // Return list of image URLs
                }
                else
                {
                    throw new Exception("API response does not contain expected 'data' array.");
                }
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI API Error: {errorMessage}");
            }
        }
        //New method: Step 1 (to convert TTS)
        //New method: Step 1 (to convert TTS)


        public async Task<String> ConvertTextToSpeechAsync(string text, string voice)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty");


            var requestBody = new
            {
                model = "tts-1",
                input = text,
                voice = voice
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            // var jsonBody = JsonConvert.SerializeObject(requestBody);
            using (var client = _httpClient)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
               // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

                if (response.IsSuccessStatusCode)
                {
                    var audioBytes = await response.Content.ReadAsByteArrayAsync();
                    var base64Audio = Convert.ToBase64String(audioBytes);
                    var audioUrl = $"data:audio/mp3;base64,{base64Audio}";
                    return audioUrl; // Return the downloadable URL
                }
                else
                {
                    throw new Exception("Failed to convert text to speech.");
                }
            }
        }

        //public async Task<byte[]> ConvertTextToSpeechAsync(string text, string voice)//working method returns audio bytes 
        //{
            

        //    // Create the request payload
        //    var requestData = new
        //    {
        //        model = "tts-1", // Ensure this matches OpenAI's TTS model
        //        input = text,
        //        voice = voice
        //    };

        //    // Serialize request data to JSON
        //    var  jsonData = JsonSerializer.Serialize(requestData);


        //    // Send request to OpenAI
        //    using (var client = _httpClient)
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        //        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        //        var response = await client.PostAsync("https://api.openai.com/v1/audio/speech", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var audioBytes = await response.Content.ReadAsByteArrayAsync();
        //            return audioBytes;
        //        }
        //        else
        //        {
        //            throw new Exception("Failed to convert text to speech.");
        //        }
        //    }

        //}


    }

}






