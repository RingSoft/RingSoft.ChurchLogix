using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public class StaffMaintenanceViewModel : AppDbMaintenanceViewModel<StaffPerson>
    {
        protected override void PopulatePrimaryKeyControls(StaffPerson newEntity, PrimaryKeyValue primaryKeyValue)
        {
            
        }

        protected override void LoadFromEntity(StaffPerson entity)
        {
            
        }

        protected override StaffPerson GetEntityData()
        {
            return new StaffPerson();
        }

        protected override void ClearData()
        {
            
        }
    }
}
