using System.ComponentModel;
using System.Runtime.CompilerServices;
using RingSoft.DataEntryControls.Engine;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
    public interface IBudgetActualsView
    {
        void CloseWindow();
    }
    public class BudgetActualsViewModel : INotifyPropertyChanged
    {
        private BudgetActualsManager _gridManager;

        public BudgetActualsManager GridManager
        {
            get { return _gridManager; }
            set
            {
                if (_gridManager == value)
                {
                    return;
                }
                _gridManager = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OkCommand { get; }

        public RelayCommand CancelCommand { get; }

        public IBudgetActualsView View { get; private set; }

        private bool? _valCloseResult;

        public BudgetActualsViewModel()
        {
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand((async () =>
            {
                if (ValidateClose())
                {
                    View.CloseWindow();
                }
            }));
            GridManager = new BudgetActualsManager();
        }

        public void Initialize(IBudgetActualsView view)
        {
            View = view;
        }

        private void OnOk()
        {
            ControlsGlobals.UserInterface.ShowMessageBox("Nub", "Nub", RsMessageBoxIcons.Exclamation);
        }

        private async void CloseResult()
        {
            _valCloseResult = null;
            var message = "Are you sure you wish to close this window?";
            var caption = "Confirm Close";
            if (await ControlsGlobals.UserInterface
            .ShowYesNoMessageBox(message, caption) == MessageBoxButtonsResult.Yes)
            {
                _valCloseResult = true;
                return;
            }

            _valCloseResult = false;
        }

        public bool ValidateClose()
        {
            CloseResult();
            if (_valCloseResult != null && _valCloseResult == true)
            {
                return true;
            }
            return false;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
