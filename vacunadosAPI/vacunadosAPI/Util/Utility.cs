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
            List<string> actualPlayers = gameList.ElementAt(tempVar).players;
            for (int i = 0; i < actualPlayers.Count; i++)
            {
                if (actualPlayers[i] == playerName) {
                    exist = true;
                }
            }
            return exist;
        }


        public static bool groupExists(List<Group> group, int position)
        {
            bool exist = false;

            /*if (gameList.ElementAt(position).rounds != null)
            {
                for (int i = 0; i < gameList.ElementAt(position).rounds.ElementAt(i).group.Count; i++)
                {
                    if (gameList.ElementAt(position).rounds.ElementAt(i).group == group)
                    {
                        exist = true;
                    }
                }
            }*/
            return exist;
        }

        public static int getPsychosQuantity(int players)
        {
            int psychosQuantity = 0;

            if (players == 5 || players == 6)
            {
                psychosQuantity = 2;
            }
            else
            {
                if (players == 7 || players == 8 || players == 9)
                {
                    psychosQuantity = 3;
                }
                else
                {
                    psychosQuantity = 4;
                }
            }

            return psychosQuantity;
        }

        public static List<string> setPsychos(List<string> players) {

            List<string> psychos = new List<string>();
            int psychosQuantity = getPsychosQuantity(players.Count);
            int index;
            var random = new Random();

            for (int i = 0; i < psychosQuantity; i++)
            {
                do
                {
                    index = random.Next(players.Count); 
                } while (psychos.Contains(players.ElementAt(index)));
                psychos.Insert(i, players.ElementAt(index));
            }

            return psychos;
        }

        public static string getRoundLeader(List<string> players) {
            string leader = "";

            var random = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                leader = players.ElementAt(random.Next(players.Count));
            }

            return leader;
        }

        public static int getRoundGroup(int round, int players) {
            int[,] mat = new int[5, 6] { { 2, 2, 2, 3, 3, 3 },
                                         { 3, 3, 3, 4, 4, 4 },
                                         { 2, 4, 3, 4, 4, 4 },
                                         { 3, 3, 4, 5, 5, 5 },
                                         { 3, 4, 4, 5, 5, 5 } };

            int groupPlayers = 0;

            switch (players)
            {
                case 5:
                    groupPlayers = mat[round - 1, 0]; 
                    break;
                case 6:
                    groupPlayers = mat[round - 1, 1];
                    break;
                case 7:
                    groupPlayers = mat[round - 1, 2];
                    break;
                case 8:
                    groupPlayers = mat[round - 1, 3];
                    break;
                case 9:
                    groupPlayers = mat[round - 1, 4];
                    break;
                default:
                    groupPlayers = mat[round - 1, 5];
                    break;
            }

            return groupPlayers;
        }

    }


}
