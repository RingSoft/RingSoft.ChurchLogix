using RingSoft.App.Library;

namespace RingSoft.ChurchLogix.MasterData
{
    public class MasterDbContext
    {
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
    }
}
