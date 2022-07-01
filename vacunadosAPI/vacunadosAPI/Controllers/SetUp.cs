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
                if (Utility.gameList.Count == 0)
                {
                    return null;
                }
                else {
                    return Utility.gameList.ToList();
                }
            }

            return null;
        }

        [SwaggerOperation(Summary = "Create new game",
                         Description = "Create a new game. A header 'name' should be used to indicate the game owner and identity to use forward")]
        [HttpPost("create")]
        public async Task<ActionResult<Game>> create(string owner, string name, string password) {
            String matchName = Utility.generateRandomString();
            Game newGame = new Game();
            newGame.name = name;
            newGame.owner = owner;
            newGame.password = password;
            newGame.gameId = matchName;
            newGame.status = "Lobby";
            if (Utility.alreadyExist(name))
            {
                return StatusCode(406, "There is already a game called with this name");
            }
            else {
                Utility.gameList.Add(newGame);
                return newGame;
            }
          
        }
    }
}
