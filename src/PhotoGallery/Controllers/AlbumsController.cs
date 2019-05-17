using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Core;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using PhotoGallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoGallery.Controllers
{
    [Route("api/[controller]")]
    public class AlbumsController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IAlbumRepository albumRepo;
        private readonly ILoggingRepository loggingRepo;

        public AlbumsController(IAuthorizationService authorizationService,
                             IAlbumRepository albumRepository,
                             ILoggingRepository loggingRepository)
        {
            this.authorizationService = authorizationService;
            this.albumRepo = albumRepository;
            this.loggingRepo = loggingRepository;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{page:int=0}/{pageSize=12}")]
        public async Task<IActionResult> Get(int? page, int? pageSize)
        {
            PaginationSet<AlbumViewModel> pagedSet = new PaginationSet<AlbumViewModel>();
            try
            {
                if (await this.authorizationService.AuthorizeAsync(User, "AdminOnly"))
                {
                    int currentPage = page.Value;
                    int currentPageSize = pageSize.Value;

                    List<Album> _album = null;
                    int _totalAlbums = new int();

                    _album = this.albumRepo.AllIncluding(a => a.Photos).OrderBy(a => a.Id).Skip(currentPage * currentPageSize)
                        .Take(currentPageSize).ToList();
                    _totalAlbums = this.albumRepo.GetAll().Count();

                    IEnumerable<AlbumViewModel> albumVM = AutoMapper.Mapper.Map<IEnumerable<Album>, IEnumerable<AlbumViewModel>>(_album);
                    pagedSet = new PaginationSet<AlbumViewModel>
                    {
                        Page = currentPage,
                        TotalCount = _totalAlbums,
                        TotalPages = (int)Math.Ceiling((decimal)_totalAlbums / currentPageSize),
                        Items = albumVM
                    };

                }
                else
                {
                    Infrastructure.Core.StatusCodeResult _codeResult = new Infrastructure.Core.StatusCodeResult(401);
                    return new ObjectResult(_codeResult);
                }
            }
            catch (Exception ex)
            {
                this.loggingRepo.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();
            }

            return new ObjectResult(pagedSet);
        }

        [Authorize(Policy ="AdminOnly")]
        [HttpGet("{id:int}/photos/{page:int=0}/{pageSize=12}")]
        public PaginationSet<PhotoViewModel> Get(int id, int? page, int? pageSize)
        {
            PaginationSet<PhotoViewModel> pagedSet = null;

            try
            {
                int currentPage = page.Value;
                int currentPageSize = pageSize.Value;
                List<Photo> photos = null;
                int totalPhotos = new int();

                Album album = this.albumRepo.GetSingle(a => a.Id == id, a => a.Photos);
                photos = album.Photos.OrderBy(p => p.Id).Skip(currentPage * currentPageSize).Take(currentPage).ToList();
                totalPhotos = album.Photos.Count();

                IEnumerable<PhotoViewModel> _photosVM = Mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoViewModel>>(photos);

                pagedSet = new PaginationSet<PhotoViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalPhotos,
                    TotalPages = (int)Math.Ceiling((decimal)totalPhotos / currentPageSize),
                    Items = _photosVM
                };
            }
            catch (Exception ex)
            {
                this.loggingRepo.Add(new Error() { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now });
                this.loggingRepo.Commit();
            }

            return pagedSet;
        }
    }
}
