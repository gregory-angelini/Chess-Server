using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChessAPI.Models
{
    public class MoveInfo
    {
        public int playerID;
        public int gameID;
        public string fenMove;
        public bool drawOffer;
        public bool resignOffer;
    }
}