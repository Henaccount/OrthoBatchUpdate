
using Autodesk.AutoCAD.Runtime;


[assembly: CommandClass(typeof(BatchExports.Commands))]

namespace BatchExports
{
    public class Commands
    {
        #region Commands
        [CommandMethod("OrthoBatchUpdateAll", CommandFlags.Session)]
        public static void OrthoBatchUpdateAll()
        {
            Program.OrthoBatchUpdate(false, false);
        }

        [CommandMethod("OrthoBatchUpdateOnlyModified", CommandFlags.Session)]
        public static void OrthoBatchUpdateOnly()
        {
            Program.OrthoBatchUpdate(true, false);
        }

        [CommandMethod("OrthoBatchUpdateOnlyModifiedDebug", CommandFlags.Session)]
        public static void OrthoBatchUpdateOnlyDebug()
        {
            Program.OrthoBatchUpdate(true, true);
        }
        #endregion
    }
}
