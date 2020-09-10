using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using ChessAPI.Models;

namespace ChessAPI.Controllers
{
    public class MovesController : CommonApiController
    {
        private ModelChessDB db = new ModelChessDB();


        // POST: api/Moves/5/Pe2e4
        [ResponseType(typeof(GameState))]
        public IHttpActionResult PostMove(MoveInfo move)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Logic logic = new Logic();
            GameState g = logic.PostMove(move);
            if (g == null)
                return NotFound();
            return Created("move", g);
        }

        

        /*
        // GET: api/Moves
        public IQueryable<Move> GetMoves()
        {
            return db.Moves;
        }

        // GET: api/Moves/5
        [ResponseType(typeof(Move))]
        public IHttpActionResult GetMove(int id)
        {
            Move move = db.Moves.Find(id);
            if (move == null)
            {
                return NotFound();
            }

            return Ok(move);
        }

        // PUT: api/Moves/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMove(int id, Move move)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != move.ID)
            {
                return BadRequest();
            }

            db.Entry(move).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoveExists(id))
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

        // POST: api/Moves
        [ResponseType(typeof(Move))]
        public IHttpActionResult PostMove(Move move)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Moves.Add(move);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = move.ID }, move);
        }

        // DELETE: api/Moves/5
        [ResponseType(typeof(Move))]
        public IHttpActionResult DeleteMove(int id)
        {
            Move move = db.Moves.Find(id);
            if (move == null)
            {
                return NotFound();
            }

            db.Moves.Remove(move);
            db.SaveChanges();

            return Ok(move);
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

        private bool MoveExists(int id)
        {
            return db.Moves.Count(e => e.ID == id) > 0;
        }
    }
}