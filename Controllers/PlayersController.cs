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
        [ResponseType(typeof(PlayerInfo))]
        public IHttpActionResult PostPlayer(Player player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Logic logic = new Logic();
            PlayerInfo p = logic.GetPlayer(player);
            if (p == null)
                return Created("player", logic.CreatePlayer(player));
            return Ok(p);
        }

        // GET: api/Players/id/color
        [ResponseType(typeof(PlayerInfo))]
        public IHttpActionResult GetPlayer(int id, string color)
        {
            Logic logic = new Logic();
            PlayerInfo p = logic.GetPlayer(id, color);
            if (p == null)
                return NotFound();
            return Ok(p);
        }

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