using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library.ViewModels.Financial_Management;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.MemberManagement
{
    public class MemberGivingPeriodTotalsViewModel : DbMaintenanceViewModel<MemberPeriodGiving>
    {
        private AutoFillSetup _memberAutoFillSetup;

        public AutoFillSetup MemberAutoFillSetup
        {
            get { return _memberAutoFillSetup; }
            set
            {
                if (_memberAutoFillSetup == value)
                {
                    return;
                }
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

        private DateTime _dateEnding;

        public DateTime DateEnding
        {
            get { return _dateEnding; }
            set
            {
                if (_dateEnding == value)
                    return;

                _dateEnding = value;
                OnPropertyChanged();
            }
        }

        private string _periodType;

        public string PeriodType
        {
            get { return _periodType; }
            set
            {
                if (_periodType == value)
                    return;

                _periodType = value;
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

        private LookupDefinition<MemberGivingHistoryLookup, MemberGivingHistory> _historyLookupDefinition;

        public LookupDefinition<MemberGivingHistoryLookup, MemberGivingHistory> HistoryLookupDefinition
        {
            get { return _historyLookupDefinition; }
            set
            {
                if (_historyLookupDefinition == value)
                    return;

                _historyLookupDefinition = value;
                OnPropertyChanged();
            }
        }


        public MemberGivingPeriodTotalsViewModel()
        {
            MemberAutoFillSetup = new AutoFillSetup(
                TableDefinition
                    .GetFieldDefinition(p => p.MemberId));

            HistoryLookupDefinition = AppGlobals.LookupContext.MemberGivingHistoryLookup.Clone();
        }

        protected override void Initialize()
        {
            ReadOnlyMode = true;
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(MemberPeriodGiving newEntity, PrimaryKeyValue primaryKeyValue)
        {
            var periodType = (PeriodTypes)newEntity.PeriodType;
            DateEnding = newEntity.Date;
            var enumTranslation =
                TableDefinition
                    .GetFieldDefinition(p => p.PeriodType)
                    .EnumTranslation;

            if (enumTranslation != null)
            {
                var enumText = enumTranslation
                    .TypeTranslations
                    .FirstOrDefault(p => p.NumericValue == newEntity.PeriodType)
                    .TextValue;

                PeriodType = enumText;
            }

            DateTime? beginDate = null;
            DateTime? endDate = null;

            switch (periodType)
            {
                case PeriodTypes.MonthEnding:
                    beginDate = new DateTime(DateEnding.Year, DateEnding.Month, 1);
                    endDate = beginDate.Value.AddMonths(1).AddDays(-1);
                    break;
                case PeriodTypes.YearEnding:
                    beginDate = new DateTime(DateEnding.Year, 1, 1);
                    endDate = new DateTime(DateEnding.Year, 12, 31);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            HistoryLookupDefinition.FilterDefinition.ClearFixedFilters();
            HistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.MemberId, Conditions.Equals, newEntity.MemberId);
            HistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Date, Conditions.GreaterThanEquals, beginDate.Value);
            HistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.Date, Conditions.LessThanEquals, endDate.Value);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            HistoryLookupDefinition.SetCommand(command);
        }

        protected override void LoadFromEntity(MemberPeriodGiving entity)
        {
            MemberAutoFillValue = entity.Member.GetAutoFillValue();
            Total = entity.TotalGiving;
        }

        protected override MemberPeriodGiving GetEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void ClearData()
        {
            MemberAutoFillValue = null;
            DateEnding = DateTime.MinValue;
            PeriodType = string.Empty;
            Total = 0;
            var command = GetLookupCommand(LookupCommands.Clear);
            HistoryLookupDefinition.SetCommand(command);
        }
    }
}
