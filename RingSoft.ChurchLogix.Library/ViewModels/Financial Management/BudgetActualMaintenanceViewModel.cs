using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IBudgetActualView
    {
        void ShowPostProcedure();

        void UpdateProcedure(string text);
    }
    public class BudgetActualMaintenanceViewModel : AppDbMaintenanceViewModel<BudgetActual>
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

        private AutoFillSetup _budgetAutoFillSetup;

        public AutoFillSetup BudgetAutoFillSetup
        {
            get { return _budgetAutoFillSetup; }
            set
            {
                if (_budgetAutoFillSetup == value)
                    return;

                _budgetAutoFillSetup = value;
                OnPropertyChanged();
            }
        }

        private AutoFillValue _budgetAutoFillValue;

        public AutoFillValue BudgetAutoFillValue
        {
            get { return _budgetAutoFillValue; }
            set
            {
                if (_budgetAutoFillValue == value)
                    return;

                _budgetAutoFillValue = value;
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

        public UiCommand BudgetAutoFillCommand { get; }

        public RelayCommand PostCostsCommand { get; }

        public IBudgetActualView View { get; private set; }

        public BudgetActualMaintenanceViewModel()
        {
            BudgetAutoFillSetup = new AutoFillSetup
                (TableDefinition.GetFieldDefinition(p => p.BudgetId));

            BudgetAutoFillCommand = new UiCommand();
            PostCostsCommand = new RelayCommand((() =>
            {
                View.ShowPostProcedure();
            }));
        }

        protected override void Initialize()
        {
            if (base.View is IBudgetActualView view)
            {
                View = view;
            }
            else
            {
                throw new Exception($"Must implement {nameof(IBudgetActualView)}");
            }
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(BudgetActual newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(BudgetActual entity)
        {
            BudgetAutoFillValue = entity.Budget.GetAutoFillValue();
            Date = entity.Date;
            Amount = entity.Amount;
        }

        protected override BudgetActual GetEntityData()
        {
            return new BudgetActual
            {
                Id = Id,
                BudgetId = BudgetAutoFillValue.GetEntity<BudgetItem>().Id,
                Date = Date,
                Amount = Amount,
            };
        }

        protected override void ClearData()
        {
            Id = 0;
            BudgetAutoFillValue = null;
            Date = DateTime.Today;
            Amount = 0;

            BudgetAutoFillCommand.SetFocus();
        }

        public bool PostCosts()
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
