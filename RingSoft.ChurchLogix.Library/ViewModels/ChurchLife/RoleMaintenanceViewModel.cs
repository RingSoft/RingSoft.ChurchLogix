using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public class RoleMaintenanceViewModel : DbMaintenanceViewModel<Role>
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                {
                    return;
                }
                _id = value;
                OnPropertyChanged();
            }
        }

        protected override void PopulatePrimaryKeyControls(Role newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(Role entity)
        {
            
        }

        protected override Role GetEntityData()
        {
            return new Role
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
            };
        }

        protected override void ClearData()
        {
            Id = 0;
        }
    }
}
