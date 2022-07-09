using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using vacunadosAPI.Models;
using vacunadosAPI.Util;

namespace vacunadosAPI.Controllers
{
    [Route("/game/")]

    [ApiController]
    public class Rounds : Controller
    {

        //ERROR 400: NO SE PUDO RESOLVER EL ENDPOINT PORQUE LA CONTRASEÑA ESTÁ MAL
        //ERROR 404: EL ID DEL JUEGO NO ES VALIDO (NO EXISTE)
        //ERROR 403: EL JUEGO EXISTE, PERO EL JUGADOR NO ES PARTE DEL MISMO
        [HttpGet("{gameId}")]
        [SwaggerOperation(Summary = "Extract an arbitrary game",
                          Description = "Extract information for an arbitrary game. Please notice this is the same as using gameId filters on /")]
        public async Task<ActionResult<Game>> gameId([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password) {
            for (int i = 0; i < Utility.gameList.Count; i++) {
                if (Utility.gameList.ElementAt(i).gameId == gameId && Utility.gameList.ElementAt(i).name == name && Utility.gameList.ElementAt(i).password == password) {
                    return Utility.gameList[i];
                }
            }
            if (Utility.gameExists(gameId))
            {
                if (!Utility.inGameUser(name))
                {
                    return StatusCode(403, "This player is not part of the indicated game");
                }
                else {
                    return StatusCode(400, "Credentials does not match");
                }
            }
            else {
                return StatusCode(404, "Invalid game Id");
            }        
        }


        [HttpPut("{gameId}/join")]
        [SwaggerOperation(Summary = "Add player to game",
                          Description = "Add player to an arbitrary game")]
        public async Task<ActionResult<string>> addPlayer([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password)
        {
            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId && Utility.gameList.ElementAt(i).password == password)
                {

                    if (!Utility.inGameUser(name)) {

                        if (Utility.gameList.ElementAt(i).players.Count < 10 && Utility.gameList.ElementAt(i).status == "Lobby")
                        {                       
                            string message = "Operation Successfull";
                            int length = Utility.gameList.ElementAt(i).players.Count;
                            if (length < 10)
                            {
                                Utility.gameList.ElementAt(i).players.Insert(length, name);
                            }
                            return message;
                        }
                        else
                        {
                            return StatusCode(406, "Game has already started or is full");
                        }
                    }
                    else
                    {
                        return StatusCode(409, "You are already part of this game");
                    }
                }
            }

            if (Utility.gameExists(gameId))
            {
                if (!Utility.inGameUser(name))
                {
                    return StatusCode(403, "This player is not part of the indicated game");
                }
                else
                {
                    return StatusCode(400, "Credentials does not match");
                }
            }
            else
            {
                return StatusCode(404, "Invalid game Id");
            }
        }



        //Name refears to owner and its password
        [HttpHead("{gameId}/start")]
        [SwaggerOperation(Summary = "Starts the game",
                          Description = "If you are the game owners, it will start the game")]
        public async Task<ActionResult<Game>> startGame([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password)
        {
            int length;
            Round round = new Round();
            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId && Utility.gameList.ElementAt(i).password == password)
                {
                    if (Utility.gameList.ElementAt(i).owner == name)
                    {
                        if (Utility.gameList.ElementAt(i).players.Count > 4 && Utility.gameList.ElementAt(i).status == "Lobby")
                        {
                            Utility.gameList.ElementAt(i).status = "Leader";
                            Utility.gameList.ElementAt(i).psychos = Utility.setPsychos(Utility.gameList.ElementAt(i).players);
                            length = Utility.gameList.ElementAt(i).rounds.Count;
                            round.id = length;
                            round.leader = Utility.getRoundLeader(Utility.gameList.ElementAt(i).players);
                            round.group = new List<Proposal>();
                            Utility.gameList.ElementAt(i).rounds.Insert(length, round);
                            return StatusCode(200, "Game has started");
                        }
                        else 
                        {
                            if (Utility.gameList.ElementAt(i).status != "Lobby")
                            {
                                return StatusCode(406, "Game already started");

                            }
                            return StatusCode(406, "Not enough players. Invite more to join");
                        }                     
                    }
                    else 
                    {
                        return StatusCode(401, "You are not the game's owner");
                    }  
                }
            }
            if (Utility.gameExists(gameId))
            {
                if (!Utility.inGameUser(name))
                {
                    return StatusCode(403, "This player is not part of the indicated game");
                }
                else
                {
                    return StatusCode(401, "Credentials does not match");
                }
            }
            else
            {
                return StatusCode(404, "Invalid game Id");
            }
        }



