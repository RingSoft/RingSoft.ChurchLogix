using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public interface IMemberGivingView : IDbMaintenanceView
    {
        void ShowPostProcedure(MemberGivingMaintenanceViewModel viewModel);

        void UpdateProcedure(string text);
    }
    public class MemberGivingMaintenanceViewModel : AppDbMaintenanceViewModel<MemberGiving>
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

        private AutoFillSetup _memberAutoFillSetup;

        public AutoFillSetup MemberAutoFillSetup
        {
            get { return _memberAutoFillSetup; }
            set
            {
                if (_memberAutoFillSetup == value)
                    return;

                _memberAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _memberAutoFillValue;

        public AutoFillValue MemberAutoFillValue
        {
            get { return _memberAutoFillValue; }
            set
            {
                if (_memberAutoFillValue == value)
                    return;

                _memberAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value)
                    return;

                _date = value;
                OnPropertyChanged();
            }
        }

        private double _total;

        public double Total
        {
            get { return _total; }
            set
            {
                if (_total == value)
                    return;

                _total = value;
                OnPropertyChanged();
            }
        }


        private MemberGivingDetailsManager _detailsManager;

        public MemberGivingDetailsManager DetailsManager
        {
            get { return _detailsManager; }
            set
            {
                if (_detailsManager == value)
                    return;

                _detailsManager = value;
                OnPropertyChanged();
            }
        }


        public UiCommand MemberUiCommand { get; }

        public IMemberGivingView View { get; private set; }

        public RelayCommand PostCommand { get; }

        public MemberGivingMaintenanceViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.MemberId));

            MemberUiCommand = new UiCommand();

            DetailsManager = new MemberGivingDetailsManager(this);
            RegisterGrid(DetailsManager);

            PostCommand = new RelayCommand((() =>
            {
                if (CheckDirty())
                {
                    View.ShowPostProcedure(this);
                }
            }));

        }

        protected override void Initialize()
        {
            if (base.View is IMemberGivingView memberGivingView)
            {
                View = memberGivingView;
            }
            else
            {
                throw new Exception("Invalid view interface");
            }
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(MemberGiving newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(MemberGiving entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            Date = entity.Date;
        }

        protected override MemberGiving GetEntityData()
        {
            var result = new MemberGiving()
            {
                Id = Id,
                MemberId = MemberAutoFillValue.GetEntity<Member>().Id,
                Date = Date,
            };

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            MemberAutoFillValue = null;
            Date = DateTime.Today;
            MemberUiCommand.SetFocus();
            Total = 0;
        }

        public bool PostGiving()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                View.UpdateProcedure(i.ToString());
            }
            return true;
        }
    }
}
