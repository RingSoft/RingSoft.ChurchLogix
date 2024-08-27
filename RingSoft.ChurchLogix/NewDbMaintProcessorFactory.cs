using RingSoft.DbLookup.Controls.WPF;

namespace RingSoft.ChurchLogix
{
    public class NewDbMaintProcessorFactory : DbMaintenanceProcessorFactory
    {
        public override DbMaintenanceWindowProcessor GetProcessor()
        {
            return new NewDbMaintProcessor();
        }
    }
}
