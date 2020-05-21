using DancingGoat.Models.PageTemplates;
using DancingGoat.Models.Sections;
using DancingGoat.Models.Widgets;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

// Widgets
[assembly: RegisterWidget("DancingGoat.General.TextWidget", "{$dancinggoatmvc.widget.text.name$}", typeof(TextWidgetProperties), Description = "{$dancinggoatmvc.widget.text.description$}", IconClass = "icon-l-text")]
[assembly: RegisterWidget("DancingGoat.LandingPage.TestimonialWidget", "{$dancinggoatmvc.widget.testimonial.name$}", typeof(TestimonialWidgetProperties), Description = "{$dancinggoatmvc.widget.testimonial.description$}", IconClass = "icon-right-double-quotation-mark")]

// Sections
[assembly: RegisterSection("DancingGoat.SingleColumnSection", "{$dancinggoatmvc.section.singlecolumn.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.singlecolumn.description$}", IconClass = "icon-square")]
[assembly: RegisterSection("DancingGoat.TwoColumnSection", "{$dancinggoatmvc.section.twocolumn.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.twocolumn.description$}", IconClass = "icon-l-cols-2")]
[assembly: RegisterSection("DancingGoat.ThreeColumnSection", "{$dancinggoatmvc.section.threecolumn.name$}", typeof(ThemeSectionProperties), Description = "{$dancinggoatmvc.section.threecolumn.description$}", IconClass = "icon-l-cols-3")]

// Page templates
[assembly: RegisterPageTemplate("DancingGoat.LandingPageSingleColumn", "{$dancinggoatmvc.pagetemplate.landingpagesinglecolumn.name$}", typeof(LandingPageSingleColumnProperties), Description = "{$dancinggoatmvc.pagetemplate.landingpagesinglecolumn.description$}")]
