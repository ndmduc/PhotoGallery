using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure.Repositories
{
    public class RoleRepository : EntityBaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(PhotoGalleryContext cont) : base(cont)
        {
        }
    }
}
