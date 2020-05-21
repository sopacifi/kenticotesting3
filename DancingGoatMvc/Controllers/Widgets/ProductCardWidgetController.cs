using System;
using System.Linq;
using System.Web.Mvc;

using CMS.Ecommerce;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget("DancingGoat.LandingPage.ProductCardWidget", typeof(ProductCardWidgetController), "{$dancinggoatmvc.widget.productcard.name$}", Description = "{$dancinggoatmvc.widget.productcard.description$}", IconClass = "icon-box")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for product card widgets.
    /// </summary>
    public class ProductCardWidgetController : WidgetController<ProductCardProperties>
    {
        private readonly IProductRepository mProductRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        public ActionResult Index()
        {
            var selectedPage = GetProperties().SelectedProducts?.FirstOrDefault();

            var product = (selectedPage != null) ? mProductRepository.GetProduct(selectedPage.NodeGuid) : null;

            mOutputCacheDependencies.AddDependencyOnPage<SKUTreeNode>(product?.DocumentID ?? 0);

            return PartialView("Widgets/_ProductCardWidget", ProductCardViewModel.GetViewModel(product));
        }


        /// <summary>
        /// Creates an instance of <see cref="ProductCardWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Repository for retrieving products.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        public ProductCardWidgetController(IProductRepository repository, IOutputCacheDependencies outputCacheDependencies)
        {
            mProductRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }
    }
}