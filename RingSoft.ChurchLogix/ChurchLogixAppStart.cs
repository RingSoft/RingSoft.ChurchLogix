using System.Windows;
using RingSoft.App.Controls;
using RingSoft.App.Library;
using RingSoft.ChurchLogix.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.ChurchLogix.FinancialManagement;
using RingSoft.ChurchLogix.Library;
using RingSoft.ChurchLogix.MemberManagement;
using RingSoft.ChurchLogix.StaffManagement;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;

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

            LookupControlsGlobals.WindowRegistry.RegisterWindow<StaffMaintenanceWindow, StaffPerson>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<GroupsMaintenanceWindow, Group>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<StaffMaintenanceWindow, StaffGroup>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<MemberGivingPeriodTotalsWindow, MemberPeriodGiving>();

            LookupControlsGlobals.WindowRegistry.RegisterWindow<MemberMaintenanceWindow, Member>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<MemberGivingMaintenanceWindow, MemberGiving>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<MemberGivingMaintenanceWindow, MemberGivingDetails>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<MemberGivingHistoryWindow, MemberGivingHistory>();

            LookupControlsGlobals.WindowRegistry.RegisterWindow<FundMaintenanceWindow, Fund>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<BudgetMaintenanceWindow, BudgetItem>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<BudgetActualMaintenanceWindow, BudgetActual>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<FundHistoryWindow, FundHistory>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<FundPeriodWindow, FundPeriodTotals>();
            LookupControlsGlobals.WindowRegistry.RegisterWindow<BudgetPeriodWindow, BudgetPeriodTotals>();

            LookupControlsGlobals.WindowRegistry.RegisterWindow<EventMaintenanceWindow, Event>();

            LookupControlsGlobals.WindowRegistry.RegisterWindow<SystemPreferencesWindow, SystemPreferences>();

            return base.DoProcess();
        }

        private void AppGlobals_AppSplashProgress(object? sender, AppProgressArgs e)
        {
            var test = SplashWindow;
            SetProgress(e.ProgressText);
        }

    }
}