﻿using CMS.MediaLibrary;

namespace DancingGoat.Models.Widgets
{
    /// <summary>
    /// View model for Banner widget.
    /// </summary>
    public class BannerWidgetViewModel
    {
        /// <summary>
        /// Banner background image.
        /// </summary>
        public MediaFileInfo Image { get; set; }


        /// <summary>
        /// Banner text.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Gets or sets a title for a link defined by <see cref="LinkUrl"/>.
        /// </summary>
        public string LinkTitle { get; set; }


        /// <summary>
        /// Gets or sets URL to which the visitor is redirected after clicking on the <see cref="Text"/>.
        /// </summary>
        public string LinkUrl { get; set; }
    }
}