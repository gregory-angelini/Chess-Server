namespace ChessAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Player
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string GUID { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}
