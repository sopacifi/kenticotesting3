using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Models.PageTemplates
{
    public class ArticleViewModel
    {
        public DocumentAttachment Teaser { get; set; }


        public string Title { get; set; }


        public DateTime PublicationDate { get; set; }


        public string Text { get; set; }


        public IEnumerable<RelatedArticleViewModel> RelatedArticles { get; set; }


        public static ArticleViewModel GetViewModel(Article article)
        {
            return new ArticleViewModel
            {
                PublicationDate = article.PublicationDate,
                RelatedArticles = article.Fields.RelatedArticles.OfType<Article>().Select((relatedArticle) => RelatedArticleViewModel.GetViewModel(relatedArticle, true)),
                Teaser = article.Fields.Teaser,
                Text = article.Fields.Text,
                Title = article.Fields.Title
            };
        }
    }
}