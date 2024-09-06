using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public class SmallGroupMaintenanceViewModel : DbMaintenanceViewModel<SmallGroup>
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

        private SmallGroupMemberManager _memberManager;

        public SmallGroupMemberManager MemberManager
        {
            get { return _memberManager; }
            set
            {
                if (_memberManager == value)
                    return;

                _memberManager = value;
                OnPropertyChanged();
            }
        }

        private string? _notes;

        public string? Notes
        {
            get { return _notes; }
            set
            {
                if (_notes == value)
                    return;

                _notes = value;
                OnPropertyChanged();
            }
        }

        public SmallGroupMaintenanceViewModel()
        {
            MemberManager = new SmallGroupMemberManager(this);

            RegisterGrid(MemberManager);
        }

        protected override void PopulatePrimaryKeyControls(SmallGroup newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(SmallGroup entity)
        {
            Notes = entity.Notes;
        }

        protected override SmallGroup GetEntityData()
        {
            return new SmallGroup
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                Notes = Notes,
            };
        }

        protected override void ClearData()
        {
            Id = 0;
            Notes = null;
        }
    }
}
