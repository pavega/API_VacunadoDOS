using Microsoft.AspNetCore.Http;
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
            Game alanGame = new Game();
            alanGame.name = "AllanGame";
            Utility.gameList.Add(alanGame);
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
    }
}
