using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Infrastructure;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace DancingGoat.Controllers
{
    public class LandingPageController : Controller
    {
        private readonly ILandingPageRepository mRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        public LandingPageController(ILandingPageRepository repository, IOutputCacheDependencies outputCacheDependencies)
        {
            mRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        // GET: LandingPage
        // [OutputCache(CacheProfile = "PageBuilder")]
        public ActionResult Index(string pageAlias)
        {
            var landingPage = mRepository.GetLandingPage(pageAlias);
            if (landingPage == null)
            {
                return HttpNotFound();
            }

            mOutputCacheDependencies.AddDependencyOnPage<LandingPage>(landingPage.DocumentID);

            return new TemplateResult(landingPage.DocumentID);
        }
    }
}