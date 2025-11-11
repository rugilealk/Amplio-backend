namespace PSI.Models
{
    public abstract class SongCollection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int popularity { get; set; }
        public SongCollection(string name)
        {
            Name = name;
        }
    }
}
