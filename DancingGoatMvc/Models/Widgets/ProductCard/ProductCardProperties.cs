using System.Collections.Generic;

using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// Product card widget properties.
    /// </summary>
    public class ProductCardProperties : IWidgetProperties
    {
        /// <summary>
        /// Selected product.
        /// </summary>
        [EditingComponent(PageSelector.IDENTIFIER, Label = "{$DancingGoatMVC.Widget.ProductCard.SelectedProduct$}", Order = 1)]
        public IList<PageSelectorItem> SelectedProducts { get; set; }
    }
}