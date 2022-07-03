namespace vacunadosAPI.Models
{
    public class Round
    {
        public int id { get; set; } = 0;
        public string leader { get; set; } = null!;
        public List<string> group { get; set; } = null!;
    }
}
