using System.Text.Json;

namespace OpenLibrary.Infrastructure.Services
{
    public class OpenLibraryService
    {
        private readonly HttpClient _client;

        public OpenLibraryService(HttpClient client)
        {
            _client = client;
        }

        public async Task<Models.OpenLibrary?> SearchBookAync(string title)
        {
            try
            {
                if (string.IsNullOrEmpty(title))
                {
                    return null;
                }

                string query = title.Replace(" ", "+");
                string url = $"https://openlibrary.org/search.json?title={query}";

                var response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Models.OpenLibrary>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error: {e.Message}");
                return null;
            }
        }
    }
}