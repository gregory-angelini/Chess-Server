using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}