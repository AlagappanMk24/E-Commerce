namespace E_Commerce.Infrastructure.Data.Context
{
    public partial class EcomDbContext(DbContextOptions<EcomDbContext> options) : DbContext(options)
    {
        public virtual DbSet<MenuItem> MenuItems { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.ToTable("MenuItems");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                     .HasMaxLength(100)
                     .IsRequired();

                entity.Property(e => e.Url)
                      .HasMaxLength(255);

                entity.Property(e => e.CssClass)
                      .HasMaxLength(50);

                entity.Property(e => e.ImageUrl)
                      .HasMaxLength(255);

                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.Children)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}