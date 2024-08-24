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
        private Timer _timer = new Timer(1000);
        private const int _refresh = 60;
        public MainWindow()
        {
            InitializeComponent();
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
            WpfPlot.Visibility = Visibility.Collapsed;
            var loginWindow = new LoginWindow { Owner = this };

            var result = false;
            var loginResult = loginWindow.ShowDialog();

            if (loginResult != null && loginResult.Value == true)
            {
                result = (bool)loginResult;
                ViewModel.SetChurchProps();
                RefreshChart();

                var interval = 0;

                _timer.Elapsed += (sender, args) =>
                {
                    interval++;
                    if (interval >= _refresh)
                    {
                        interval = 0;
                        _timer.Stop();
                        _timer.Enabled = false;
                        Dispatcher.Invoke(() => { RefreshChart(); });
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
            return staffPersonLoginWindow.ViewModel.DialogResult;

        }

        public void CloseWindow()
        {
            Close();
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
            ShowWindow(new AdvancedFindWindow());
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
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.Staff,
                    });
                }

                if (AppGlobals.LookupContext.Groups.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit Security _Groups...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
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
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.Members,
                    });
                }

                if (AppGlobals.LookupContext.Members.HasSpecialRight((int)MemberSpecialRights.AllowViewGiving))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit Member _Giving...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
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
                        Command = ViewModel.ShowMaintenanceWindowCommand,
                        CommandParameter = AppGlobals.LookupContext.Funds,
                    });
                }

                if (AppGlobals.LookupContext.Funds.HasRight(RightTypes.AllowView))
                {
                    menuItem.Items.Add(new MenuItem()
                    {
                        Header = "Add/Edit _Budget Items...",
                        Command = ViewModel.ShowMaintenanceWindowCommand,
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

        public void ShowMaintenanceWindow(TableDefinitionBase tableDefinition)
        {
            LookupControlsGlobals.WindowRegistry.ShowDbMaintenanceWindow(tableDefinition, WPFControlsGlobals.ActiveWindow);
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

        public void RefreshChart()
        {
            WpfPlot.Plot.Clear();
            WpfPlot.Plot.Style(dataBackground: Color.Transparent, figureBackground: Color.Transparent);

            var fundPeriodTotals = GetChartData();
            var totalIncomeMonthsList = new List<double>();
            var totalExpenseMonthsList = new List<double>();
            var xAxisLabelsList = new List<string>();

            foreach (var fundPeriodTotal in fundPeriodTotals)
            {
                totalIncomeMonthsList.Add(fundPeriodTotal.TotalIncome);
                totalExpenseMonthsList.Add(fundPeriodTotal.TotalExpenses);
                var xAxisLabel = fundPeriodTotal.Date.ToString("MMMM\nyyyy");
                xAxisLabelsList.Add(xAxisLabel);
            }

            double[] xs = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
            double[] ys = totalIncomeMonthsList.ToArray();
            double[] ys1 = totalExpenseMonthsList.ToArray();
            string[] xAxisLabels = xAxisLabelsList.ToArray();
            
            var incomeLine = WpfPlot.Plot.AddScatter(xs, ys, Color.Blue, 3F);
            incomeLine.Label = "Income";
            var expenseLine = WpfPlot.Plot.AddScatter(xs, ys1, Color.Red, 3F);
            expenseLine.Label = "Expenses";
            WpfPlot.Plot.XAxis.TickDensity(0.01D);
            //WpfPlot.Plot.XAxis.AutomaticTickPositions(xs, xAxisLabels);
            WpfPlot.Plot.XAxis.ManualTickPositions(xs, xAxisLabels);
            WpfPlot.Plot.XLabel("Month");
            WpfPlot.Plot.YLabel("Amount");

            var legend = WpfPlot.Plot.Legend(true, Alignment.UpperRight);
            legend.FontSize = 20;
            legend.FontBold = true;
            WpfPlot.Plot.Title("Income vs. Expenses");
            WpfPlot.Refresh();

            var context = SystemGlobals.DataRepository.GetDataContext();

            if (context.GetTable<FundPeriodTotals>().Any())
            {
                WpfPlot.Visibility = Visibility.Visible;
            }
        }

        private List<FundPeriodTotals> GetChartData()
        {
            var resultTotals = new List<FundPeriodTotals>();
            var monthsLeft = 13;

            var currentDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(-1);
            var context = SystemGlobals.DataRepository.GetDataContext();
            var periodsTable = context.GetTable<FundPeriodTotals>();

            while (monthsLeft > 0)
            {
                resultTotals.Add(GetTotalsForDate(currentDate, periodsTable));
                monthsLeft--;
                if (currentDate.Month == 12 && monthsLeft > 0)
                {
                    currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                }
                else if (monthsLeft > 0)
                {
                    currentDate = currentDate.AddMonths(1);
                }
            }
            return resultTotals;
        }

        private FundPeriodTotals GetTotalsForDate(DateTime date, IQueryable<FundPeriodTotals> periodsTable)
        {
            var result = new FundPeriodTotals();
            date = date.AddMonths(1).AddDays(-1);
            var periods = periodsTable
                .Where(p => p.Date == date
                            && p.PeriodType == (int)PeriodTypes.MonthEnding);
            result.Date = date;
            result.TotalIncome = periods.Sum(p => p.TotalIncome);
            result.TotalExpenses = periods.Sum(p => p.TotalExpenses);
            return result;
        }

        public void ShutDownApp()
        {
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }
    }
}