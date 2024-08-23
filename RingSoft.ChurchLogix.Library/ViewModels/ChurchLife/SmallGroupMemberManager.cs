using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.DataEntryControls.Engine.DataEntryGrid;
using RingSoft.DbLookup;
using RingSoft.DbLookup.AutoFill;
using RingSoft.DbMaintenance;

namespace RingSoft.ChurchLogix.Library.ViewModels.ChurchLife
{
    public enum SmallGroupMemberColumns
    {
        Member = 1,
        Role = 2,
    }
    public class SmallGroupMemberManager : DbMaintenanceDataEntryGridManager<SmallGroupMember>
    {
        public const int MemberColumnId = (int)SmallGroupMemberColumns.Member;
        public const int RoleColumnId = (int)SmallGroupMemberColumns.Role;

        public new SmallGroupMaintenanceViewModel ViewModel { get; }

        private int _selectedMemberId = -1;

        public SmallGroupMemberManager(SmallGroupMaintenanceViewModel viewModel) : base(viewModel)
        {
            ViewModel = viewModel;
        }

        protected override DataEntryGridRow GetNewRow()
        {
            return new SmallGroupMemberRow(this);
        }

        protected override DbMaintenanceDataEntryGridRow<SmallGroupMember> ConstructNewRowFromEntity(SmallGroupMember entity)
        {
            return new SmallGroupMemberRow(this);
        }

        public SmallGroupMemberRow GetRowForMember(AutoFillValue newAutoFillValue)
        {
            var newMemberId = newAutoFillValue.GetEntity<Member>().Id;

            return Rows.OfType<SmallGroupMemberRow>()
                .FirstOrDefault(p => p.MemberId == newMemberId);
        }

        protected override void SelectRowForEntity(SmallGroupMember entity)
        {
            _selectedMemberId = entity.MemberId;
            base.SelectRowForEntity(entity);
        }

        public override void LoadGrid(IEnumerable<SmallGroupMember> entityList)
        {
            base.LoadGrid(entityList);
            if (_selectedMemberId >= 0)
            {
                var row = Rows.OfType<SmallGroupMemberRow>()
                    .FirstOrDefault(p => p.MemberId == _selectedMemberId);
                _selectedMemberId = -1;
                GotoCell(row, MemberColumnId);
            }
        }
    }
}
