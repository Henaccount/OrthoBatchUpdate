

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.ProcessPower.ProjectManager;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using PlantApp = Autodesk.ProcessPower.PlantInstance.PlantApplication;

namespace BatchExports
{
    class Program
    {
        //v0


        public static async void OrthoBatchUpdate(string projectpart)
        {
            string errorstack = "";

            try
            {
                errorstack += "PLANTORTHOSILENTUPDATE=1\n";
                Application.SetSystemVariable("PLANTORTHOSILENTUPDATE", 1);

                Project prj = PlantApp.CurrentProject.ProjectParts[projectpart];

                System.Collections.Generic.List<PnPProjectDrawing> dwgList = prj.GetPnPDrawingFiles();


                foreach (PnPProjectDrawing dwg in dwgList)
                {
                    Document docToWorkOn = null;
                    try
                    {
                        errorstack += dwg.ResolvedFilePath + "\n";

                        docToWorkOn = AcadApp.DocumentManager.Open(dwg.ResolvedFilePath, false);


                        AcadApp.DocumentManager.MdiActiveDocument = docToWorkOn;


                        using (docToWorkOn.LockDocument())
                        {
                            await AcadApp.DocumentManager.ExecuteInCommandContextAsync(async (obj) => { await AcadApp.DocumentManager.MdiActiveDocument.Editor.CommandAsync("PLANTORTHOUPDATE", "ALL"); }, null);

                        }

                        docToWorkOn.CloseAndSave(dwg.ResolvedFilePath);
                    }
                    catch (System.Exception ex)
                    {

                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                        errorstack += trace.ToString() + "\n";
                        errorstack += "Line: " + trace.GetFrame(0).GetFileLineNumber() + "\n";
                        errorstack += "message: " + ex.Message + "\n";

                    }
                    finally
                    {
                        
                    }

                }


            }
            catch (System.Exception e)
            {

                errorstack += "message: " + e.Message + "\n";

            }
            finally
            {
                errorstack += "PLANTORTHOSILENTUPDATE=0\nscript finished\n";
                Application.SetSystemVariable("PLANTORTHOSILENTUPDATE", 0);
                AcadApp.DocumentManager.MdiActiveDocument.Editor.WriteMessage(errorstack);
            }

        }


    }
}

