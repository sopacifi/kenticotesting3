using System;
using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Models.PageTemplates
{
    public class ArticleWithSideBarViewModel
    {
        public DocumentAttachment Teaser { get; set; }


        public string Title { get; set; }


        public DateTime PublicationDate { get; set; }


        public string Text { get; set; }


        public IEnumerable<RelatedArticleViewModel> RelatedArticles { get; set; }


        public ArticleSidebarLocationEnum SidebarLocation { get; set; }


        public string ArticleWidth { get; set; }


        public static ArticleWithSideBarViewModel GetViewModel(Article article, ArticleWithSideBarProperties templateProperties)
        {
            return new ArticleWithSideBarViewModel
            {
                Teaser = article.Fields.Teaser,
                PublicationDate = article.PublicationDate,
                RelatedArticles = article.Fields.RelatedArticles.OfType<Article>().Select((relatedArticle) => RelatedArticleViewModel.GetViewModel(relatedArticle, false)),
                Text = article.Fields.Text,
                Title = article.Fields.Title,
                SidebarLocation = (ArticleSidebarLocationEnum)Enum.Parse(typeof(ArticleSidebarLocationEnum), templateProperties.SidebarLocation, true),
                ArticleWidth = templateProperties.ArticleWidth,
            };
        }
    }
}