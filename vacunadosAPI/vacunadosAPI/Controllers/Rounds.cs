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
    }
}
