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
        public PlayerInfo PostPlayer(Player player)
        {
            Logic logic = new Logic();
            return logic.GetPlayer(player);
        }

        // GET: api/Players/id/color
        public PlayerInfo GetPlayer(int id, string color)
        {
            Logic logic = new Logic();
            return logic.GetPlayer(id, color);
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