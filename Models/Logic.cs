using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Web;
using ChessAPI.Models;
using ChessCore;


namespace ChessAPI.Models
{
    public class Logic
    {
        ModelChessDB db = new ModelChessDB();

        public Game GetGame()
        {
            // enumerates all games with "play" status and returns only a game with the minimum ID 
            Game game = db
                .Games
                .Where(g => g.Status == "play")
                .OrderBy(g => g.ID)
                .FirstOrDefault();

            if (game == null)
                game = CreateNewGame();
            return game;
        }

        Game CreateNewGame()
        {
            Game game = new Game();

            Chess chess = new Chess();
            game.FEN = chess.fen; 
            game.Status = "play";

            db.Games.Add(game);
            db.SaveChanges();

            return game;
        }

        public Game GetGame(int id)
        {
            return db.Games.Find(id);
        }

        public Game MakeMove(int id, string move)
        {
            Game game = GetGame(id);
            if (game == null) 
                return game;

            if (game.Status != "play") // we process only running games
                return game;

            Chess chess = new Chess(game.FEN);
            Chess nextChess = chess.Move(move);

            if (nextChess.fen == game.FEN) 
                return game;

            game.FEN = nextChess.fen;

            if(nextChess.IsCheckmate() || nextChess.IsStalemate())
            {
                game.Status = "completed";
            }
            db.Entry(game).State = System.Data.Entity.EntityState.Modified;// we commit updates to database
            db.SaveChanges();

            return game;
        }
    }
}