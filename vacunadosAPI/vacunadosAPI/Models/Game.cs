namespace vacunadosAPI.Models
{
    public class Game
    {

        public string gameId { get; set; } = null!;
        
        public string name { get; set; } = null!;

        public string owner { get; set; } = null!;

        public string password { get; set; } = null!;

        public List<string> players { get; set; } = null;

        public List<string> psychos { get; set; } = null!;
        
        public List<bool> psychoWin { get; set; } = null!;

        public string status { get; set; } = null!;

        public List<Round> rounds { get; set; } = null!;




    }
}
