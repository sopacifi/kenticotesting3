using System;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Models.PageTemplates
{
    public class RelatedArticleViewModel
    {
        public DocumentAttachment Teaser { get; set; }


        public string Title { get; set; }


        public DateTime PublicationDate { get; set; }


        public string Summary { get; set; }


        public Guid NodeGUID { get; set; }


        public string NodeAlias { get; set; }


        public bool HandleArticlePosition { get; set; }


        public static RelatedArticleViewModel GetViewModel(Article article, bool handleArticlePosition)
        {
            return new RelatedArticleViewModel
            {
                NodeAlias = article.NodeAlias,
                NodeGUID = article.NodeGUID,
                Summary = article.Fields.Summary,
                Teaser = article.Fields.Teaser,
                Title = article.Fields.Title,
                PublicationDate = article.PublicationDate,
                HandleArticlePosition = handleArticlePosition,
            };
        }
    }
}