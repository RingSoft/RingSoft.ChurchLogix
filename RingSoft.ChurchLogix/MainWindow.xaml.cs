﻿using System.Diagnostics;
using System.Windows;
using RingSoft.ChurchLogix.Library.ViewModels;
using System.Windows.Controls;
using RingSoft.ChurchLogix.Library;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Controls.WPF;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.ModelDefinition;
using RingSoft.DbLookup.Controls.WPF.AdvancedFind;
using System.Windows.Documents;
using RingSoft.DataEntryControls.Engine;
using Microsoft.VisualBasic.ApplicationServices;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.WPF;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainView
    {
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
            var loginWindow = new LoginWindow { Owner = this };

            var result = false;
            var loginResult = loginWindow.ShowDialog();

            if (loginResult != null && loginResult.Value == true)
            {
                result = (bool)loginResult;
                ViewModel.SetChurchProps();
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
                        CommandParameter = AppGlobals.LookupContext.MemberGiving,
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
    }
}