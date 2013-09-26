using System.Web;
using System.Web.Mvc;

namespace TechnicalInterview_FeedReader
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}