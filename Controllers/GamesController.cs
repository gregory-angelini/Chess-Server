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
    public class GamesController : ApiController
    {
        ModelChessDB db = new ModelChessDB();

        // POST: api/Games
        public GameInfo PostGame(RequestedGame r)
        {
            Logic logic = new Logic();
            return logic.GetGame(r);
        }


        // GET: api/Games/5
        public GameState GetGame(int id)
        {
            Logic logic = new Logic();
            return logic.GetGame(id);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.ID == id) > 0;
        }
    }
}