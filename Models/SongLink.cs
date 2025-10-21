namespace PSI.Models
{
    public readonly struct SongLink
    {
        public string Value { get; }

        public SongLink(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Song link cannot be empty.", nameof(value));

            Value = NormalizeLink(value);
        }

        // This method converts Google Drive links into direct download links
        private static string NormalizeLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return string.Empty;

            const string drivePattern = "/d/";
            if (link.Contains(drivePattern))
            {
                var parts = link.Split('/');
                int idIndex = Array.IndexOf(parts, "d");
                if (idIndex >= 0 && idIndex + 1 < parts.Length)
                {
                    string fileId = parts[idIndex + 1];
                    return $"https://drive.google.com/uc?export=download&id={fileId}";
                }
            }

            return link; // Return unchanged if not a Drive link
        }

        public override string ToString() => Value;
    }
}
