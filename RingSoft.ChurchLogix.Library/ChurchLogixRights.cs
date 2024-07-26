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
            category.Items.Add(new RightCategoryItem(item: "Add/Edit Staff", AppGlobals.LookupContext.Staff));

            Categories.Add(category);

            category = new RightCategory("Member Management", (int)MenuCategories.MemberManagement);
            category.Items.Add(new RightCategoryItem(item: "Add/Edit Members", AppGlobals.LookupContext.Members));

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
