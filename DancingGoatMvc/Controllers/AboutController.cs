using System.Linq;
using System.Web;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using DancingGoat.Infrastructure;
using DancingGoat.Models.MediaGallery;
using DancingGoat.Repositories;

namespace DancingGoat.Controllers
{
    public class AboutController : Controller
    {
        private const string MEDIA_LIBRARY_NAME = "CoffeeGallery";

        private readonly IAboutUsRepository mAboutUsRepository;
        private readonly IMediaFileRepository mMediaFileRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        public AboutController(IAboutUsRepository aboutUsRepository, IMediaFileRepository mediaFileRepository, IOutputCacheDependencies outputCacheDependencies)
        {
            mAboutUsRepository = aboutUsRepository;
            mMediaFileRepository = mediaFileRepository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        // GET: About
        [OutputCache(CacheProfile = "Default")]
        public ActionResult Index()
        {
            var sideStories = mAboutUsRepository.GetSideStories();

            mOutputCacheDependencies.AddDependencyOnPages<AboutUsSection>();

            return View(sideStories);
        }


        [ChildActionOnly]
        public ActionResult MediaGallery()
        {
            var mediaLibary = mMediaFileRepository.GetByName(MEDIA_LIBRARY_NAME);

            if (mediaLibary == null)
            {
                throw new HttpException(404, "Media library not found.");
            }

            var mediaFiles = mMediaFileRepository.GetMediaFiles(MEDIA_LIBRARY_NAME);
            var mediaGallery = new MediaGalleryViewModel(mediaLibary.LibraryDisplayName);
            mediaGallery.MediaFiles = mediaFiles.Select(MediaFileViewModel.GetViewModel);

            return PartialView("_MediaGallery", mediaGallery);
        }
    }
}