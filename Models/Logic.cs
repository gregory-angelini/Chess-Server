using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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

        public GameInfo GetGame()
        {
            bool createOrJoin = false;
            string opponentName = "";

            // enumerates all games with "play" status and returns only a game with the minimum ID 
            Game game = db
                .Games
                .Where(g => g.Status == "wait")
                .OrderBy(g => g.ID)
                .FirstOrDefault();

            if (game == null)
            {
                int ID = -1;// TODO
                game = CreateGame(ID);
                createOrJoin = true;
            }
            else
            {
                game.Status = "play";
                //game.Black_ID = ID;// TODO
                Player player = FindPlayer((int)game.White_ID);
                if (player != null) 
                    opponentName = player.Name;

                db.Games.Add(game);
                db.SaveChanges();
            }

            GameInfo gameInfo = new GameInfo();

            gameInfo.gameID = game.ID;
            gameInfo.FEN = game.FEN;
            gameInfo.opponentName = createOrJoin == false ? opponentName : "";

            // the creator always plays white
            gameInfo.isYourMove = createOrJoin;
            gameInfo.yourColor = createOrJoin ? "white" : "black";
            
            return gameInfo;
        }

        Player FindPlayer(int ID)
        {
            // enumerates all players and returns only the player with the same ID
            Player player = db
                .Players
                .Where(g => g.ID == ID).FirstOrDefault();
            return player;
        }

        Game CreateGame(int playerID)
        {
            Game game = new Game();

            Chess chess = new Chess();
            game.FEN = chess.fen; 
            game.Status = "wait";
            game.White_ID = playerID;

            db.Games.Add(game);
            db.SaveChanges();

            return game;
        }

        Game GetGame(int id)
        {
            return db.Games.Find(id);
        }
        /*
        public Player GetPlayer(int id)
        {
            return db.Players.Find(id);
        }*/

        public PlayerInfo GetPlayer(Player define)
        {
            // enumerates all players and returns only the player with the same GUID
            Player player = db
                .Players
                .Where(g => g.GUID == define.GUID).FirstOrDefault();

            if (player == null)
                player = CreatePlayer(define);

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.Name = player.Name;

            return playerInfo;
        }

        Player CreatePlayer(Player define)
        {
            Player player = new Player();

            player.GUID = define.GUID;
            player.Name = define.Name;
                 
            db.Players.Add(player);
            db.SaveChanges();

            return player;
        }

        public GameState GetMove(int id, string move)
        {
            GameState gameState = new GameState();

            Game game = GetGame(id);
            if (game == null) 
                return gameState;

            if (game.Status != "play") // we process only running games
                return gameState;

            Chess chess = new Chess(game.FEN);
            Chess nextChess = chess.Move(move);

            if (nextChess.fen == game.FEN) 
                return gameState;

            UpdateGame(game, nextChess);

            db.Entry(game).State = System.Data.Entity.EntityState.Modified;// we commit updates to database
            db.SaveChanges();

            gameState.gameID = game.ID;
            gameState.FEN = game.FEN;
            gameState.status = game.Status;
            gameState.lastMove = move;
            //gameState.offer = "";// TODO
            gameState.result = game.Result;
            gameState.winnerColor = GetWinnerColor(game);

            return gameState;
        }

        void UpdateGame(Game game, Chess chess)
        {
            game.FEN = chess.fen;

            game.Result = GetResult(chess);

            if (game.Result.Length > 0)
            {
                game.Status = "completed";

                if (game.Result == "checkmate")
                {
                    switch (chess.GetCurrentPlayerColor())
                    {
                        case Color.white:
                            game.Winner_ID = game.White_ID;
                            break;

                        case Color.black:
                            game.Winner_ID = game.Black_ID;
                            break;
                    }
                }
            }
        }

        string GetResult(Chess chess)
        {
            if (chess.IsCheckmate())
            {
                return "checkmate";
            }
            if(chess.IsStalemate())
            {
                return "stalemate";
            }
            return "";
        }

        string GetWinnerColor(Game game)
        {
            if (game.Winner_ID == game.White_ID)
                return "white";
            if (game.Winner_ID == game.Black_ID)
                return "black";
            return "";
        }
    }
}