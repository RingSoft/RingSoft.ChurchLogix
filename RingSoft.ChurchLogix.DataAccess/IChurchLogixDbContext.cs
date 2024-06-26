﻿using Microsoft.EntityFrameworkCore;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.DataAccess
{
    public interface IChurchLogixDbContext : IDbContext
    {
        DbContext DbContext { get; }

        DbSet<SystemMaster> SystemMaster { get; set; }

        DbSet<SystemPreferences> SystemPreferences { get; set; }

        DbSet<StaffPerson> Staff { get; set; }

        DbSet<Member> Members { get; set; }

        void SetLookupContext(ChurchLogixLookupContext lookupContext);
    }
}
