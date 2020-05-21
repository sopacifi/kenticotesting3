using System.Collections.Generic;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.PageBuilder.Web.Mvc;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// Card widget properties.
    /// </summary>
    public class CardWidgetProperties : IWidgetProperties
    {
        /// <summary>
        /// Guid of an image to be displayed.
        /// </summary>
        public IList<MediaFilesSelectorItem> Image { get; set; }


        /// <summary>
        /// Text to be displayed.
        /// </summary>
        public string Text { get; set; }
    }
}