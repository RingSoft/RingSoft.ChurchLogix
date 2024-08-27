using RingSoft.DbLookup.Controls.WPF;

namespace RingSoft.ChurchLogix
{
    internal class NewDbProcessorFactory : DbMaintenanceProcessorFactory
    {
        public override DbMaintenanceWindowProcessor GetProcessor()
        {
            return new NewDbMaintProcessor1();
        }
    }
}
