using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DataEntryControls.Engine;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public class FundHistoryViewModel : AppDbMaintenanceViewModel<FundHistory>
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
            if (Processor is IAppDbMaintenanceProcessor appDbMaintenanceProcessor)
            {
                appDbMaintenanceProcessor.WindowReadOnlyMode = true;
            }

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
        }
    }
}
