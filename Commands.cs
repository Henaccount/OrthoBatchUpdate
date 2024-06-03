
using Autodesk.AutoCAD.Runtime;


[assembly: CommandClass(typeof(BatchExports.Commands))]

namespace BatchExports
{
    public class Commands
    {
        #region Commands
        [CommandMethod("OrthoBatchUpdate", CommandFlags.Session)]
        public static void OrthoBatchUpdate()
        {
            Program.OrthoBatchUpdate("Ortho");
        }

        #endregion
    }
}
