namespace ChessAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelChessDB : DbContext
    {
        public ModelChessDB()
            : base("name=ModelChessDB")
        {
        }

        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Move> Moves { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .Property(e => e.FEN)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Game>()
                .Property(e => e.Result)
                .IsUnicode(false);

            modelBuilder.Entity<Move>()
                .Property(e => e.FEN)
                .IsFixedLength();

            modelBuilder.Entity<Move>()
                .Property(e => e.FenMove)
                .IsUnicode(false);

            modelBuilder.Entity<Move>()
                .Property(e => e.Result)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.GUID)
                .IsUnicode(false);

            modelBuilder.Entity<Player>()
                .Property(e => e.Name)
                .IsFixedLength();
        }
    }
}
