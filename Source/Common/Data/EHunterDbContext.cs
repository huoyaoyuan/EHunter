using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Data
{
    public class EHunterDbContext : DbContext
    {
        public DbSet<ImageEntry> Images => Set<ImageEntry>();
        public DbSet<Post> Posts => Set<Post>();

        public DbSet<Gallery> Galleries => Set<Gallery>();
        public DbSet<PostGallery> PostGalleries => Set<PostGallery>();

        public DbSet<TagConvert> TagConverts => Set<TagConvert>();
        public DbSet<TagImply> TagImplies => Set<TagImply>();

        public EHunterDbContext(DbContextOptions<EHunterDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageEntry>()
                .GenerateId()
                .ToTable("Images");
            modelBuilder.Entity<Post>()
                .GenerateId()
                .ToTable("Posts");
            modelBuilder.Entity<Gallery>()
                .GenerateId()
                .ToTable("Galleries");

            modelBuilder.Entity<Post>()
                .HasIndex(x => new { x.Provider, x.Identifier })
                .IsUnique();

            modelBuilder.Entity<ImageEntry>()
                .HasMany(x => x.Tags)
                .WithOne()
                .HasForeignKey(x => x.ImageId);
            modelBuilder.Entity<ImageEntry>()
                .HasOne(x => x.Post)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ImageEntry>()
                .HasKey("Id")
                .IsClustered(false);
            modelBuilder.Entity<ImageEntry>()
                .HasIndex(x => new { x.PostId, x.PostOrderId })
                .IsUnique()
                .IsClustered();

            modelBuilder.Entity<ImageTag>()
                .HasKey(x => new { x.ImageId, x.TagScopeName, x.TagName });
            modelBuilder.Entity<ImageTag>()
                .ToTable("ImageTags");

            modelBuilder.Entity<Gallery>()
                .HasMany(x => x.Tags)
                .WithOne()
                .HasForeignKey(x => x.GalleryId);

            modelBuilder.Entity<GalleryTag>()
                .HasKey(x => new { x.GalleryId, x.TagScopeName, x.TagName });
            modelBuilder.Entity<GalleryTag>()
                .ToTable("GalleryTags");

            modelBuilder.Entity<PostGallery>()
                .HasOne(x => x.Post)
                .WithOne(x => x.Gallery!)
                .HasForeignKey<PostGallery>();

            modelBuilder.Entity<TagConvert>()
                .HasKey(x => new { x.TagScopeName, x.TagName });
            modelBuilder.Entity<TagConvert>()
                .ToTable("TagConverts");

            modelBuilder.Entity<TagImply>()
                .HasKey(x => new { x.TagScopeName, x.TagName, x.ImpliedTagScopeName, x.ImpliedTagName });
            modelBuilder.Entity<TagImply>()
                .ToTable("TagImplies");
        }

        public async IAsyncEnumerable<(string? tagScopeName, string tagName)> MapTag(string? tagScopeName, string tagName)
        {
            var convert = await Set<TagConvert>().FindAsync(tagScopeName, tagName).ConfigureAwait(false);
            if (convert != null)
                (tagScopeName, tagName) = (convert.ConvertedTagScopeName, convert.ConvertedTagName);

            yield return (tagScopeName, tagName);

            await foreach (var imply in Set<TagImply>()
                .Where(x => x.TagScopeName == tagScopeName && x.TagName == tagName)
                .AsAsyncEnumerable()
                .ConfigureAwait(false))
                yield return (imply.TagScopeName, imply.TagName);
        }
    }
}
