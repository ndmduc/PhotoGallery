using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhotoGallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure
{
    public class PhotoGalleryContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Error> Errors { get; set; }

        public PhotoGalleryContext(DbContextOptions opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }

            // Photos
            modelBuilder.Entity<Photo>().Property(p => p.Title).HasMaxLength(100);
            modelBuilder.Entity<Photo>().Property(p => p.AlbumId).IsRequired();

            // Album
            modelBuilder.Entity<Album>().Property(p => p.Title).HasMaxLength(100);
            modelBuilder.Entity<Album>().Property(p => p.Description).HasMaxLength(500);
            modelBuilder.Entity<Album>().HasMany(p => p.Photos).WithOne(p=>p.Album);

            // User
            modelBuilder.Entity<User>().Property(p => p.Username).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().Property(p => p.Email).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(p => p.HashedPassword).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<User>().Property(p => p.Salt).IsRequired().HasMaxLength(200);

            // UserRole
            modelBuilder.Entity<UserRole>().Property(ur => ur.UserId).IsRequired();
            modelBuilder.Entity<UserRole>().Property(ur => ur.RoleId).IsRequired();

            // Role
            modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired().HasMaxLength(50);
        }
    }
}
