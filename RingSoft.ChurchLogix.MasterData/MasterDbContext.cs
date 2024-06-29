using Microsoft.EntityFrameworkCore;
using RingSoft.App.Library;
using RingSoft.DbLookup.EfCore;

namespace RingSoft.ChurchLogix.MasterData
{
    public class MasterDbContext : DbContext
    {
        public virtual DbSet<Church> Churches { get; set; }
        public static string ProgramDataFolder
        {
            get
            {
#if DEBUG
                return RingSoftAppGlobals.AssemblyDirectory;
#else
                return
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\RingSoft\\ChurchLogix\\";
#endif
            }
        }
        public static string MasterFilePath => $"{ProgramDataFolder}{MasterFileName}";

        public const string MasterFileName = "MasterDb.sqlite";
        public const string DemoDataFileName = "DemoData.sqlite";

        public static void ConnectToMaster()
        {
            if (!Directory.Exists(ProgramDataFolder))
                Directory.CreateDirectory(ProgramDataFolder);

            var firstTime = !File.Exists(MasterFilePath);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source={MasterFilePath}");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DbConstants.ConstantGenerator = new SqliteDbConstants();
            modelBuilder.Entity<Church>(entity =>
            {
                entity.HasKey(hk => hk.Id);

                entity.Property(p => p.Id).HasColumnType(DbConstants.IntegerColumnType);

                entity.Property(p => p.Name).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.FilePath).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.FileName).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.IsDefault).HasColumnType(DbConstants.BoolColumnType);

                entity.Property(p => p.Platform).HasColumnType(DbConstants.ByteColumnType);

                entity.Property(p => p.Server).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.Database).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.AuthenticationType).HasColumnType(DbConstants.ByteColumnType);

                entity.Property(p => p.Username).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.Password).HasColumnType(DbConstants.StringColumnType);

                entity.Property(p => p.DefaultUser).HasColumnType(DbConstants.IntegerColumnType);

                entity.Property(p => p.MigrateDb).HasColumnType(DbConstants.BoolColumnType);

            });

            base.OnModelCreating(modelBuilder);
        }

        public static IEnumerable<Church> GetChurches()
        {
            var context = new MasterDbContext();
            return context.Churches.OrderBy(p => p.Name);
        }

        public static Church GetDefaultChurch()
        {
            var context = new MasterDbContext();
            return context.Churches.FirstOrDefault(f => f.IsDefault);
        }

        public static bool SaveOrganization(Church church)
        {
            var context = new MasterDbContext();
            return context.SaveEntity(context.Churches, church, "Saving Church");
        }

        public static bool DeleteChurch(int churchId)
        {
            var context = new MasterDbContext();
            var church = context.Churches.FirstOrDefault(f => f.Id == churchId);
            return context.DeleteEntity(context.Churches, church, "Deleting Church");
        }
    }
}
