using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.Library.ViewModels.Financial_Management
{
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
            }
        }


        protected override void PopulatePrimaryKeyControls(Fund newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(Fund entity)
        {
            Goal = entity.Goal;
            TotalCollected = entity.TotalCollected;
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
            };

            if (existFund != null)
            {
                result.TotalCollected = existFund.TotalCollected;
            }

            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            TotalCollected = 0;
            Goal = 0;
        }
    }
}
