using RingSoft.App.Interop;
using RingSoft.App.Library;
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
    public class GroupMaintenanceViewModel : AppDbMaintenanceViewModel<Group>
    {
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

        public new IGroupView View { get; private set; }

        public GroupMaintenanceViewModel()
        {
            //UsersManager = new GroupsUsersManager(this);
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
            //UsersManager.LoadGrid(entity.UserGroups);
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
            //UsersManager.SetupForNewRecord();
        }

        protected override bool ValidateEntity(Group entity)
        {
            //if (!UsersManager.ValidateGrid())
            //{
            //    return false;
            //}
            return base.ValidateEntity(entity);
        }

        protected override bool SaveEntity(Group entity)
        {
            var context = AppGlobals.DataRepository.GetDataContext();
            if (context.SaveEntity(entity, $"Saving Group '{entity.Name}.'"))
            {
                var sgQuery = AppGlobals.DataRepository.GetDataContext().GetTable<StaffGroup>();
                var staffGroups = sgQuery.Where(p => p.GroupId == Id).ToList();
                context.RemoveRange(staffGroups);
                //staffGroups = UsersManager.GetList();
                if (staffGroups != null)
                {
                    foreach (var userGroup in staffGroups)
                    {
                        userGroup.GroupId = entity.Id;
                    }

                    context.AddRange(staffGroups);
                }

                return context.Commit("Saving UsersGroups");
            }

            return false;

        }

        protected override bool DeleteEntity()
        {
            var context = AppGlobals.DataRepository.GetDataContext();
            var query = context.GetTable<Group>();
            var group = query.FirstOrDefault(f => f.Id == Id);
            if (group != null)
            {
                var sgQuery = context.GetTable<StaffGroup>();
                var staffGroups = sgQuery.Where(p => p.GroupId == Id).ToList();
                context.RemoveRange(staffGroups);
                if (context.DeleteNoCommitEntity(group, $"Deleting Group '{group.Name}'"))
                {
                    return context.Commit($"Deleting Group '{group.Name}'");
                }
            }
            return false;
        }

    }
}
