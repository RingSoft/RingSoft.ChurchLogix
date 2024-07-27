using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.Library
{
    public enum MenuCategories
    {
        StaffManagement = 0,
        Tools = 1,
        MemberManagement = 2,
    }

    public class ChurchLogixRights : ItemRights
    {
        public override void SetupRightsTree()
        {
            var category = new RightCategory("Staff Management", (int)MenuCategories.StaffManagement);
            var staffItem = new RightCategoryItem(item: "Add/Edit Staff", AppGlobals.LookupContext.Staff);
            category.Items.Add(staffItem);

            category.Items.Add(new RightCategoryItem("Add/Edit Security Groups", AppGlobals.LookupContext.Groups));

            Categories.Add(category);

            category = new RightCategory("Member Management", (int)MenuCategories.MemberManagement);
            var memberItem = new RightCategoryItem(item: "Add/Edit Members", AppGlobals.LookupContext.Members);
            AddSpecialRight((int)MemberSpecialRights.AllowViewGiving, "Allow View Member Giving"
                , AppGlobals.LookupContext.Members);

            category.Items.Add(memberItem);

            Categories.Add(category);

        }
    }

    public class ChurchLogixRightsFactory : ItemRightsFactory
    {
        public override ItemRights GetNewItemRights()
        {
            return new ChurchLogixRights();
        }
    }

}
