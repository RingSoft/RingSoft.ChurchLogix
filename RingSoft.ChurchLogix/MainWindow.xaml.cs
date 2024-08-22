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

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainView
    {
        private Timer _timer = new Timer(1000);
        private const int _refresh = 60;
        public MainWindow()
        {
            InitializeComponent();

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
                var menuItem = new MenuItem() { Header = "_Member Management" };
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
        }

        public bool UpgradeVersion()
        {
            throw new NotImplementedException();
        }

        public void ShowAbout()
        {
            throw new NotImplementedException();
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
    }
}