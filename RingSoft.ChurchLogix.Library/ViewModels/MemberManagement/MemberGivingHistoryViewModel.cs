using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class MemberGivingHistoryViewModel : AppDbMaintenanceViewModel<MemberGivingHistory>
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

        private AutoFillSetup _fundAutoFillSetup;

        public AutoFillSetup FundAutoFillSetup
        {
            get { return _fundAutoFillSetup; }
            set
            {
                if (_fundAutoFillSetup == value)
                    return;

                _fundAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _fundAutoFillValue;

        public AutoFillValue FundAutoFillValue
        {
            get { return _fundAutoFillValue; }
            set
            {
                if (_fundAutoFillValue == value)
                    return;

                _fundAutoFillValue = value;
                OnPropertyChanged();
            }
        }

        private double _amount;

        public double Amount
        {
            get { return _amount; }
            set
            {
                if (_amount == value)
                    return;

                _amount = value;
                OnPropertyChanged();
            }
        }


        public MemberGivingHistoryViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition
                    .GetFieldDefinition(p => p.MemberId));

            FundAutoFillSetup = new AutoFillSetup(
                TableDefinition
                    .GetFieldDefinition(p => p.FundId));
        }

        protected override void Initialize()
        {
            if (Processor is IAppDbMaintenanceProcessor appDbMaintenanceProcessor)
            {
                appDbMaintenanceProcessor.WindowReadOnlyMode = true;
            }

            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(MemberGivingHistory newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(MemberGivingHistory entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            FundAutoFillValue = entity.Fund.GetAutoFillValue();
            Date = entity.Date;
            Amount = entity.Amount;
        }

        protected override MemberGivingHistory GetEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void ClearData()
        {
            MemberAutoFillValue = null;
            FundAutoFillValue = null;
            Date = DateTime.MinValue;
            Amount = 0;
        }
    }
}
