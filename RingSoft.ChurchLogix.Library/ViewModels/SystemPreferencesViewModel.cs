using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface ISystemPreferencesView : IDbMaintenanceView
    {
        void CloseWindow();
    }
    public class SystemPreferencesViewModel : AppDbMaintenanceViewModel<SystemPreferences>
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

        private DateTime _fiscalYearEnd;

        public DateTime FiscalYearEnd
        {
            get { return _fiscalYearEnd; }
            set
            {
                if (_fiscalYearEnd == value)
                    return;

                _fiscalYearEnd = value;
                OnPropertyChanged();
            }
        }

        public DateTime OriginalFYEnd { get; private set; }

        public RelayCommand OkCommand { get; }

        public RelayCommand CancelCommand { get; }

        public ISystemPreferencesView View { get; private set; }

        public SystemPreferencesViewModel()
        {
            OkCommand = new RelayCommand(OnOk);

            CancelCommand = new RelayCommand(OnCancel);
        }

        protected override void Initialize()
        {
            if (base.View is ISystemPreferencesView systemPreferencesView)
            {
                View = systemPreferencesView;
            }
            base.Initialize();
            OnGotoNextButton();
            NextCommand.IsEnabled = false;
            PreviousCommand.IsEnabled = false;
        }

        protected override void PopulatePrimaryKeyControls(SystemPreferences newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(SystemPreferences entity)
        {
            FiscalYearEnd = entity.FiscalYearEnd.Value;
            OriginalFYEnd = FiscalYearEnd;
        }

        protected override SystemPreferences GetEntityData()
        {
            var result = new SystemPreferences()
            {
                Id = Id,
                FiscalYearEnd = FiscalYearEnd,
            };

            return result;
        }

        protected override void ClearData()
        {
        }

        protected override bool SaveEntity(SystemPreferences entity)
        {
            AppGlobals.SystemPreferences = entity;
            return base.SaveEntity(entity);
        }

        private async void OnOk()
        {
            if (FiscalYearEnd != OriginalFYEnd)
            {
                if (FiscalYearEnd.Month - OriginalFYEnd.Month != 0
                    || FiscalYearEnd.Day - OriginalFYEnd.Day != 0)
                {
                    var context = SystemGlobals.DataRepository.GetDataContext();
                    var needsRecalculation = false;
                    var fundPeriodTable = context.GetTable<FundPeriodTotals>();
                    var fundPeriodRecs = fundPeriodTable
                        .Where(p => p.PeriodType == (int)PeriodTypes.YearEnding
                        && p.Date == FiscalYearEnd);
                    var budgetPeriodTable = context.GetTable<BudgetPeriodTotals>();
                    var budgetPeriodRecs = budgetPeriodTable
                        .Where(p => p.PeriodType == (int)PeriodTypes.YearEnding
                                    && p.Date == FiscalYearEnd);
                    var memberPeriodTable = context.GetTable<MemberPeriodGiving>();
                    var memberPeriodRecs = memberPeriodTable
                        .Where(p => p.PeriodType == (int)PeriodTypes.YearEnding
                                    && p.Date == FiscalYearEnd);

                    if (!fundPeriodRecs.Any()
                        || !budgetPeriodRecs.Any()
                        || !memberPeriodRecs.Any())
                    {
                        var beginDate = FiscalYearEnd.AddYears(-1)
                            .AddDays(1);
                    }
                }
            }

        }

        private void OnCancel()
        {

        }
    }
}
