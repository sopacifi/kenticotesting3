using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// Cafe card widget properties.
    /// </summary>
    public class CafeCardProperties : IWidgetProperties
    {
        /// <summary>
        /// Selected cafes.
        /// </summary>
        [EditingComponent(PageSelector.IDENTIFIER, Label = "{$DancingGoatMVC.Widget.CafeCard.SelectedCafe$}", Order = 1)]
        [EditingComponentProperty(nameof(PageSelectorProperties.RootPath), "/Cafes")]
        [Required]
        public IList<PageSelectorItem> SelectedCafes { get; set; }
    }
}