        [HttpPost("{gameId}/group")]
        [SwaggerOperation(Summary = "Group proposal",
                          Description = "The round leader can propose a group for the current round")]
        public async Task<ActionResult<Game>> createGroup([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password, [FromBody] Group group)
        {
            Round round = new Round();

            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId && Utility.gameList.ElementAt(i).password == password)
                {
                    if (Utility.roundLeader(name, i))
                    {
                        Utility.gameList.ElementAt(i).status = "rounds";

                        if (Utility.playersInGame(group, i))
                        {
                            if (!Utility.groupExists(group, i, Utility.gameList.ElementAt(i).rounds.Count) && Utility.gameList.ElementAt(i).status == "rounds")
                            {
                                if (Utility.getRoundGroup(Utility.gameList.ElementAt(i).rounds.Count, Utility.gameList.ElementAt(i).players.Count()) == group.group.Count)
                                {
                                    Utility.gameList.ElementAt(i).rounds.ElementAt(Utility.gameList.ElementAt(i).rounds.Count - 1).group = Utility.setProposalFormat(group);

                                    return StatusCode(200, "Group was added to the ongoing round");
                                }
                                else
                                {
                                    return StatusCode(406, "Game is not in the groups stage or provided group has invalid parameters (size/players)");
                                }
                            }
                            return StatusCode(409, "There is already a group added for this round");
                        }
                        else
                        {
                            return StatusCode(406, "Not all players belong to this game");
                        }
                    }
                    else {
                        return StatusCode(403, "You are not the leader for this round");
                    }                                 
                }
            }
            if (Utility.gameExists(gameId))
            {
                if (!Utility.inGameUser(name))
                {
                    return StatusCode(403, "This player is not part of the indicated game");
                }
                else
                {
                    return StatusCode(401, "Credentials does not match");
                }
            }
            else
            {
                return StatusCode(406, "Invalid game Id");
            }
        }


        [HttpPost("{gameId}/go")]
        [SwaggerOperation(Summary = "Go into round",
                          Description = "If the part of the group, a player can go. Psychos can activate the pysho mode")]
        public async Task<ActionResult<string>> goIntoRound([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password, [FromBody] Psycho psychoMode)
        {
            Round round = new Round();
            int length;
            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId && Utility.gameList.ElementAt(i).password == password)
                {
                    if (Utility.inGroupList(name, i))
                    {
                        if (Utility.verifyData(psychoMode))
                        {
                            Utility.setPsychoMode(name, psychoMode, i, Utility.gameList.ElementAt(i).rounds.Count);

                            bool execute = true;
                            foreach (var roundTemp in Utility.gameList.ElementAt(i).rounds.Last().group)
                            {
                                if (roundTemp.psycho == null)
                                {
                                    execute = false;
                                }
                            }
                            if (execute)
                            {
                                Utility.gameList.ElementAt(i).psychoWin.Insert(Utility.gameList.ElementAt(i).psychoWin.Count, Utility.getRoundWinner(i, Utility.gameList.ElementAt(i).rounds.Count));

                                if (!Utility.getPsychoWinsQuantity(Utility.gameList.ElementAt(i).psychoWin))
                                {
                                    Utility.gameList.ElementAt(i).status = "Leader";
                                    length = Utility.gameList.ElementAt(i).rounds.Count;
                                    round.id = length;
                                    round.leader = Utility.getRoundLeader(Utility.gameList.ElementAt(i).players);
                                    round.group = new List<Proposal>();
                                    Utility.gameList.ElementAt(i).rounds.Insert(length, round);
                                }
                                else
                                {
                                    Utility.gameList.ElementAt(i).status = "ended";
                                }
                            }
                            return StatusCode(200, "Operation successful");
                        }
                        else {
                            return StatusCode(406, "Data provided is invalid");
                        }                       
                    }
                    else 
                    {
                        if (!Utility.inGameUser(name))
                        {
                            return StatusCode(403, "This player is not part of the indicated game");
                        }

                        return StatusCode(401, "You are not part of the round group list");
                    }
                    
                }
            }
            if (Utility.gameExists(gameId))
            {
                if (!Utility.inGameUser(name))
                {
                    return StatusCode(403, "This player is not part of the indicated game");
                }
                else
                {
                    return StatusCode(500, "Incorrect authentication");
                }
            }
            else
            {
                return StatusCode(404, "Invalid game Id");
            }
        }

    }
}
