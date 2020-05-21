using System;

using CMS.Helpers;
using CMS.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultItemModel
    {
        public string Title { get; set; }


        public string Content { get; set; }


        public DateTime Date { get; set; }


        public string ObjectType { get; set; }


        public string ImageUrl { get; set; }


        public string Url { get; set; }


        public SearchResultItemModel(SearchResultItem resultItem)
        {
            Title = resultItem.Title;
            Content = HTMLHelper.StripTags(resultItem.Content, false);
            Date = resultItem.Created;
            ImageUrl = resultItem.GetImageUrl();
            ObjectType = resultItem.Type;
        }
    }
}