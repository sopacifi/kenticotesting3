using System;
using System.Collections.Generic;
using System.Linq;

using CMS.MediaLibrary;
using CMS.SiteProvider;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of media files.
    /// </summary>
    public class KenticoMediaFileRepository : IMediaFileRepository
    {
        /// <summary>
        /// Returns instance of <see cref="MediaFileInfo"/> specified by library name.
        /// </summary>
        /// <param name="mediaLibraryName">Name of the media library.</param>
        public MediaLibraryInfo GetByName(string mediaLibraryName)
        {
            return MediaLibraryInfoProvider.GetMediaLibraryInfo(mediaLibraryName, SiteContext.CurrentSiteName);
        }


        /// <summary>
        /// Returns all media files in the media library.
        /// </summary>
        /// <param name="mediaLibraryName">Name of the media library.</param>
        public IEnumerable<MediaFileInfo> GetMediaFiles(string mediaLibraryName)
        {
            var mediaLibrary = GetByName(mediaLibraryName);

            if (mediaLibrary == null)
            {
                throw new ArgumentException("Media library not found.", nameof(mediaLibraryName));
            }

            return MediaFileInfoProvider.GetMediaFiles()
                .WhereEquals("FileLibraryID", mediaLibrary.LibraryID)
                .ToList();
        }


        /// <summary>
        /// Returns media file with given identifier and site name.
        /// </summary>
        /// <param name="fileIdentifier">Identifier of the media file.</param>
        /// <param name="siteName">Site name.</param>
        public MediaFileInfo GetMediaFile(Guid fileIdentifier, string siteName)
        {
            return MediaFileInfoProvider.GetMediaFileInfo(fileIdentifier, siteName);
        }
    }
}