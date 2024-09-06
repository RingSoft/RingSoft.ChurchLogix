using RingSoft.App.Interop;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.StaffManagement
{
    public interface IGroupView : IDbMaintenanceView
    {
        public string GetRights();

        public void LoadRights(string rightsString);

        public void ResetRights();
    }
    public class GroupMaintenanceViewModel : DbMaintenanceViewModel<Group>
    {
        #region Properties

        private int _id;

        public int Id
        {
            get => _id;
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

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                OnPropertyChanged();
            }
        }

        private bool _rightsChanged;

        public bool RightsChanged
        {
            get => _rightsChanged;
            set
            {
                if (_rightsChanged == value)
                    return;

                _rightsChanged = value;
                OnPropertyChanged();
            }
        }

        private GroupsStaffManager _staffManager;

        public GroupsStaffManager StaffManager
        {
            get { return _staffManager; }
            set
            {
                if (_staffManager == value)
                {
                    return;
                }
                _staffManager = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public new IGroupView View { get; private set; }

        public GroupMaintenanceViewModel()
        {
            StaffManager = new GroupsStaffManager(this);
            RegisterGrid(StaffManager);
        }
        protected override void Initialize()
        {
            View = base.View as IGroupView;
            if (View == null)
                throw new Exception($"Group View interface must be of type '{nameof(IGroupView)}'.");

            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(Group newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(Group entity)
        {
            View.LoadRights(entity.Rights.Decrypt());
        }

        protected override Group GetEntityData()
        {
            var group = new Group
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                Rights = View.GetRights().Encrypt()
            };

            return group;
        }

        protected override void ClearData()
        {
            Id = 0;
            View.ResetRights();
        }
    }
}
