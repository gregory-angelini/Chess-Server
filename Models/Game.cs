namespace ChessAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Game
    {
        public int ID { get; set; }

        [Required]
        [StringLength(90)]
        public string FEN { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        public int White_ID { get; set; }

        public int Black_ID { get; set; }

        public int? Winner_ID { get; set; }

        [StringLength(10)]
        public string Result { get; set; }
    }
}
