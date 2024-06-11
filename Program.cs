

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.ProcessPower.DataObjects;
using Autodesk.ProcessPower.ProjectManager;
using System.Collections.Generic;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using PlantApp = Autodesk.ProcessPower.PlantInstance.PlantApplication;

namespace BatchExports
{
    class Program
    {
        //v0


        public static async void OrthoBatchUpdate(bool checkdates, bool mydebug)
        {
            string errorstack = "";
            object silentSetting = Application.GetSystemVariable("PLANTORTHOSILENTUPDATE");

            try
            {
                errorstack += "PLANTORTHOSILENTUPDATE=1\n";                
                Application.SetSystemVariable("PLANTORTHOSILENTUPDATE", 1);

                //associative list for 3d dwg lastmodified timestamps:
                IDictionary<string, System.DateTime> dictLastMod = new Dictionary<string, System.DateTime>();
                if (checkdates)
                {
                    errorstack += "\nchecking ortho file timestamp against used 3d files, if ortho older than a 3d file, then updating all views. Creating full list of 3d files: ...";
                    Project prj3d = PlantApp.CurrentProject.ProjectParts["Piping"];
                    System.Collections.Generic.List<PnPProjectDrawing> dwg3dList = prj3d.GetPnPDrawingFiles();
                    foreach (PnPProjectDrawing dwg3d in dwg3dList)
                    {
                        dictLastMod[dwg3d.DrawingGuid] = System.IO.File.GetLastWriteTime(dwg3d.ResolvedFilePath);
                        errorstack += "\n dwg3d.DrawingGuid: " + dwg3d.DrawingGuid + " GetLastWriteTime: " + System.IO.File.GetLastWriteTime(dwg3d.ResolvedFilePath) +  "\n";
                    }
                    //end associative list
                }

                Project prj = PlantApp.CurrentProject.ProjectParts["Ortho"];
                System.Collections.Generic.List<PnPProjectDrawing> orthoDwgList = prj.GetPnPDrawingFiles();

                foreach (PnPProjectDrawing orthoDwg in orthoDwgList)
                {
                    bool needsrefresh = false;
                    errorstack += orthoDwg.ResolvedFilePath + "\n";
                    var orthoLastModified = System.IO.File.GetLastWriteTime(orthoDwg.ResolvedFilePath);
                    errorstack += "\n ortholastModified: " + orthoLastModified + "\n";

                    Document docToWorkOn = null;
                    try
                    {
                        

                        docToWorkOn = AcadApp.DocumentManager.Open(orthoDwg.ResolvedFilePath, false);

                        AcadApp.DocumentManager.MdiActiveDocument = docToWorkOn;

                        if (checkdates)
                        {
                            //changed or not? find all related 3dfile guids -> datetimes, check if one of those datetimes is later than orthoLastModified var
                            PromptSelectionResult selResult = AcadApp.DocumentManager.MdiActiveDocument.Editor.SelectAll();
                            SelectionSet selset = selResult.Value;
                            ObjectId[] objIds = selset.GetObjectIds();

                            using (Transaction tr = AcadApp.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                            {
                                foreach (ObjectId objId in objIds)
                                {
                                    Autodesk.ProcessPower.Drawings2d.MdObjectId orthoId =
                                    Autodesk.ProcessPower.Drawings2d.PnPDwg2dUtil.GetModelObjectId(objId, objId.Database);
                                    string dwgGUID = orthoId.DwgGUID;
                                    if(!string.IsNullOrEmpty(dwgGUID)) errorstack += "\nextracted dwguid: " + dwgGUID;
                                    if (!string.IsNullOrEmpty(dwgGUID) && dictLastMod.ContainsKey(dwgGUID))
                                    {
                                        if (System.DateTime.Compare(orthoLastModified, dictLastMod[dwgGUID]) < 0)
                                        {
                                            needsrefresh = true;
                                            errorstack += "\nneeds refresh";
                                            break;
                                        }
                                    }
                                }
                                tr.Commit();
                            }
                            //end changed or not*/
                        }

                        if (needsrefresh || !checkdates)
                        {
                            using (docToWorkOn.LockDocument())
                            {
                                await AcadApp.DocumentManager.ExecuteInCommandContextAsync(async (obj) => { await AcadApp.DocumentManager.MdiActiveDocument.Editor.CommandAsync("PLANTORTHOUPDATE", "ALL"); }, null);
                            }

                            docToWorkOn.CloseAndSave(orthoDwg.ResolvedFilePath);
                        }
                        else docToWorkOn.CloseAndDiscard();

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
                errorstack += "\nsetting back PLANTORTHOSILENTUPDATE\nscript finished\n";
                Application.SetSystemVariable("PLANTORTHOSILENTUPDATE", silentSetting);
                //Application.SetSystemVariable("PLANTORTHOSILENTUPDATE", 0);
                if(mydebug)
                    AcadApp.DocumentManager.MdiActiveDocument.Editor.WriteMessage(errorstack);
                else
                    AcadApp.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nscript finished");
            }

        }


    }
}

