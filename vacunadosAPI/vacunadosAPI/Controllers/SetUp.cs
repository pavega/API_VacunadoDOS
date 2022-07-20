using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using vacunadosAPI.Models;
using vacunadosAPI.Util;

namespace vacunadosAPI.Controllers
{

    [Route("/game/")]
    [ApiController]
    public class SetUp : Controller
    {

        [SwaggerOperation(Summary = "list all games", 
                          Description = "List all games. Query strings can be used to indicate filters to be applied on the server side.")]
        [HttpGet()]
        public async Task<ActionResult<List<GameHeader>>> game(string? filter, string? filterValue) {
            GameHeader game = new GameHeader();
            List<GameHeader> filteredGames = new List<GameHeader>();

            switch (filter){
                case "gameId":
                    for (int i = 0; i < Utility.gameList.Count; i++)
                    {
                        if (Utility.gameList.ElementAt<Game>(i).gameId == filterValue)
                        {
                            game.gameId = Utility.gameList.ElementAt<Game>(i).gameId;
                            game.name = Utility.gameList.ElementAt<Game>(i).name;
                            filteredGames.Add(game);
                        }
                    }
                break;
                case "owner":
                    for (int i = 0; i < Utility.gameList.Count; i++)
                    {
                        if (Utility.gameList.ElementAt<Game>(i).owner == filterValue)
                        {
                            game.gameId = Utility.gameList.ElementAt<Game>(i).gameId;
                            game.name = Utility.gameList.ElementAt<Game>(i).name;
                            filteredGames.Add(game);
                        }
                    }
                break;
                case "status":
                    for (int i = 0; i < Utility.gameList.Count; i++)
                    {
                        if (Utility.gameList.ElementAt<Game>(i).status == filterValue)
                        {
                            game.gameId = Utility.gameList.ElementAt<Game>(i).gameId;
                            game.name = Utility.gameList.ElementAt<Game>(i).name;
                            filteredGames.Add(game);
                        }
                    }
                break;
                case null:
                case "--":
                    filteredGames = Utility.gameHeaderList;
                break;
            }

            return filteredGames;

        }


        //ERROR 400: NO RECIBIÓ PARÁMETROS
        //ERROR 406: LA PARTIDA CON ESE NOMBRE EXISTE ACTUALMENTE
        [SwaggerOperation(Summary = "Create new game",
                         Description = "Create a new game. A header 'name' should be used to indicate the game owner and identity to use forward")]
        [HttpPost("create")]
        public async Task<ActionResult<Game>> create([FromHeader] string name, [FromBody]NewGameRequest gameRequest) {
            ErrorMessage error = new ErrorMessage();
            string matchName = Utility.generateRandomString();
            Game newGame = new Game();
            GameHeader gameHeader = new GameHeader();
            newGame.name = gameRequest.name;
            newGame.owner = name;
            if (gameRequest.password == "" || gameRequest.password == null)
            {
                newGame.password = "";
            }
            else
            {
                newGame.password = gameRequest.password;
            }
            newGame.gameId = matchName;
            newGame.players = new List<string>() { name };
            newGame.psychoWin = new List<bool>();
            newGame.psychos = new List<string>();
            newGame.status = "lobby";
            newGame.rounds = new List<Round> { };
            gameHeader.gameId = newGame.gameId;
            gameHeader.name = newGame.name;
       
            if (Utility.alreadyExist(gameRequest.name))
            {
                this.HttpContext.Response.StatusCode = 404;
                error.error = "There is already a game called with this name";
                return Json(error);
            }
            else {
                Utility.gameList.Add(newGame);
                Utility.gameHeaderList.Add(gameHeader);
                return newGame;
            }
          
        }
    }
}
