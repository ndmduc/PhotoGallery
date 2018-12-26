using PhotoGallery.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure
{
    public static class DbInitializer
    {
        private static PhotoGalleryContext context;

        public static void Initialize(IServiceProvider serviceProvider, string imagesPath)
        {
            context = (PhotoGalleryContext)serviceProvider.GetService(typeof(PhotoGalleryContext));

        }

        private static void InitializePhotoAlbums(string imagesPath)
        {
            if (!context.Albums.Any())
            {
                List<Album> albums = new List<Album>();
                var _album1 = context.Albums.Add(
                    new Album
                    {
                        DateCreated = DateTime.Now,
                        Title = "Album 1",
                        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
                    }).Entity;

                var _album2 = context.Albums.Add(
                    new Album
                    {
                        DateCreated = DateTime.Now,
                        Title = "Album 2",
                        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
                    }).Entity;

                var _album3 = context.Albums.Add(
                    new Album
                    {
                        DateCreated = DateTime.Now,
                        Title = "Album 3",
                        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
                    }).Entity;

                var _album4 = context.Albums.Add(
                    new Album
                    {
                        DateCreated = DateTime.Now,
                        Title = "Album 4",
                        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
                    }).Entity;

                albums.Add(_album1);
                albums.Add(_album2);
                albums.Add(_album3);
                albums.Add(_album4);

                string[] images = Directory.GetFiles(Path.Combine(imagesPath, "images"));
                Random rnd = new Random();

                foreach (var image in images)
                {
                    int selectedAlbum = rnd.Next(1, 4);
                    string fileName = Path.GetFileName(image);

                    context.Photos.Add(new Photo
                    {
                        Title = fileName,
                        DateUploaded = DateTime.Now,
                        Uri = fileName,
                        Album = albums.ElementAt(selectedAlbum)
                    });
                }

                context.SaveChanges();
            }
        }

        private static void InitializeUserRoles()
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new Role[] { new Role { Name = "Admin" } });
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Email = "ndminhduc@gmail.com",
                    Username = "duc",
                    HashedPassword = "9wsmLgYM5Gu4zA/BSpxK2GIBEWzqMPKs8wl2WDBzH/4=",
                    Salt = "GTtKxJA6xJuj3ifJtTXn9Q==",
                    IsLocked = false,
                    DateCreated = DateTime.Now
                });

                // create user admin for duc
                context.UserRoles.AddRange(new UserRole[] {
                    new UserRole {
                        RoleId = 1,
                        UserId=1}
                });

                context.SaveChanges();
            }
        }
    }
}
