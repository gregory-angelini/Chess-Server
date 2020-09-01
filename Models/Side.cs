namespace ChessAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Side
    {
        public int ID { get; set; }

        public int Player_ID { get; set; }

        public int Game_ID { get; set; }

        [Required]
        [StringLength(10)]
        public string Color { get; set; }
    }
}
