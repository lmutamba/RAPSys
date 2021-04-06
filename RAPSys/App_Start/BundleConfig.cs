using System.Web;
using System.Web.Optimization;

namespace RAPSys
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/notify.js",
                        "~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/jquery.pjax.js",
                      "~/Scripts/util.js",
                      "~/Scripts/jquery.slimscroll.js",
                      "~/Scripts/widgster.js",
                      "~/Scripts/pace.js",
                      "~/Scripts/settings.js",
                      "~/Scripts/app.js",
                      "~/Scripts/scripts.js",
                      "~/Scripts/umd/popper.min.js",
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/fileinput.min.js",
                      "~/Scripts/theme.js",
                      "~/Scripts/jquery-confirm.min.js",
                      "~/Scripts/datatables.min.js",
                      "~/Scripts/select2.min.js",
                      "~/Scripts/jq-wizard.js"));

            bundles.Add(new StyleBundle("~/bundles/Content/cssFiles").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/fileinput.min.css",
                      "~/Content/bootstrap-select.min.css",
                      "~/Content/datatables.min.css",
                      "~/Content/application.min.css",
                      "~/Content/jquery-confirm.min.css",
                      "~/Content/select2.min.css",
                      "~/Content/site.css").Include("~/Content/css/all.css", new CssRewriteUrlTransform()).Include("~/Content/css/fontawesome.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/bundles/Content/otherJS").Include(
                      "~/Scripts/bootstrap-select.min.js",
                      "~/Scripts/form-elements.js"));
        }
    }
}
