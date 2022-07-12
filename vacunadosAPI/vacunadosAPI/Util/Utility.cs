using vacunadosAPI.Models;

namespace vacunadosAPI.Util
{
    public static class Utility
    {
        public static List<Game> gameList = new List<Game>();
        public static int tempVar = 0; //Will save temporal values

        public static String generateRandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[36];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public static bool alreadyExist(string name)
        {
            bool exist = false;
            for (int i = 0; i < gameList.Count; i++)
            {
                if (gameList.ElementAt(i).name == name)
                {
                    exist = true;
                }
            }
            return exist;
        }

        public static bool gameExists(string gameId)
        {
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


        public static bool inGameUser(string playerName)
        {
            bool exist = false;
            List<string> actualPlayers = gameList.ElementAt(tempVar).players;
            for (int i = 0; i < actualPlayers.Count; i++)
            {
                if (actualPlayers[i] == playerName)
                {
                    exist = true;
                }
            }
            return exist;
        }

        public static bool playersInGame(Group group, int gamePosition)
        {
            bool exist = true;
            int count = 0;

            if (gameList.ElementAt(gamePosition).rounds != null)
            {
                foreach (var x in group.group)
                {
                    if (!gameList.ElementAt(gamePosition).players.Contains(x))
                    {
                        count++;
                    }
                }
                if (count>=1)
                {
                    exist = false;
                }
            }
            return exist;
        }

        public static bool groupExists(Group group, int gamePosition, int roundPosition)
        {
            bool exist = false;

            if (gameList.ElementAt(gamePosition).rounds != null)
            {
                foreach (var x in gameList.ElementAt(gamePosition).rounds.ElementAt(roundPosition - 1).group) {

                    for (int i = 0; i < group.group.Count; i++)
                    {
                        if (x.name == group.group.ElementAt(i))
                        {
                            exist = true;
                        }
                    }
                    
                }
            }
            return exist;
        }

        public static bool roundLeader(string name, int gameNumber) {

            bool leader = false;

            if (gameList.ElementAt(gameNumber).rounds.ElementAt(gameList.ElementAt(gameNumber).rounds.Count-1).leader == name)
            {
                leader = true;
            }

            return leader;
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

        public static List<string> setPsychos(List<string> players)
        {

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

        public static string getRoundLeader(List<string> players)
        {
            string leader = "";

            var random = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                leader = players.ElementAt(random.Next(players.Count));
            }

            return leader;
        }

        public static int getRoundGroup(int round, int players)
        {
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

        public static List<Proposal> setProposalFormat(Group group)
        {
            List<Proposal> proposalList = new List<Proposal>();
            Proposal proposal = new Proposal();

            for (int i = 0; i < group.group.Count; i++)
            {
                proposal.name = group.group.ElementAt(i);
                proposalList.Insert(proposalList.Count, proposal);
                proposal = new Proposal();
            }
            return proposalList;
        }

        public static void setPsychoMode(string name, Psycho psycho, int gameNumber, int roundNumber)
        {
            string namePlayer = "";

            for (int i = 0; i < gameList.ElementAt(gameNumber).rounds.ElementAt(roundNumber - 1).group.Count; i++)
            {
                namePlayer = gameList.ElementAt(gameNumber).rounds.ElementAt(roundNumber - 1).group.ElementAt(i).name;

                if (name == namePlayer)
                {
                    gameList.ElementAt(gameNumber).rounds.ElementAt(roundNumber - 1).group.ElementAt(i).psycho = psycho.psycho;
                }
            }

        }
        

        public static bool getRoundWinner(int gameNumber, int roundNumber)
        {
            bool psychoWins = false;
            int count = 0;

            foreach (var i in gameList.ElementAt(gameNumber).rounds.ElementAt(roundNumber-1).group)
            {
                for (int j = 1; j < gameList.ElementAt(gameNumber).rounds.ElementAt(roundNumber - 1).group.Count; j++)
                {
                    if (gameList.ElementAt(gameNumber).psychos.Contains(i.name) && i.psycho == true && 
                        !gameList.ElementAt(gameNumber).psychos.Contains(gameList.ElementAt(gameNumber).rounds.ElementAt(roundNumber - 1).group.ElementAt(j).name))
                    {
                        count++;
                    }
                }    
            }
            if (count >= 1)
            {
                psychoWins = true;
            }
            return psychoWins;
        }

        public static string getWinner(int gameNumber, int roundNumber) {
            string winner = "";

            if (getRoundWinner(gameNumber, roundNumber))
            {
                winner = "psychos";
            }
            else
            {
                winner = "ejemplares";
            }
            return winner;
        }

        public static bool getPsychoWinsQuantity(List<bool> psychoWins)
        {
            bool wins = false;
            int count = 0;
            foreach (bool w in psychoWins)
            {
                if (w == true)
                {
                    count++;
                }
            }
            if (count==3)
            {
                wins = true;
            }

            return wins;
        }

        public static bool inGroupList(string name, int gameposition)
        {
            bool exist = false;

            foreach (var element in gameList.ElementAt(gameposition).rounds.ElementAt(gameList.ElementAt(gameposition).rounds.Count-1).group) 
            {
                if (element.name == name)
                {
                    exist = true;
                }
            }

            return exist;
        }

        public static bool verifyData(Psycho pyscho)
        {
            bool correct = false;

            if (pyscho.psycho == true || pyscho.psycho == false)
            {
                correct = true;
            }

            return correct;
        }
    }

}
