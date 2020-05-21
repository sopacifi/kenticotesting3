using System;
using System.Linq;
using System.Web.Mvc;

using Kentico.PageBuilder.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;
using DancingGoat.Infrastructure;

using CMS.DocumentEngine.Types.DancingGoatMvc;

[assembly: RegisterWidget("DancingGoat.HomePage.CafeCardWidget", typeof(CafeCardWidgetController), "{$dancinggoatmvc.widget.cafecard.name$}", Description = "{$dancinggoatmvc.widget.cafecard.description$}", IconClass = "icon-cup")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for cafe card widgets.
    /// </summary>
    public class CafeCardWidgetController : WidgetController<CafeCardProperties>
    {
        private readonly ICafeRepository mCafeRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        /// <summary>
        /// Creates an instance of <see cref="CafeCardWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Repository for retrieving cafes.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        public CafeCardWidgetController(ICafeRepository repository, IOutputCacheDependencies outputCacheDependencies)
        {
            mCafeRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        public ActionResult Index()
        {
            var selectedPage = GetProperties().SelectedCafes?.FirstOrDefault();
            var cafe = (selectedPage != null) ? mCafeRepository.GetCafeByGuid(selectedPage.NodeGuid) : null;

            mOutputCacheDependencies.AddDependencyOnPage<Cafe>(cafe?.DocumentID ?? 0);

            return PartialView("Widgets/_CafeCardWidget", CafeCardViewModel.GetViewModel(cafe));
        }
    }
}