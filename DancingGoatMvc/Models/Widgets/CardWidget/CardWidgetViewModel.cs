using CMS.MediaLibrary;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// View model for Card widget.
    /// </summary>
    public class CardWidgetViewModel
    {
        /// <summary>
        /// Card background image.
        /// </summary>
        public MediaFileInfo Image { get; set; }


        /// <summary>
        /// Card text.
        /// </summary>
        public string Text { get; set; }
    }
}