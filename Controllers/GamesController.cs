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
        [ResponseType(typeof(GameInfo))]
        public IHttpActionResult PostGame(RequestedGame r)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Logic logic = new Logic();
             GameInfo g = logic.GetGame(r);      
            if(g == null)
                return Created("game", logic.CreateGame(r));
            return Ok(g);
        }


        // GET: api/Games/5
        [ResponseType(typeof(GameState))]
        public IHttpActionResult GetGame(int id)
        {
            Logic logic = new Logic();
            GameState g = logic.GetGame(id);
            if(g == null)
                return NotFound();
            return Ok(g);
        }

        /*
        // POST: api/Games1
        [ResponseType(typeof(Game))]
        public IHttpActionResult PostGame(Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Games.Add(game);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = game.ID }, game);
        }
        */

        /*
        // DELETE: api/Games1/5
        [ResponseType(typeof(Game))]
        public IHttpActionResult DeleteGame(int id)
        {
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            db.Games.Remove(game);
            db.SaveChanges();

            return Ok(game);
        }
        */

        /*
        // PUT: api/Games1/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGame(int id, Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != game.ID)
            {
                return BadRequest();
            }

            db.Entry(game).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
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

        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.ID == id) > 0;
        }
    }
}