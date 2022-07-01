using vacunadosAPI.Models;

namespace vacunadosAPI.Util
{
    public static class Utility
    {
        public static List<Game> gameList = new List<Game>();
        public static int tempVar = 0; //Will save temporal values

        public static String generateRandomString() {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[36];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public static bool alreadyExist(string name) {
            bool exist = false;
            for (int i = 0; i < gameList.Count; i++)
            {
                if (gameList.ElementAt(i).name == name) { 
                    exist = true;
                }
            }
            return exist;
        }

        public static bool gameExists(string gameId) {
            bool exist = false;
            for (int i = 0; i < gameList.Count; i++)
            {
                if (gameList.ElementAt(i).gameId == gameId)
                {
                    tempVar = i;
                    exist = true;
                }
            }
            return exist;
        }

        public static bool inGameUser(string playerName) {
            bool exist = false;
            string[] actualPlayers = gameList.ElementAt(tempVar).players;
            for (int i = 0; i < actualPlayers.Length; i++)
            {
                if (actualPlayers[i] == playerName) {
                    exist = true;
                }
            }
            return exist;
        }
    }


}
