namespace ChessAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Move
    {
        public int ID { get; set; }

        public int Game_ID { get; set; }

        public int Player_ID { get; set; }

        [Required]
        [StringLength(90)]
        public string FEN { get; set; }

        [Required]
        [StringLength(6)]
        public string FenMove { get; set; }

        [StringLength(10)]
        public string Result { get; set; }
    }
}
