using System.ComponentModel;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library;
using RingSoft.ChurchLogix.Library.ViewModels;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DataEntryControls.WPF;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbLookup.Controls.WPF.AdvancedFind;
using RingSoft.DbLookup.ModelDefinition;
using ScottPlot;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Timers;
using Timer = System.Timers.Timer;
using RingSoft.App.Controls;
using RingSoft.App.Library;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainView, ICheckVersionView
    {
        public StatsnGraphsUserControl StatsUserControl { get; private set; }


        private Timer _timer = new Timer(1000);
        private const int _refresh = 60;
        public MainWindow()
        {
            InitializeComponent();

            LookupControlsGlobals.SetTabSwitcherWindow(this, TabControl);
            TabControl.SetDestionationAsFirstTab = false;

            SetupToolbar();
            ContentRendered += (sender, args) =>
            {
#if DEBUG
                ViewModel.Initialize(this);
#else
                try
                {
                    ViewModel.Initialize(this);
                }
                catch (Exception e)
                {
                    RingSoft.App.Library.RingSoftAppGlobals.HandleError(e);
                }
#endif
            };
        }

        public bool ChangeChurch()
        {
            _timer.Stop();
            _timer.Enabled = false;
            var loginWindow = new LoginWindow { Owner = this };

            var result = false;
            var loginResult = loginWindow.ShowDialog();

            if (loginResult != null && loginResult.Value == true)
            {
                result = (bool)loginResult;
                ViewModel.SetChurchProps();
                RefreshChart(true);

                var interval = 0;

                _timer.Elapsed += (sender, args) =>
                {
                    interval++;
                    if (interval >= _refresh)
                    {
                        interval = 0;
                        _timer.Stop();
                        _timer.Enabled = false;
                        Dispatcher.Invoke(() => { RefreshChart(true); });
                        _timer.Enabled = true;
                        _timer.Start();
                    }
                };
                _timer.Start();
                _timer.Enabled = true;
            }

            return result;

        }

        public bool LoginStaffPerson(int staffPersonId = 0)
        {
            var staffPersonLoginWindow = new StaffPersonLoginWindow(staffPersonId) { Owner = this };
            staffPersonLoginWindow.ShowDialog();
            if (staffPersonLoginWindow.ViewModel.DialogResult)
            {
                if (staffPersonId > 0)
                {
                    return true;
                }

                MakeMenu();
            }
            var result = staffPersonLoginWindow.ViewModel.DialogResult;
            if (!result)
            {
                CloseAllTabs();
            }
            return result;

        }

        public void CloseWindow()
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CloseAllTabs())
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        public void ShowWindow(System.Windows.Window window)
        {
            window.Owner = this;
            window.ShowInTaskbar = false;
            window.Closed += (sender, args) => Activate();
            window.Show();
        }


        public void ShowAdvancedFindWindow()
        {
            //ShowWindow(new AdvancedFindWindow());
            ShowMaintenanceTab(AppGlobals.LookupContext.AdvancedFinds);
        }

        private void ProcessButton(DbMaintenanceButton button, TableDefinitionBase tableDefinition)
        {
            if (tableDefinition.HasRight(RightTypes.AllowView))
            {
                button.Visibility = Visibility.Visible;
            }
            else
            {
                button.Visibility = Visibility.Collapsed;
            }
        }

        public void MakeMenu()
        {
            MainMenu.Items.Clear();

            SetupToolbar();

            var fileMenu = new MenuItem()
            {
                Header = "_File"
            };

            fileMenu.Items.Add(new MenuItem()
            {
                Header = $"_Change Church",
                Command = ViewModel.ChangeChurchCommand
            });

            fileMenu.Items.Add(new MenuItem()
            {
                Header = $"_Logout",
                Command = ViewModel.LogoutCommand
            });


            fileMenu.Items.Add(new MenuItem()
            {
                Header = $"E_xit",
                Command = ViewModel.ExitCommand
            });

            MainMenu.Items.Add(fileMenu);

            MakeStaffMenu();
            MakeMemberMenu();
            MakeFinancesMenu();
            MakeChurchLifeMenu();
            MakeSystemMenu();

            MainMenu.Items.Add(new MenuItem()
            {
                Header = "_Advanced Find",
                Command = ViewModel.AdvFindCommand
            });

            MainMenu.Items.Add(new MenuItem()
            {
                Header = "A_bout ChurchLogix...",
                Command = ViewModel.AboutCommand,
            });
            MainMenu.Items.Add(new MenuItem()
            {
                Header = "Upgrade _Version...",
                Command = ViewModel.UpgradeCommand,
            });

        }

        public void ClearMenu()
        {
            MainMenu.Items.Clear();
        }

        private void MakeStaffMenu()
        {
            var staffCategory =
                SystemGlobals.Rights.UserRights.Categories.FirstOrDefault(p =>
                    p.MenuCategoryId == (int)MenuCategories.StaffManagement);

            var items = staffCategory.Items.Where(p => p.TableDefinition.HasRight(RightTypes.AllowView));
            if (items.Any())
            {
                var menuItem = new MenuItem() { Header = "_Staff Management" };
                MainMenu.Items.Add(menuItem);

                if (AppGlobals.LookupContext.Staff.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Staff...",
                        Command = ViewModel.ShowMaintenanceTabCommand,
                        CommandParameter = AppGlobals.LookupContext.Staff,
                    });
                }

                if (AppGlobals.LookupContext.Groups.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit Security _Groups...",
                        Command = ViewModel.ShowMaintenanceTabCommand,
                        CommandParameter = AppGlobals.LookupContext.Groups,
                    });
                }

            }
        }

        private void MakeMemberMenu()
        {
            var memberCategory =
                SystemGlobals.Rights.UserRights.Categories.FirstOrDefault(p =>
                    p.MenuCategoryId == (int)MenuCategories.MemberManagement);

            var items = memberCategory.Items.Where(p => p.TableDefinition.HasRight(RightTypes.AllowView));
            if (items.Any())
            {
                var menuItem = new MenuItem() { Header = "M_ember Management" };
                MainMenu.Items.Add(menuItem);

                if (AppGlobals.LookupContext.Members.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Members...",
                        Command = ViewModel.ShowMaintenanceTabCommand,
                        CommandParameter = AppGlobals.LookupContext.Members,
                    });
                }

                if (AppGlobals.LookupContext.Members.HasSpecialRight((int)MemberSpecialRights.AllowViewGiving))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit Member _Giving...",
                        Command = ViewModel.ShowMaintenanceTabCommand,
                        CommandParameter = AppGlobals.LookupContext.MembersGiving,
                    });
                }

            }
        }

        private void MakeFinancesMenu()
        {
            var financialCategory =
                SystemGlobals.Rights.UserRights.Categories.FirstOrDefault(p =>
                    p.MenuCategoryId == (int)MenuCategories.FinancialManagement);

            var items = financialCategory.Items.Where(p => p.TableDefinition.HasRight(RightTypes.AllowView));
            if (items.Any())
            {
                var menuItem = new MenuItem() { Header = "F_inancial Management" };
                MainMenu.Items.Add(menuItem);

                if (AppGlobals.LookupContext.Funds.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Funds...",
                        Command = ViewModel.ShowMaintenanceTabCommand,
                        CommandParameter = AppGlobals.LookupContext.Funds,
                    });
                }

                if (AppGlobals.LookupContext.Funds.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Budget Items...",
                        Command = ViewModel.ShowMaintenanceTabCommand,
                        CommandParameter = AppGlobals.LookupContext.Budgets,
                    });
                }

                if (AppGlobals.LookupContext.BudgetActuals.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "_Enter Budget Costs...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.BudgetActuals,
                    });
                }
            }
        }

        private void MakeChurchLifeMenu()
        {
            var churchLifeCategory =
                SystemGlobals.Rights.UserRights.Categories.FirstOrDefault(p =>
                    p.MenuCategoryId == (int)MenuCategories.ChurchLife);

            var items = churchLifeCategory.Items.Where(p => p.TableDefinition.HasRight(RightTypes.AllowView));
            if (items.Any())
            {
                var menuItem = new MenuItem() { Header = "_Church Life" };
                MainMenu.Items.Add(menuItem);

                if (AppGlobals.LookupContext.Events.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Events...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.Events,
                    });
                }

                if (AppGlobals.LookupContext.SmallGroups.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Small Groups...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.SmallGroups,
                    });
                }

                if (AppGlobals.LookupContext.Roles.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit Small Group _Roles...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.Roles,
                    });
                }
            }
        }

        private void MakeSystemMenu()
        {
            var systemCategory =
                SystemGlobals.Rights.UserRights.Categories.FirstOrDefault(p =>
                    p.MenuCategoryId == (int)MenuCategories.System);

            var items = systemCategory.Items.Where(p => p.TableDefinition.HasRight(RightTypes.AllowView));
            if (items.Any())
            {
                var menuItem = new MenuItem() { Header = "S_ystem" };
                MainMenu.Items.Add(menuItem);

                if (AppGlobals.LookupContext.Funds.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _System Preferences...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.SystemPreferences,
                    });
                }
            }
        }

        private void SetupToolbar()
        {
            ChangeChurchButton.ToolTip.HeaderText = "Change Church (Alt + C)";
            ChangeChurchButton.ToolTip.DescriptionText =
                "Change to a different church.";

            LogoutButton.ToolTip.HeaderText = "Logout Current User (Alt + L)";
            LogoutButton.ToolTip.DescriptionText =
                "Log out of the current user and log into a different user.";

            if (AppGlobals.LookupContext == null)
            {
                StaffButton.Visibility = Visibility.Collapsed;
                MemberButton.Visibility = Visibility.Collapsed;
                FundButton.Visibility = Visibility.Collapsed;
                BudgetButton.Visibility = Visibility.Collapsed;
                EventButton.Visibility = Visibility.Collapsed;
                SmallGroupButton.Visibility = Visibility.Collapsed;
                AdvancedFindButton.Visibility = Visibility.Collapsed;
                return;
            }

            ProcessButton(StaffButton, AppGlobals.LookupContext.Staff);
            MemberButton.ToolTip.HeaderText = "Show the Staff Maintenance Window (Alt + F)";
            MemberButton.ToolTip.DescriptionText =
                "Add or edit Staff.";

            ProcessButton(MemberButton, AppGlobals.LookupContext.Members);
            MemberButton.ToolTip.HeaderText = "Show the Member Maintenance Window (Alt + M)";
            MemberButton.ToolTip.DescriptionText =
                "Add or edit Members.";

            ProcessButton(FundButton, AppGlobals.LookupContext.Funds);
            FundButton.ToolTip.HeaderText = "Show the Fund Maintenance Window (Alt + U)";
            FundButton.ToolTip.DescriptionText =
                "Add or edit Funds.";

            ProcessButton(BudgetButton, AppGlobals.LookupContext.Budgets);
            BudgetButton.ToolTip.HeaderText = "Show the Budget Maintenance Window (Alt + B)";
            BudgetButton.ToolTip.DescriptionText =
                "Add or edit Budgets.";

            ProcessButton(EventButton, AppGlobals.LookupContext.Events);
            EventButton.ToolTip.HeaderText = "Show the Event Maintenance Window (Alt + E)";
            EventButton.ToolTip.DescriptionText =
                "Add or edit Events.";

            ProcessButton(SmallGroupButton, AppGlobals.LookupContext.SmallGroups);
            SmallGroupButton.ToolTip.HeaderText = "Show the Small Group Maintenance Window (Alt + S)";
            SmallGroupButton.ToolTip.DescriptionText =
                "Add or edit Small Groups.";

            ProcessButton(AdvancedFindButton, AppGlobals.LookupContext.AdvancedFinds);
            AdvancedFindButton.ToolTip.HeaderText = "Advanced Find (Alt + A)";
            AdvancedFindButton.ToolTip.DescriptionText =
                "Search any table in the database for information you're looking for.";
        }

        public bool UpgradeVersion()
        {
            return AppStart.CheckVersion(this, true);
        }

        public void ShowAbout()
        {
            var splashWindow = new AppSplashWindow(false);
            splashWindow.Title = "About ChurchLogix";
            splashWindow.Owner = this;
            splashWindow.ShowInTaskbar = false;
            splashWindow.ShowDialog();
        }

        public void RefreshChart(bool selectTab)
        {
            if (StatsUserControl == null)
            {
                if (ViewModel.ChurchDataExists())
                {
                    StatsUserControl = new StatsnGraphsUserControl();
                    TabControl.ShowUserControl(
                        StatsUserControl
                        , "Statistics and Graphs"
                        , true
                        , selectTab);
                    StatsUserControl.RefreshChart();
                }
            }
            else
            {
                StatsUserControl.RefreshChart();
            }
        }

        public void ShowMaintenanceWindow(TableDefinitionBase tableDefinition)
        {
            LookupControlsGlobals.WindowRegistry.ShowDbMaintenanceWindow(tableDefinition, WPFControlsGlobals.ActiveWindow);
        }

        public bool CloseAllTabs()
        {
            var result = TabControl.CloseAllTabs();
            StatsUserControl = null;
            return result;
        }

        public void ShowMaintenanceTab(TableDefinitionBase tableDefinition)
        {
            TabControl.ShowTableControl(tableDefinition, false);
        }

        internal static void ProcessSendEmailLink(TextBlock sendEmailLink, string? emailAddress)
        {
            if (emailAddress.IsNullOrEmpty())
            {
                sendEmailLink.Visibility = Visibility.Collapsed;
            }
            else
            {
                sendEmailLink.Visibility = Visibility.Visible;
                sendEmailLink.Inlines.Clear();
                try
                {
                    var uri = new Uri($"mailto:{emailAddress}");
                    var hyperLink = new Hyperlink
                    {
                        NavigateUri = uri
                    };
                    hyperLink.Inlines.Add("Send Email");
                    hyperLink.RequestNavigate += (sender, args) =>
                    {
                        Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
                        args.Handled = true;
                    };
                    sendEmailLink.Inlines.Add(hyperLink);
                }
                catch (Exception e)
                {
                    sendEmailLink.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void ShutDownApp()
        {
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}