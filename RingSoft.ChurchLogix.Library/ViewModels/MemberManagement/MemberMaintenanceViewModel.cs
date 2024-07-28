using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class HouseholdToken
    {

    }
    public enum MemberSpecialRights
    {
        AllowViewGiving = 1,
    }

    public interface IMemberView : IDbMaintenanceView
    {
        public void RefreshView();
    }
    public class MemberMaintenanceViewModel : AppDbMaintenanceViewModel<Member>
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

        private AutoFillSetup _householdAutoFillSetup;

        public AutoFillSetup HouseholdAutoFillSetup
        {
            get { return _householdAutoFillSetup; }
            set
            {
                if (_householdAutoFillSetup == value)
                {
                    return;
                }
                _householdAutoFillSetup = value;
                OnPropertyChanged(null, false);
            }
        }

        private AutoFillValue _householdAutoFillValue;

        public AutoFillValue HouseholdAutoFillValue
        {
            get { return _householdAutoFillValue; }
            set
            {
                if (_householdAutoFillValue == value)
                {
                    return;
                }
                _householdAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private string? _phone;

        public string? Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                if (_phone == value)
                {
                    return;
                }
                _phone = value;
                OnPropertyChanged();
            }
        }

        private string? _email;

        public string? Email
        {
            get { return _email; }
            set
            {
                if (_email == value)
                {
                    return;
                }
                _email = value;
                View.RefreshView();
                OnPropertyChanged();
            }
        }

        private LookupDefinition<MemberLookup, Member> _householdMembersLookupDefinition;

        public LookupDefinition<MemberLookup, Member> HouseholdMembersLookupDefinition
        {
            get => _householdMembersLookupDefinition;
            set
            {
                if (_householdMembersLookupDefinition == value)
                    return;

                _householdMembersLookupDefinition = value;
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
                {
                    return;
                }
                _notes = value;
                OnPropertyChanged();
            }
        }

        public IMemberView View { get; private set; }

        public RelayCommand AddModifyHouseholdLookupCommand { get; }

        public AutoFillValue DefaultHouseHoldAutoFillValue { get; private set; }

        private LookupDefinition<MemberLookup, Member> _householdLookupDefinition;

        public MemberMaintenanceViewModel()
        {
            _householdLookupDefinition = AppGlobals.LookupContext.MemberLookup.Clone();
                
            HouseholdAutoFillSetup = new AutoFillSetup(_householdLookupDefinition);

            HouseholdMembersLookupDefinition = AppGlobals.LookupContext.MemberLookup.Clone();

            AddModifyHouseholdLookupCommand = new RelayCommand(AddModifyHousehold);
        }

        protected override void Initialize()
        {
            View = base.View as IMemberView;
            if (View == null)
                throw new Exception($"Member View interface must be of type '{nameof(IMemberView)}'.");

            if (LookupAddViewArgs != null && LookupAddViewArgs.ParentWindowPrimaryKeyValue != null &&  LookupAddViewArgs.ParentWindowPrimaryKeyValue.TableDefinition ==
                AppGlobals.LookupContext.Members)
            {
                var member =
                    AppGlobals.LookupContext.Members.GetEntityFromPrimaryKeyValue(LookupAddViewArgs
                        .ParentWindowPrimaryKeyValue);

                var context = AppGlobals.DataRepository.GetDataContext();
                var table = context.GetTable<Member>();
                member = table.FirstOrDefault(p => p.Id == member.Id);
                if (LookupAddViewArgs.InputParameter is HouseholdToken)
                {
                    DefaultHouseHoldAutoFillValue = member.GetAutoFillValue();
                }
            }


            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(Member newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
            View.RefreshView();
            _householdLookupDefinition.FilterDefinition.ClearFixedFilters();
            _householdLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Id
                , Conditions.NotEquals, Id);

            HouseholdMembersLookupDefinition.FilterDefinition.ClearFixedFilters();
            HouseholdMembersLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.HouseholdId
                , Conditions.Equals, Id);
            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            HouseholdMembersLookupDefinition.SetCommand(command);
        }

        protected override void LoadFromEntity(Member entity)
        {
            HouseholdAutoFillValue = entity.Household.GetAutoFillValue();
            Phone = entity.PhoneNumber;
            Email = entity.Email;
            Notes = entity.Notes;
        }

        protected override Member GetEntityData()
        {
            var result = new Member()
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                HouseholdId = HouseholdAutoFillValue.GetEntity<Member>().Id,
                PhoneNumber = Phone,
                Email = Email,
                Notes = Notes
            };

            if (result.HouseholdId == 0)
            {
                result.HouseholdId = null;
            }

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            HouseholdAutoFillValue = DefaultHouseHoldAutoFillValue;
            Phone = null;
            Email = null;
            Notes = null;
            View.RefreshView();
            HouseholdMembersLookupDefinition.SetCommand(GetLookupCommand(LookupCommands.Clear));
        }

        private void AddModifyHousehold()
        {
            if (ExecuteAddModifyCommand() == DbMaintenanceResults.Success)
                HouseholdMembersLookupDefinition.SetCommand(GetLookupCommand(LookupCommands.AddModify
                , null, new HouseholdToken()));
        }
    }
}
