using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Core;
using PhotoGallery.Infrastructure.Repositories;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using PhotoGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Controllers
{
    [Route("api/[controller]")]
    public class PhotosController : Controller
    {
        private IPhotoRepository photoRepo;

        private ILoggingRepository loggingRepo;

        public PhotosController(IPhotoRepository photoRepo, ILoggingRepository loggingRepo)
        {
            this.photoRepo = photoRepo;
            this.loggingRepo = loggingRepo;
        }

        [HttpGet("{page:int=0}/{pageSize=12}")]
        public PaginationSet<PhotoViewModel> Get(int? page, int? pageSize)
        {
            PaginationSet<PhotoViewModel> pagedSet = null;

            try
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;

                List<Photo> photos = null;
                int totalPhotos = new int();

                photos = photoRepo.AllIncluding(p => p.Album).OrderBy(p => p.Id).Skip(currentPage * currentPageSize)
                                    .Take(currentPageSize).ToList();
                totalPhotos = photoRepo.GetAll().Count();

                IEnumerable<PhotoViewModel> photosVM = AutoMapper.Mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoViewModel>>(photos);
                pagedSet = new PaginationSet<PhotoViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalPhotos,
                    TotalPages = (int)Math.Ceiling((decimal)totalPhotos / currentPageSize),
                    Items = photosVM
                };
            }
            catch (Exception ex)
            {
                this.loggingRepo.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();
            }

            return pagedSet;
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            IActionResult result = new ObjectResult(false);
            GenericResult removeResult = null;

            try
            {
                Photo photo2Remove = this.photoRepo.GetSingle(id);
                this.photoRepo.Delete(photo2Remove);
                this.photoRepo.Commit();

                removeResult = new GenericResult
                {
                    Succeeded = true,
                    Message = "Photo removed"
                };
            }
            catch (Exception ex)
            {
                removeResult = new GenericResult()
                {
                    Succeeded = false,
                    Message = ex.Message
                };

                this.loggingRepo.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();
            }

            result = new ObjectResult(removeResult);
            return result;
        }
    }
}
