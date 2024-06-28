using System.Configuration;
using System.Data;
using System.Windows;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var appStart = new ChurchLogixAppStart(this);
            appStart.Start();

            base.OnStartup(e);
        }
    }

}
