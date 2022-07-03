using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using vacunadosAPI.Models;
using vacunadosAPI.Util;

namespace vacunadosAPI.Controllers
{
    [Route("/Rounds/")]

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
        public async Task<ActionResult<String>> addPlayer([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password)
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
                        Utility.gameList.ElementAt(i).status = "Leader";
                        Utility.gameList.ElementAt(i).psychos = Utility.setPsychos(Utility.gameList.ElementAt(i).players);                  
                        length =  Utility.gameList.ElementAt(i).rounds.Count;
                        round.id = length;
                        round.leader = Utility.getRoundLeader(Utility.gameList.ElementAt(i).players);
                        round.group = new List<string>();
                        Utility.gameList.ElementAt(i).rounds.Insert(length, round);
                        return StatusCode(200, "Game has started");
                    }
                    return StatusCode(401, "You are not the game's owner");
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



        [HttpPost("{gameId}/group")]
        [SwaggerOperation(Summary = "Group proposal",
                          Description = "The round leader can propose a group for the current round")]
        public async Task<ActionResult<Game>> createGroup([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string password, [FromBody] List<string> group)
        {
            Round round = new Round();
            int length;
            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId && Utility.gameList.ElementAt(i).password == password)
                {
                    if (!Utility.groupExists(group, i))
                    {
                        for (int j = 0; j < Utility.gameList.ElementAt(i).rounds.Count; j++)
                        {
                            if (Utility.getRoundGroup(Utility.gameList.ElementAt(i).rounds.Count, Utility.gameList.ElementAt(i).players.Count()) == group.Count)
                            {
                                length = Utility.gameList.ElementAt(i).rounds.Count;
                                round = Utility.gameList.ElementAt(i).rounds.ElementAt(length-1); 
                                round.group = group;
                                return StatusCode(200, "Group was added to the ongoing round");
                            }
                            else
                            {
                                return StatusCode(406, "Game is not in the groups stage or provided group has invalid parameters (size/players)");
                            }
                        }
                                             
                    }
                    return StatusCode(409, "There is already a group added for this round");
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

    }
}
