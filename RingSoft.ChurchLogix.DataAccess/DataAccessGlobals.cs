﻿using Microsoft.EntityFrameworkCore;
using RingSoft.ChurchLogix.DataAccess.Configurations.FinancialManagement;
using RingSoft.ChurchLogix.DataAccess.Configurations.MemberManagement;
using RingSoft.ChurchLogix.DataAccess.Configurations.StaffManagement;
using RingSoft.ChurchLogix.DataAccess.Model;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.DataAccess
{
    public class DataAccessGlobals
    {
        public static DbContext DbContext { get; set; }

        public static void ConfigureModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StaffPersonConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new StaffGroupConfiguration());

            modelBuilder.ApplyConfiguration(new MemberConfiguration());
            modelBuilder.ApplyConfiguration(new MemberGivingHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new MemberPeriodGivingConfiguration());
            modelBuilder.ApplyConfiguration(new MemberGivingConfiguration());
            modelBuilder.ApplyConfiguration(new MemberGivingDetailsConfiguration());

            modelBuilder.ApplyConfiguration(new FundConfiguration());
            modelBuilder.ApplyConfiguration(new BudgetConfiguration());
            modelBuilder.ApplyConfiguration(new FundHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new FundPeriodTotalsConfiguration());
            modelBuilder.ApplyConfiguration(new BudgetPeriodTotalsConfiguration());
            modelBuilder.ApplyConfiguration(new BudgetActualConfiguration());
        }

        public static void SetupSysPrefs()
        {
            var sysPrefsSet = DbContext.Set<SystemPreferences>();
            var sysPrefsRecord = sysPrefsSet.FirstOrDefault();
            if (sysPrefsRecord == null)
            {
                sysPrefsRecord = new SystemPreferences();
                SaveEntity(sysPrefsRecord, "Creating New System Preferences Record");
            }
        }

        public static bool SaveNoCommitEntity<TEntity>(TEntity entity, string message) where TEntity : class
        {
            if (!DbContext.SaveNoCommitEntity(DbContext.Set<TEntity>(), entity, message))
                return false;

            return true;
        }

        public static bool SaveEntity<TEntity>(TEntity entity, string message) where TEntity : class
        {
            return DbContext.SaveEntity(DbContext.Set<TEntity>(), entity, message);
        }

        public static bool DeleteEntity<TEntity>(TEntity entity, string message) where TEntity : class
        {
            return DbContext.DeleteEntity(DbContext.Set<TEntity>(), entity, message);
        }

        public static bool AddNewNoCommitEntity<TEntity>(TEntity entity, string message) where TEntity : class
        {
            return DbContext.AddNewNoCommitEntity(DbContext.Set<TEntity>(), entity, message);
        }

        public static bool Commit(string message)
        {
            var result = DbContext.SaveEfChanges(message);

            return result;
        }

        public static void RemoveRange<TEntity>(IEnumerable<TEntity> listToRemove) where TEntity : class
        {
            var dbSet = DbContext.Set<TEntity>();

            dbSet.RemoveRange(listToRemove);
        }

        public static void AddRange<TEntity>(List<TEntity> listToAdd) where TEntity : class
        {
            var dbSet = DbContext.Set<TEntity>();

            dbSet.AddRange(listToAdd);
        }

        public static bool DeleteNoCommitEntity<TEntity>(TEntity entity, string message) where TEntity : class
        {
            return DbContext.DeleteNoCommitEntity(DbContext.Set<TEntity>(), entity, message);
        }

    }
}
