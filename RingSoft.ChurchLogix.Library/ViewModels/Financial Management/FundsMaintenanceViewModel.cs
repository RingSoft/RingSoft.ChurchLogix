using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup;
using RingSoft.DbLookup.Lookup;
using RingSoft.DbLookup.QueryBuilder;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IFundView
    {
        void RefreshView();
    }
    public class FundsMaintenanceViewModel : AppDbMaintenanceViewModel<Fund>
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

        private double _goal;

        public double Goal
        {
            get { return _goal; }
            set
            {
                if (_goal == value)
                {
                    return;
                }
                _goal = value;
                OnPropertyChanged();
                UpdateDiffValues();
            }
        }


        private double _totalCollected;

        public double TotalCollected
        {
            get { return _totalCollected; }
            set
            {
                if (_totalCollected == value)
                    return;
                _totalCollected = value;
                OnPropertyChanged();
                UpdateDiffValues();
            }
        }

        private double _goalDif;

        public double GoalDif
        {
            get { return _goalDif; }
            set
            {
                if (_goalDif == value)
                    return;

                _goalDif = value;
                OnPropertyChanged();
            }
        }

        private double _totalSpent;

        public double TotalSpent
        {
            get { return _totalSpent; }
            set
            {
                if (_totalSpent == value)
                    return;

                _totalSpent = value;
                OnPropertyChanged();
                UpdateDiffValues();
            }
        }

        private double _fundDiff;

        public double FundDiff
        {
            get
            {
                return _fundDiff;
            }
            set
            {
                if (_fundDiff == value)
                    return;

                _fundDiff = value;
                OnPropertyChanged();
            }
        }

        private LookupDefinition<FundHistoryLookup, FundHistory> _fundHistoryLookupDefinition;

        public LookupDefinition<FundHistoryLookup, FundHistory> FundHistoryLookupDefinition
        {
            get { return _fundHistoryLookupDefinition; }
            set
            {
                if (_fundHistoryLookupDefinition == value)
                    return;

                _fundHistoryLookupDefinition = value;
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


        public IFundView View { get; private set; }

        public FundsMaintenanceViewModel()
        {
            FundHistoryLookupDefinition = AppGlobals.LookupContext.FundHistoryLookup.Clone();
            FundHistoryLookupDefinition.InitialOrderByColumn = FundHistoryLookupDefinition
                .GetColumnDefinition(p => p.Date);
            FundHistoryLookupDefinition.InitialOrderByType = OrderByTypes.Descending;
        }

        protected override void Initialize()
        {
            if (base.View is IFundView fundView)
            {
                View = fundView;
            }
            else
            {
                throw new Exception($"Maintenance window must implement {nameof(IFundView)} interface.");
            }
            base.Initialize();
        }


        protected override void PopulatePrimaryKeyControls(Fund newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;

            FundHistoryLookupDefinition.FilterDefinition.ClearFixedFilters();
            FundHistoryLookupDefinition.FilterDefinition.AddFixedFilter(
                p => p.FundId, Conditions.Equals, newEntity.Id);

            var command = GetLookupCommand(LookupCommands.Refresh, primaryKeyValue);
            FundHistoryLookupDefinition.SetCommand(command);

        }

        protected override void LoadFromEntity(Fund entity)
        {
            Goal = entity.Goal;
            TotalCollected = entity.TotalCollected;
            TotalSpent = entity.TotalSpent;
            Notes = entity.Notes;
        }

        protected override Fund GetEntityData()
        {
            var context = SystemGlobals.DataRepository.GetDataContext();
            var table = context.GetTable<Fund>();
            var existFund = table.FirstOrDefault(p => p.Id == Id);

            var result = new Fund
            {
                Id = Id,
                Description = KeyAutoFillValue.Text,
                Goal = Goal,
                Notes = Notes,
            };

            if (existFund != null)
            {
                result.TotalCollected = existFund.TotalCollected;
                result.TotalSpent = existFund.TotalSpent;
            }

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            GoalDif = 0;
            FundDiff = 0;
            TotalCollected = 0;
            Goal = 0;
            TotalSpent = 0;
            Notes = null;

            var command = GetLookupCommand(LookupCommands.Clear);
            FundHistoryLookupDefinition.SetCommand(command);
        }

        private void UpdateDiffValues()
        {
            GoalDif = TotalCollected - Goal;
            FundDiff = TotalCollected - TotalSpent;
            View.RefreshView();
        }
    }
}
