using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Infrastructure.Repositories
{
    public class LoggingRepository : EntityBaseRepository<Error>, ILoggingRepository
    {
        public LoggingRepository(PhotoGalleryContext cont) : base(cont)
        {
        }

        public override void Commit()
        {
            try
            {
                base.Commit();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
