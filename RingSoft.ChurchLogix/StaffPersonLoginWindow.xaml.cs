using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.ApplicationServices;
using RingSoft.ChurchLogix.Library.ViewModels;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for StaffPersonLoginWindow.xaml
    /// </summary>
    public partial class StaffPersonLoginWindow : IStaffLoginView
    {
        public StaffPersonLoginWindow(int staffPersonId)
        {
            InitializeComponent();
            Loaded += (sender, args) => ViewModel.Initialize(this, staffPersonId);
            if (staffPersonId > 0)
            {
                UserControl.IsEnabled = false;
            }

        }

        public void CloseWindow()
        {
            Close();
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }

        public void EnablePassword(bool enable)
        {
            PasswordBox.IsEnabled = enable;
        }
    }
}
