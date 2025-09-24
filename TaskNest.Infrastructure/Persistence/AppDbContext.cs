namespace TaskNest.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<BoardColumn> BoardColumns { get; set; } = null!;
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<BoardUser> BoardUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly ignore any shadow properties that might conflict
            modelBuilder.Entity<Board>()
                .Ignore(b => b.ApplicationUserId);

            // -----------------------
            // Board entity
            // -----------------------
            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(b => b.Description)
                      .HasMaxLength(500);

                // A Board belongs to a User (explicitly use ApplicationUserId)
                entity.HasOne(b => b.User)
                      .WithMany(u => u.Boards)
                      .HasForeignKey(b => b.ApplicationUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // -----------------------
            // BoardColumn entity
            // -----------------------
            modelBuilder.Entity<BoardColumn>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                // Column belongs to a Board
                entity.HasOne(c => c.Board)
                      .WithMany(b => b.BoardColumns)
                      .HasForeignKey(c => c.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -----------------------
            // TaskItem entity
            // -----------------------
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(300);
                entity.Property(t => t.Description)
                      .HasMaxLength(1000);

                // Task belongs to a Board
                entity.HasOne(t => t.Board)
                      .WithMany(b => b.Tasks)
                      .HasForeignKey(t => t.BoardId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Task belongs to a Column (optional)
                entity.HasOne(t => t.Column)
                      .WithMany(c => c.Tasks)
                      .HasForeignKey(t => t.ColumnId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // -----------------------
            // BoardUser entity (many-to-many between Board and ApplicationUser)
            // -----------------------
            modelBuilder.Entity<BoardUser>(entity =>
            {
                entity.HasKey(bu => bu.Id);
                entity.HasOne(bu => bu.Board)
                      .WithMany(b => b.BoardUsers)
                      .HasForeignKey(bu => bu.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(bu => bu.ApplicationUser)
                      .WithMany(u => u.BoardUsers)
                      .HasForeignKey(bu => bu.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}