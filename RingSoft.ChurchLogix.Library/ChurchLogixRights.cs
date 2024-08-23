using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.Library.ViewModels.MemberManagement;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.Library
{
    public enum MenuCategories
    {
        StaffManagement = 0,
        System = 1,
        MemberManagement = 2,
        FinancialManagement = 3,
        ChurchLife = 4,
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
            var rightItem = new RightCategoryItem(item: "Add/Edit Members", AppGlobals.LookupContext.Members);
            AddSpecialRight((int)MemberSpecialRights.AllowViewGiving, "Allow View Member Giving"
                , AppGlobals.LookupContext.Members);

            category.Items.Add(rightItem);

            Categories.Add(category);

            category = new RightCategory("Financial Management", (int)MenuCategories.FinancialManagement);

            rightItem = new RightCategoryItem(item: "Add/Edit Funds", AppGlobals.LookupContext.Funds);
            category.Items.Add(rightItem);

            rightItem = new RightCategoryItem(item: "Add/Edit Budget Items", AppGlobals.LookupContext.Budgets);
            category.Items.Add(rightItem);

            rightItem = new RightCategoryItem(item: "Add/Edit Budget Costs", AppGlobals.LookupContext.BudgetActuals);
            category.Items.Add(rightItem);

            Categories.Add(category);

            category = new RightCategory("Church Life", (int)MenuCategories.ChurchLife);

            rightItem = new RightCategoryItem(item: "Add/Edit Events"
                , AppGlobals.LookupContext.Events);
            category.Items.Add(rightItem);

            rightItem = new RightCategoryItem(item: "Add/Edit Small Group Roles"
                , AppGlobals.LookupContext.Roles);
            category.Items.Add(rightItem);

            Categories.Add(category);

            category = new RightCategory("System", (int)MenuCategories.System);

            rightItem = new RightCategoryItem(item: "Add/Edit System Preferences"
                , AppGlobals.LookupContext.SystemPreferences);
            category.Items.Add(rightItem);

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
