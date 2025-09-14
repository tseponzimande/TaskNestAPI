namespace TaskNest.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Board> Boards { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<BoardColumn> BoardColumns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(b => b.User)
                      .WithMany()
                      .HasForeignKey(b => b.ApplicationUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<BoardColumn>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.Board)
                      .WithMany(b => b.BoardColumns)
                      .HasForeignKey(c => c.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Position).IsRequired();

                entity.HasOne(t => t.Board)
                      .WithMany(b => b.Tasks)
                      .HasForeignKey(t => t.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Column)
                      .WithMany(c => c.Tasks)
                      .HasForeignKey(t => t.ColumnId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}