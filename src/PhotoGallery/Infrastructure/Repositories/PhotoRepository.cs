using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure.Repositories
{
    public class PhotoRepository : EntityBaseRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(PhotoGalleryContext cont) : base(cont)
        {
        }
    }
}
