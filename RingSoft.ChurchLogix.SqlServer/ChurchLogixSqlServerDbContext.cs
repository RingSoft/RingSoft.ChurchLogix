﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RingSoft.ChurchLogix.DataAccess;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.ChurchLogix.DataAccess.Model.ChurchLife;
using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.ChurchLogix.DataAccess.Model.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Model.StaffManagement;
using RingSoft.DbLookup.DataProcessor;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.SqlServer
{
    public class ChurchLogixSqlServerDbContext : DbContextEfCore, IChurchLogixDbContext
    {
        public DbContext DbContext => this;
        public DbSet<SystemMaster> SystemMaster { get; set; }
        public DbSet<SystemPreferences> SystemPreferences { get; set; }
        public DbSet<StaffPerson> Staff { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberGivingHistory> MembersGivingHistory { get; set; }
        public DbSet<MemberPeriodGiving> MembersPeriodGiving { get; set; }
        public DbSet<MemberGiving> MembersGiving { get; set; }
        public DbSet<MemberGivingDetails> MembersGivingDetails { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<StaffGroup> StaffGroups { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<BudgetItem> Budgets { get; set; }
        public DbSet<FundHistory> FundHistory { get; set; }
        public DbSet<FundPeriodTotals> FundPeriodTotals { get; set; }
        public DbSet<BudgetPeriodTotals> BudgetsPeriodTotals { get; set; }
        public DbSet<BudgetActual> BudgetActuals { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventMember> EventsMember { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SmallGroup> SmallGroups { get; set; }
        public DbSet<SmallGroupMember> SmallGroupsMember { get; set; }

        public bool IsDesignTime { get; set; }

        private static string? _connectionString;
        public static string? ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    return _lookupContext.SqlServerDataProcessor.ConnectionString;
                }
                return _connectionString;
            }
            set { _connectionString = value; }
        }


        private static ChurchLogixLookupContext _lookupContext;


        public ChurchLogixSqlServerDbContext()
        {
            DbConstants.ConstantGenerator = new SqlServerDbConstants();

            DataAccessGlobals.DbContext = this;
        }

        public ChurchLogixSqlServerDbContext(ChurchLogixLookupContext lookupContext)
        {
            DbConstants.ConstantGenerator = new SqlServerDbConstants();
            _lookupContext = lookupContext;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (IsDesignTime)
            {
                var sqlProcessor = new SqlServerDataProcessor();
                sqlProcessor.Server = "localhost\\SQLEXPRESS";
                sqlProcessor.Database = "RSChurchLogixTemp";
                sqlProcessor.SecurityType = SecurityTypes.WindowsAuthentication;
                optionsBuilder.UseSqlServer(sqlProcessor.ConnectionString);
            }
            else
                optionsBuilder.UseSqlServer(ConnectionString);

            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DbConstants.ConstantGenerator = new SqlServerDbConstants();
            DataAccessGlobals.ConfigureModel(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        public void SetLookupContext(ChurchLogixLookupContext lookupContext)
        {
            _lookupContext = lookupContext;
            DbConstants.ConstantGenerator = new SqlServerDbConstants();
        }

        public override DbContextEfCore GetNewDbContextEfCore()
        {
            return new ChurchLogixSqlServerDbContext();
        }

        public override void SetProcessor(DbDataProcessor processor)
        {

        }

        public override void SetConnectionString(string? connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
