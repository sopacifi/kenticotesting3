using System.Web.Mvc;

using Kentico.Forms.Mvc;
using Kentico.Forms.Web.Mvc;

using DancingGoat.Controllers.FormSections;

[assembly: RegisterFormSection("DancingGoat.TwoColumnSection", typeof(TwoColumnFormSectionController), "{$dancinggoatmvc.formsection.twocolumn.name$}", Description = "{$dancinggoatmvc.formsection.twocolumn.description$}", IconClass = "icon-l-cols-2")]

namespace DancingGoat.Controllers.FormSections
{
    [FormSectionExceptionFilter]
    public class TwoColumnFormSectionController : Controller
    {
        // GET: TwoColumnSection
        public ActionResult Index()
        {
            return PartialView("FormSections/_TwoColumnSection");
        }
    }
}