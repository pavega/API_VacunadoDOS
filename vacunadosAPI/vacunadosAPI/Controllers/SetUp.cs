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
        public async Task<ActionResult<List<Game>>> game(string? filter, string? filterValue) {

            if (filter == null || filter == "")
            {
                return Utility.gameList.ToList();
                //if (Utility.gameList.Count == 0)
                //{
                //    return null;
                //}
                //else {
                //    return Utility.gameList.ToList();
                //}
            }

            return null;
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
            if (Utility.alreadyExist(gameRequest.name))
            {
                this.HttpContext.Response.StatusCode = 404;
                error.error = "There is already a game called with this name";
                return Json(error);
            }
            else {
                Utility.gameList.Add(newGame);
                return newGame;
            }
          
        }
    }
}
