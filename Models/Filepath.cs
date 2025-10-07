namespace PSI.Models
{
    public readonly struct FilePath
    {
        public string Value { get; }

        public FilePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(value));
            }

            Value = NormalizePath(value);
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        public override string ToString() => Value;
    }
}