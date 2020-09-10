using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http.Controllers;
using ChessAPI.Models;
using ChessCore;


namespace ChessAPI.Models
{
    public class Logic
    {
        ModelChessDB db = new ModelChessDB();


        public GameInfo GetGame(RequestedGame r)
        {
            /* matchmaking 
             * 1. enumerates all games with "wait" status and returns the first
             * 2. we're looking for a game with "white" or "black" empty slot
             * 3. we can reconnect to our game after a while */
            Game game = db
                .Games
                .OrderByDescending(g => g.ID)
                .Where(g =>
                       // new connect
                       (g.Status == "wait" && ((r.playerColor == "white" && g.White_ID == 0) ||
                                               (r.playerColor == "black" && g.Black_ID == 0)))
                        || // or reconnect
                       (g.Status == "play" && ((r.playerColor == "white" && g.White_ID == r.playerID) ||
                                               (r.playerColor == "black" && g.Black_ID == r.playerID))) )
                .FirstOrDefault();

            if (game == null)
                return null; 

            return JoinGame(game, r);
        }

        GameInfo JoinGame(Game game, RequestedGame r)
        {
            if (r.playerColor == "white")
            {
                game.White_ID = r.playerID;
            }

            if (r.playerColor == "black")
            {
                game.Black_ID = r.playerID;
            }
            game.Status = "play";

            db.Entry(game).State = System.Data.Entity.EntityState.Modified;// we commit updates to database
            db.SaveChanges();

            GameInfo gameInfo = new GameInfo();
            gameInfo.gameID = game.ID;
            gameInfo.FEN = game.FEN;
            return gameInfo;
        }

        public GameInfo CreateGame(RequestedGame r)
        {
            Game game = new Game();

            Chess chess = new Chess("r3k2r/1pppppp1/8/8/8/8/1PPPPPP1/R3K2R w KQkq - 0 1");
            game.FEN = chess.fen;
            game.Status = "wait";

            if (r.playerColor == "white")
            {
                game.White_ID = r.playerID;
            }

            if (r.playerColor == "black")
            {
                game.Black_ID = r.playerID;
            }

            db.Games.Add(game);
            db.SaveChanges();

            GameInfo gameInfo = new GameInfo();
            gameInfo.gameID = game.ID;
            gameInfo.FEN = game.FEN;
            return gameInfo;
        }

        public GameState GetGame(int id)
        {
            Game game = db.Games.Find(id);
            if (game == null)
                return null;

            //if (game.Status != "play") // we process only running games
            //    return null;

            Move move = GetLastMove(game.ID);
            string lastMove = move != null ? move.FenMove : "";
            string lastMoveColor = move != null ? GetPlayerColor(game, move.Player_ID) : "";

            GameState gameState = new GameState();
            gameState.gameID = game.ID;
            gameState.FEN = game.FEN;
            gameState.status = game.Status;
            gameState.lastMove = lastMove;
            gameState.lastMoveColor = lastMoveColor;
            gameState.result = game.Result; 
             
            return gameState;
        }

        Move GetLastMove(int gameID)
        {
            Move move = db
                .Moves
                .OrderByDescending(g => g.ID)
                .Where(g => g.Game_ID == gameID)
                .FirstOrDefault();
            return move;
        }

        public PlayerInfo GetPlayer(int gameID, string color)
        {
            Game game = db.Games.Find(gameID);
            if (game == null)
                return null;

            int playerID = GetPlayerByColor(game, color);

            Player player = db
               .Players
               .Where(g => g.ID == playerID).FirstOrDefault();

            if (player == null)
                return null;

            PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.playerName = player.Name;
            playerInfo.playerID = player.ID;

            return playerInfo;
        }

        int GetPlayerByColor(Game game, string color)
        {
            if (color == "white")
                return game.White_ID;

            if(color == "black")
                return game.Black_ID;
            return 0;
        }

        public PlayerInfo GetPlayer(Player define)
        {
            // enumerates all players and returns only the player with the same GUID
            Player player = db
                .Players
                .Where(g => g.GUID == define.GUID).FirstOrDefault();

            if (player == null)
                return null;

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerName = player.Name;
            playerInfo.playerID = player.ID;

            return playerInfo;
        }

        public PlayerInfo CreatePlayer(Player define)
        {
            Player player = new Player();

            player.GUID = define.GUID;
            player.Name = define.Name;
                 
            db.Players.Add(player);
            db.SaveChanges();

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerName = player.Name;
            playerInfo.playerID = player.ID;

            return playerInfo;
        }

        public GameState PostMove(MoveInfo move)
        {
            Game game = db.Games.Find(move.gameID);
            if (game == null) 
                return null;

            if (game.Status != "play") // we process only running games
                return null;

            // TODO: process 'draw' and 'resign' offers 

            Chess chess = new Chess(game.FEN);

            if(!IsAuthorized(chess.GetMoveColor().ToString(), 
                             game, 
                             move.playerID))
                return null; 

            Chess nextChess = chess.Move(move.fenMove);

            if (nextChess.fen == game.FEN) // move is illegal
                return null;

            string gameResult = GetGameResult(move, nextChess);

            UpdateGame(game, nextChess.fen, gameResult, move.playerID); 

            SaveMove(game.ID,  
                     move.playerID,
                     chess.fen,// game state before the move
                     move.fenMove,
                     gameResult);

            GameState gameState = new GameState();

            gameState.gameID = game.ID;
            gameState.FEN = game.FEN;
            gameState.status = game.Status;
            gameState.lastMove = move.fenMove;
            gameState.lastMoveColor = GetPlayerColor(game, move.playerID);
            gameState.result = game.Result;

            return gameState;
        }

        bool IsAuthorized(string moveColor, Game game, int playerID)
        {
            if ((moveColor == "white" && game.White_ID == playerID) ||
                (moveColor == "black" && game.Black_ID == playerID))
            {
                return true;
            }
            return false;
        }
       

        void SaveMove(int gameID, int playerID, string FEN, string fenMove, string result)
        {
            Move move = new Move();
            move.Game_ID = gameID;
            move.Player_ID = playerID;
            move.FEN = FEN;
            move.FenMove = fenMove;
            move.Result = result;

            db.Moves.Add(move);
            db.SaveChanges();
        }

        void UpdateGame(Game game, string FEN, string gameResult, int playerID)
        {
            game.FEN = FEN;
            
            if (gameResult.Length > 0)
            {
                game.Status = "completed";
            }
                
            if (gameResult == "checkmate") 
            {
                game.Winner_ID = playerID;
            }
            else if (gameResult == "resign")
            {
                game.Winner_ID = GetOpponentOf(game, playerID);
            }
            
            game.Result = gameResult;

            db.Entry(game).State = System.Data.Entity.EntityState.Modified;// we commit updates to database
            db.SaveChanges();
        }

        int GetOpponentOf(Game game, int playerID)
        {
            if (game.White_ID == playerID) 
                return game.Black_ID;
            else if (game.Black_ID == playerID)
                return game.White_ID;

            return 0;
        }

        string GetGameResult(MoveInfo move, Chess chess)
        {
            if (move.resignOffer)
                return "resign";

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

        string GetPlayerColor(Game game, int playerID)
        {
            if (game.White_ID == playerID)
                return "white";

            if (game.Black_ID == playerID)
                return "black";
            return "";
        }
    }
}