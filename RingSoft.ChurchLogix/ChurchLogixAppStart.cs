using System.Windows;
using RingSoft.App.Controls;
using RingSoft.ChurchLogix.Library;

namespace RingSoft.ChurchLogix
{
    public class ChurchLogixAppStart : AppStart
    {
        public ChurchLogixAppStart(Application application) : base(application, new MainWindow())
        {
            AppGlobals.InitSettings();
        }
    }
}
