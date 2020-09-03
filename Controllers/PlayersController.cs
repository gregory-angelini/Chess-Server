using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ChessAPI.Models;

namespace ChessAPI.Controllers
{
    public class PlayersController : ApiController
    {
        private ModelChessDB db = new ModelChessDB();

        // POST: api/Players
        public PlayerInfo PosPlayer(Player player)
        {
            Logic logic = new Logic();
            return logic.GetPlayer(player);
        }


        /*
        // GET: api/Players/5
        [ResponseType(typeof(Player))]
        public IHttpActionResult GetPlayer(int id)
        {
            Logic logic = new Logic();
            Player player = logic.GetPlayer(id);

            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }
        */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlayerExists(int id)
        {
            return db.Players.Count(e => e.ID == id) > 0;
        }
    }
}