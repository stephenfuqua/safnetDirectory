using System.Web.Mvc;

namespace safnetDirectory.FullMvc
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
