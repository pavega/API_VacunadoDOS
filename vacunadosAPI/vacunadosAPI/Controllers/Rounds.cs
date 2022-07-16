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
        public async Task<ActionResult<Game>> gameId([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string? password) {
            ErrorMessage error = new ErrorMessage();
            int? temp = null;
            for (int i = 0; i < Utility.gameList.Count; i++) {

                if (Utility.gameList.ElementAt(i).gameId == gameId /*&& Utility.gameList.ElementAt(i).owner == name*/) {

                    if (Utility.gameList.ElementAt(i).password != "" && Utility.gameList.ElementAt(i).password != password)
                    {
                        this.HttpContext.Response.StatusCode = 401;
                        error.error = "Credentials does not match";
                        return Json(error);
                    }
                    temp = i;
                    return Utility.gameList[i];
                }

            }
            if (Utility.gameExists(gameId))
            {
                if (temp != null)
                {
                    return Utility.gameList[temp.Value];
                }
                else {
                    this.HttpContext.Response.StatusCode = 403;
                    error.error = "This player is not part of the indicated game";
                    return Json(error);
                }
            }
            else {
                this.HttpContext.Response.StatusCode = 404;
                error.error = "Invalid game Id";
                return Json(error);
            }        
        }


        [HttpPut("{gameId}/join")]
        [SwaggerOperation(Summary = "Add player to game",
                          Description = "Add player to an arbitrary game")]
        public async Task<ActionResult<Message>> addPlayer([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string? password)
        {
            Message message = new Message();
            ErrorMessage error = new ErrorMessage();

            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId)
                {
                    if (Utility.gameList.ElementAt(i).password != "" && Utility.gameList.ElementAt(i).password != password)
                    {
                        this.HttpContext.Response.StatusCode = 401;
                        error.error = "Credentials does not match";
                        return Json(error);

                    }
                    if (!Utility.inGameUser(i, name)) {

                        if (Utility.gameList.ElementAt(i).players.Count < 10 && Utility.gameList.ElementAt(i).status == "lobby")
                        {                       
                            
                            int length = Utility.gameList.ElementAt(i).players.Count;
                            if (length < 10)
                            {
                                Utility.gameList.ElementAt(i).players.Insert(length, name);
                            }
                            message.message = "Operation Successfull";
                            return message;
                        }
                        else
                        {
                            this.HttpContext.Response.StatusCode = 406;
                            error.error = "Game has already started or is full";
                            return Json(error);
                        }
                    }
                    else
                    {
                        this.HttpContext.Response.StatusCode = 409;
                        error.error = "You are already part of this game";
                        return Json(error);
                    }
                }
            }

            if (Utility.gameExists(gameId))
            {            
                this.HttpContext.Response.StatusCode = 403;
                error.error = "This player is not part of the indicated game";
                return Json(error);
               
            }
            else
            {
                this.HttpContext.Response.StatusCode = 404;
                error.error = "Invalid game Id";
                return Json(error);
            }
        }



        //Name refears to owner and its password
        [HttpHead("{gameId}/start")]
        [SwaggerOperation(Summary = "Starts the game",
                          Description = "If you are the game owners, it will start the game")]
        public async Task<ActionResult<Message>> startGame([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string? password)
        {
            Message message = new Message();
            ErrorMessage error = new ErrorMessage();

            int length;
            Round round = new Round();
            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId)
                {
                    if (Utility.gameList.ElementAt(i).password != "" && Utility.gameList.ElementAt(i).password != password)
                    {
                        this.HttpContext.Response.StatusCode = 401;
                        error.error = "Credentials does not match";
                        return Json(error);
                    }

                    if (Utility.gameList.ElementAt(i).owner == name)
                    {
                        if (Utility.gameList.ElementAt(i).players.Count > 4 && Utility.gameList.ElementAt(i).status == "lobby")
                        {
                            Utility.gameList.ElementAt(i).status = "leader";
                            Utility.gameList.ElementAt(i).psychos = Utility.setPsychos(Utility.gameList.ElementAt(i).players);
                            length = Utility.gameList.ElementAt(i).rounds.Count;
                            round.id = length;
                            round.leader = Utility.getRoundLeader(Utility.gameList.ElementAt(i).players);
                            round.group = new List<Proposal>();
                            Utility.gameList.ElementAt(i).rounds.Insert(length, round);
                            message.message = "Game has started";
                            return message;
                        }
                        else 
                        {
                            if (Utility.gameList.ElementAt(i).status != "lobby")
                            {
                                this.HttpContext.Response.StatusCode = 406;
                                error.error = "Game already started";
                                return Json(error);

                            }
                            this.HttpContext.Response.StatusCode = 406;
                            error.error = "Not enough players. Invite more to join";
                            return Json(error);
                        }                     
                    }
                    else 
                    {
                        this.HttpContext.Response.StatusCode = 401;
                        error.error = "You are not the game's owner";
                        return Json(error);
                    }  
                }
            }
            if (Utility.gameExists(gameId))
            {
                this.HttpContext.Response.StatusCode = 403;
                error.error = "This player is not part of the indicated game";
                return Json(error);
            }
            else
            {
                this.HttpContext.Response.StatusCode = 404;
                error.error = "Invalid game Id";
                return Json(error);
            }
        }



        [HttpPost("{gameId}/group")]
        [SwaggerOperation(Summary = "Group proposal",
                          Description = "The round leader can propose a group for the current round")]
        public async Task<ActionResult<Message>> createGroup([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string? password, [FromBody] Group group)
        {
            Message message = new Message();
            ErrorMessage error = new ErrorMessage();
            Round round = new Round();

            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId)
                {
                    if (Utility.gameList.ElementAt(i).password != "" && Utility.gameList.ElementAt(i).password != password)
                    {
                        this.HttpContext.Response.StatusCode = 401;
                        error.error = "Credentials does not match";
                        return Json(error);

                    }
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

                                    message.message = "Group was added to the ongoing round";
                                    return message;
                                }
                                else
                                {
                                    this.HttpContext.Response.StatusCode = 406;
                                    error.error = "Game is not in the groups stage or provided group has invalid parameters (size/players)";
                                    return Json(error);
                                }
                            }
                            this.HttpContext.Response.StatusCode = 409;
                            error.error = "There is already a group added for this round";
                            return Json(error);
                        }
                        else
                        {
                            this.HttpContext.Response.StatusCode = 406;
                            error.error = "Not all players belong to this game";
                            return Json(error);
                        }
                    }
                    else {
                        this.HttpContext.Response.StatusCode = 403;
                        error.error = "You are not the leader for this round";
                        return Json(error);
                    }                                 
                }
            }
            this.HttpContext.Response.StatusCode = 406;
            error.error = "Invalid game Id";
            return Json(error);
        }


        [HttpPost("{gameId}/go")]
        [SwaggerOperation(Summary = "Go into round",
                          Description = "If the part of the group, a player can go. Psychos can activate the pysho mode")]
        public async Task<ActionResult<Message>> goIntoRound([FromRoute] string gameId, [FromHeader] string name, [FromHeader] string? password, [FromBody] Psycho psychoMode)
        {
            Message message = new Message();
            ErrorMessage error = new ErrorMessage();
            Round round = new Round();
            int length;
            for (int i = 0; i < Utility.gameList.Count; i++)
            {
                if (Utility.gameList.ElementAt(i).gameId == gameId)
                {
                    if (Utility.gameList.ElementAt(i).password != "" && Utility.gameList.ElementAt(i).password != password)
                    {
                        this.HttpContext.Response.StatusCode = 500;
                        error.error = "Credentials does not match";
                        return Json(error);

                    }

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
                                Utility.gameList.ElementAt(i).rounds.ElementAt(Utility.gameList.ElementAt(i).rounds.Count-1).winner = Utility.getWinner(i, Utility.gameList.ElementAt(i).rounds.Count);

                                if (Utility.getPsychoWinsQuantity(Utility.gameList.ElementAt(i).psychoWin) == null)
                                {
                                    Utility.gameList.ElementAt(i).status = "leader";
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
                            message.message = "Operation successful";
                            return message;
                        }
                        else {
                            this.HttpContext.Response.StatusCode = 406;
                            error.error = "Data provided is invalid";
                            return Json(error);
                        }                       
                    }
                    else 
                    {
                        if (!Utility.inGameUser(i, name))
                        {
                            this.HttpContext.Response.StatusCode = 403;
                            error.error = "This player is not part of the indicated game";
                            return Json(error);
                        }
                        this.HttpContext.Response.StatusCode = 401;
                        error.error = "You are not part of the round group list";
                        return Json(error);
                    }
                    
                }
            }
            this.HttpContext.Response.StatusCode = 404;
            error.error = "Invalid game Id";
            return Json(error);
        }
    }
}
