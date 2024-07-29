namespace RingSoft.ChurchLogix.DataAccess
{
    public class StaffLookup
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }

    public class MemberLookup
    {
        public string Name { get; set; }
    }

    public class GroupsLookup
    {
        public string Name { get; set; }
    }

    public class StaffGroupsLookup
    {
        public string StaffPerson { get; set; }

        public string Group { get; set; }
    }
}
