using System.Windows;
using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.Library;

namespace RingSoft.ChurchLogix
{
    public class ChurchLogixAppStart : AppStart
    {
        public ChurchLogixAppStart(Application application) : base(application, new MainWindow())
        {
            AppGlobals.InitSettings();
        }

        protected override void CheckVersion()
        {
#if DEBUG
            var app = RingSoftAppGlobals.IsAppVersionOld();
            if (app != null)
            {
                RingSoftAppGlobals.UserVersion = app.VersionName;
            }

#else
            base.CheckVersion();
#endif
        }

        protected override bool DoProcess()
        {
            AppGlobals.AppSplashProgress += AppGlobals_AppSplashProgress;

            AppGlobals.Initialize();

            AppGlobals.AppSplashProgress -= AppGlobals_AppSplashProgress;

            return base.DoProcess();
        }

        private void AppGlobals_AppSplashProgress(object? sender, AppProgressArgs e)
        {
            var test = SplashWindow;
            SetProgress(e.ProgressText);
        }

    }
}