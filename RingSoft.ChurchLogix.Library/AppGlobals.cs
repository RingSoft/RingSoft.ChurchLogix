using RingSoft.App.Library;
using RingSoft.ChurchLogix.MasterData;
using RingSoft.DbLookup;

namespace RingSoft.ChurchLogix.Library
{
    public class AppGlobals
    {
        public static void InitSettings()
        {
            RingSoftAppGlobals.AppTitle = "ChurchLogix";
            RingSoftAppGlobals.AppCopyright = "©2024 by Peter Ringering";
            RingSoftAppGlobals.PathToDownloadUpgrade = MasterDbContext.ProgramDataFolder;
            RingSoftAppGlobals.AppGuid = "cd59af5f-799d-4203-8bfa-f4fdbe35c49c";
            RingSoftAppGlobals.AppVersion = 252;
            SystemGlobals.ProgramDataFolder = MasterDbContext.ProgramDataFolder;
        }

    }
}
