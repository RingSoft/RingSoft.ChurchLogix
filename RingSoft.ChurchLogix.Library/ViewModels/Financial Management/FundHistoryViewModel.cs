using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public class FundHistoryViewModel : DbMaintenanceViewModel<FundHistory>
    {
        #region Properties

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

        private AutoFillSetup _fundAutoFilSetup;

        public AutoFillSetup FundAutoFillSetup
        {
            get { return _fundAutoFilSetup; }
            set
            {
                if (_fundAutoFilSetup == value)
                    return;

                _fundAutoFilSetup = value;
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

        private AutoFillSetup _budgetAutoFilSetup;

        public AutoFillSetup BudgetAutoFillSetup
        {
            get { return _budgetAutoFilSetup; }
            set
            {
                if (_budgetAutoFilSetup == value)
                    return;

                _budgetAutoFilSetup = value;
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

        private string _amountType;

        public string AmountType
        {
            get { return _amountType; }
            set
            {
                if (_amountType == value)
                    return;

                _amountType = value;
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

        #endregion

        public UiCommand BudgetUiCommand { get; }

        public FundHistoryViewModel()
        {
            FundAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.FundId));
            BudgetAutoFillSetup = new AutoFillSetup(
                TableDefinition.GetFieldDefinition(p => p.BudgetId));

            BudgetUiCommand = new UiCommand();
        }

        protected override void Initialize()
        {
            ReadOnlyMode = true;
            base.Initialize();
        }

        protected override void PopulatePrimaryKeyControls(FundHistory newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(FundHistory entity)
        {
            FundAutoFillValue = entity.Fund.GetAutoFillValue();
            BudgetAutoFillValue = entity.BudgetItem.GetAutoFillValue();
            if (BudgetAutoFillValue == null)
            {
                BudgetUiCommand.Visibility = UiVisibilityTypes.Collapsed;
            }
            else
            {
                BudgetUiCommand.Visibility = UiVisibilityTypes.Visible;
            }
            Date = entity.Date;
            var enumTranslation = TableDefinition.GetFieldDefinition(
                    p => p.AmountType)
                .EnumTranslation;

            if (enumTranslation != null)
            {
                AmountType = enumTranslation
                    .TypeTranslations
                    .FirstOrDefault(p => p.NumericValue == entity.AmountType)
                    .TextValue;
            }
            Amount = entity.Amount;
        }

        protected override FundHistory GetEntityData()
        {
            throw new NotImplementedException();
        }

        protected override void ClearData()
        {
            Id = 0;
            FundAutoFillValue = null;
            BudgetAutoFillValue = null;
            BudgetUiCommand.Visibility = UiVisibilityTypes.Collapsed;
            Date = DateTime.Today;
            Amount = 0;
            AmountType = string.Empty;
        }
    }
}
