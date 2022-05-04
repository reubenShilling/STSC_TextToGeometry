// (C) Copyright 2012 by Sean Tessier 
//

using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using STSC_TextToGeometry_v19;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(MyCommands))]

namespace STSC_TextToGeometry_v19
{
    public class MyCommands
    {

        [CommandMethod("STSCGroup", "Text2Geom", "T2G", CommandFlags.Modal)]
        public void Text2Geom()
        {
            var frmt2G = new FrmTxt2GeomUi();

            if (Application.ShowModalDialog(frmt2G) == DialogResult.OK)
            {
                var ts = frmt2G.TextConverter;
                ts.Doit();
                frmt2G.Dispose();
            }
        }
    }
}
