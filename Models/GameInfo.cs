using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChessAPI.Models
{
    public class GameInfo
    {
        public int gameID;
        public string FEN;
        public string opponentName;
        public bool isYourMove;
        public string yourColor;// black, white
    }
}