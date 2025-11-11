namespace PSI.Models
{
    public abstract class SongCollection
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public int popularity { get; set; } = 0;
        public abstract void IncreasePopularity();
    }
}
