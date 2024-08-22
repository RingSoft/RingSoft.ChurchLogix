using RingSoft.App.Library;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public class EventMaintenanceViewModel : AppDbMaintenanceViewModel<Event>
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

        private DateTime _beginDate;

        public DateTime BeginDate
        {
            get { return _beginDate; }
            set
            {
                if (_beginDate == value)
                    return;

                _beginDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate == value)
                    return;

                _endDate = value;
                OnPropertyChanged();
            }
        }

        private double? _memberCost;

        public double? MemberCost
        {
            get { return _memberCost; }
            set
            {
                if (_memberCost == value)
                    return;

                _memberCost = value;
                OnPropertyChanged();
            }
        }

        private double? _totalCost;

        public double? TotalCost
        {
            get { return _totalCost; }
            set
            {
                if (_totalCost == value)
                    return;

                _totalCost = value;
                OnPropertyChanged();
            }
        }

        private double? _totalPaid;

        public double? TotalPaid
        {
            get { return _totalPaid; }
            set
            {
                if (_totalPaid == value)
                    return;

                _totalPaid = value;
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


        protected override void PopulatePrimaryKeyControls(Event newEntity, PrimaryKeyValue primaryKeyValue)
        {
            Id = newEntity.Id;
        }

        protected override void LoadFromEntity(Event entity)
        {
            BeginDate = entity.BeginDate.ToLocalTime();
            EndDate = entity.EndDate.ToLocalTime();
            MemberCost = entity.MemberCost;
            TotalCost = entity.TotalCost;
            TotalPaid = entity.TotalPaid;
            Notes = entity.Notes;
        }

        protected override Event GetEntityData()
        {
            var result = new Event
            {
                Id = Id,
                Name = KeyAutoFillValue.Text,
                BeginDate = BeginDate.ToUniversalTime(),
                EndDate = EndDate.ToUniversalTime(),
                MemberCost = MemberCost,
                TotalCost = TotalCost,
                TotalPaid = TotalPaid,
                Notes = Notes
            };
            return result;
        }

        protected override void ClearData()
        {
            Id = 0;
            BeginDate = GblMethods.NowDate();
            EndDate = GblMethods.NowDate();
            MemberCost = 0;
            TotalCost = 0;
            TotalPaid = 0;
            Notes = null;
        }
    }
}
