using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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


        public GameInfo GetGame(RequestedGame rGame)
        {
            //bool createOrJoin = false;
            bool whiteSide = rGame.playerColor == "white" ? true : false;

            // enumerates all games with "play" status and returns only a game with the minimum ID 
            Game game = db
                .Games
                .Where(g => g.Status == "wait" && ((whiteSide && g.White_ID == 0) ||
                                                   (!whiteSide && g.Black_ID == 0))) 
                .OrderBy(g => g.ID)
                .FirstOrDefault();

            if (game == null)
            {
                game = CreateGame(rGame);
                //createOrJoin = true;
            }
            else
            {
                game = JoinGame(game, rGame);
            }

            GameInfo gameInfo = new GameInfo();
            gameInfo.gameID = game.ID;
            gameInfo.FEN = game.FEN;
            return gameInfo;
        }

        Game JoinGame(Game game, RequestedGame rGame)
        {
            if (rGame.playerColor == "white")
            {
                game.White_ID = rGame.playerID;
            }

            if (rGame.playerColor == "black")
            {
                game.Black_ID = rGame.playerID;
            }
            game.Status = "play";

            db.Entry(game).State = System.Data.Entity.EntityState.Modified;// we commit updates to database
            db.SaveChanges();

            return game;
        }

        Game CreateGame(RequestedGame rGame)
        {
            Game game = new Game();

            Chess chess = new Chess();
            game.FEN = chess.fen;
            game.Status = "wait";

            if (rGame.playerColor == "white")
            {
                game.White_ID = rGame.playerID;
            }

            if (rGame.playerColor == "black")
            {
                game.Black_ID = rGame.playerID;
            }

            db.Games.Add(game);
            db.SaveChanges();

            return game;
        }

        public GameState GetGame(int id)
        {
            GameState gameState = new GameState();

            Game game = db.Games.Find(id);
            if (game == null)
                return gameState;

            if (game.Status != "play") // we process only running games
                return gameState;

            gameState.gameID = game.ID;
            gameState.FEN = game.FEN;
            gameState.status = game.Status;
            gameState.lastMove = GetLastMove(game.ID);
            gameState.lastMoveColor = GetLastMoveColor(gameState.lastMove);
            //gameState.offer = "";// TODO
            gameState.result = game.Result;
            //gameState.winnerColor = GetWinnerColor(game);
             
            return gameState;
        }

        /*
        Player FindPlayer(int ID)
        {
            // enumerates all players and returns only the player with the same ID
            Player player = db
                .Players
                .Where(g => g.ID == ID).FirstOrDefault();
            return player;
        }*/

        string GetLastMoveColor(string fenMove)
        {
            if(!string.IsNullOrEmpty(fenMove))
            {
                if(char.IsUpper(fenMove[0]))
                {
                    return "white";
                }
                else return "black";
            }
            return "";
        }

        string GetLastMove(int gameID)
        {
            Move move = db
                .Moves
                .Where(g => g.Game_ID == gameID)
                .OrderByDescending(g => g.ID)
                .FirstOrDefault();

            if (move != null)
                return move.FenMove;

            return "";
        }

        public PlayerInfo GetPlayer(Player define)
        {
            // enumerates all players and returns only the player with the same GUID
            Player player = db
                .Players
                .Where(g => g.GUID == define.GUID).FirstOrDefault();

            if (player == null)
                player = CreatePlayer(define);

            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.playerName = player.Name;
            playerInfo.playerID = player.ID;

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

            Game game = db.Games.Find(id);
            if (game == null) 
                return gameState;

            if (game.Status != "play") // we process only running games
                return gameState;

            Chess chess = new Chess(game.FEN);
            Chess nextChess = chess.Move(move);

            if (nextChess.fen == game.FEN) 
                return gameState;

            SaveMove(game.ID, 
                     chess.GetCurrentPlayerColor() == Color.white ? (int)game.White_ID : (int)game.Black_ID,
                     move,
                     game.Result,
                     nextChess);

            UpdateGame(game, nextChess);

  
            gameState.gameID = game.ID;
            gameState.FEN = game.FEN;
            gameState.status = game.Status;
            gameState.lastMove = move;
            gameState.lastMoveColor = GetLastMoveColor(move);
            //gameState.offer = "";// TODO
            gameState.result = game.Result;
            //gameState.winnerColor = GetWinnerColor(game);

            return gameState;
        }
         
        void SaveMove(int gameID, int playerID, string fenMove, string result, Chess chess)
        {
            Move move = new Move();
            move.Game_ID = gameID;
            move.Player_ID = playerID;
            move.FEN = chess.fen;
            move.FenMove = fenMove;
            move.Result = result;

            db.Moves.Add(move);
            db.SaveChanges();
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

            db.Entry(game).State = System.Data.Entity.EntityState.Modified;// we commit updates to database
            db.SaveChanges();
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

        /*
        string GetWinnerColor(Game game)
        {
            if (game.Winner_ID == game.White_ID)
                return "white";
            if (game.Winner_ID == game.Black_ID)
                return "black";
            return "";
        }
        */
    }
}