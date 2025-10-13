namespace PSI.Models
{
    public class Song
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public string Link { get; set; } = string.Empty;

        public async Task<Stream> OpenStreamAsync(HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(Link))
            {
                throw new InvalidOperationException("Google Drive link for this song is not set.");
            }

            var response = await httpClient.GetAsync(Link);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch the song from Google Drive. Status code: {response.StatusCode}");
            }

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
