using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Font = System.Drawing.Font;
using FontStyle = System.Windows.FontStyle;

//Author: 
//Sean Tessier
//Contributors:
//Daniel Marcotte - Form manipulation and general OOP guidance
//



namespace STSC_TextToGeometry_v19
{
    internal class TextConverter : IDisposable
    {
        //Fields
        private String m_textString;
        private Font m_textFont;
        private Double m_height;
        private List<Char> m_lCharacters = new List<char>();
        private FontStyle m_fontStyle;
        private FontWeight m_fontWeight;
        private GlyphTypeface m_gtf;
        private double[] m_widths;
        private DBObjectCollection m_dbObjText;
        private readonly Database m_db = HostApplicationServices.WorkingDatabase;
        private bool m_isValid;
        private readonly bool m_asRegion;
        private List<GlyphRun> m_glyphRunList;
        private RegionDistinctElement m_rde;
        private Typeface m_typeface;
        private bool m_isDisposed;

        
        //Constructors

        //Constructors
        public TextConverter(String TextString, Font TextFont, Double Height, bool AsReg)
        {
            m_textString = TextString;
            m_textFont = TextFont;
            m_height = Height;
            m_asRegion = AsReg;
        }


        //Destructor
        ~TextConverter()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (disposing)
                {
                }
                m_isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            if (m_dbObjText != null) m_dbObjText.Dispose();
        }

        //Properties

        public String TextString
        {
            get { return m_textString; }
            set { m_textString = value; }
        }


        public Font TextFont
        {
            get { return m_textFont; }
            set { m_textFont = value; }
        }


        public Double TextHeight
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public bool IsValid
        {
            get { return m_isValid; }
        }



        //Internal

        internal void Doit()
        {
            var count = m_textString.Length;
            CreateFontStyle();
            CreateTypeFace();
            m_dbObjText = new DBObjectCollection();
            Entity ent = null;
            using (var trans = m_db.TransactionManager.StartTransaction())
            {
                if (m_asRegion)
                {
                    m_widths = new double[1];
                    m_glyphRunList = new List<GlyphRun>(count);
                    CreateGeomDescriptAsReg(count);
                    using (m_rde = new RegionDistinctElement())
                    {
                        RegionProcess();
                        ent = CreateRegText();
                        m_rde.CleanUp();
                    }
                }
                else
                {
                    m_widths = new double[count];
                    m_glyphRunList = new List<GlyphRun>(1);
                    CreateGeomDescriptAsGeom(count);
                    GeometryProcess();
                    ent = new BlockReference(new Point3d(), InitializeBlk(m_db, trans));
                }
                try
                {
                    CreateEnt(ent);
                }
                catch
                {
                    if (ent != null) ent.Dispose();
                }
                finally
                {
                    trans.Commit();
                }
            }
        }

        private void CreateFontStyle()
        {
            if (m_textFont.Style == System.Drawing.FontStyle.Regular) m_fontStyle = FontStyles.Normal;
            if (m_textFont.Style == System.Drawing.FontStyle.Italic) m_fontStyle = FontStyles.Italic;
            if (m_textFont.Style == System.Drawing.FontStyle.Bold) m_fontWeight = FontWeight.FromOpenTypeWeight(700);
            if (m_textFont.Style != System.Drawing.FontStyle.Regular &&
               m_textFont.Style != System.Drawing.FontStyle.Italic &&
                m_textFont.Style != System.Drawing.FontStyle.Bold)
            {
                m_fontStyle = FontStyles.Italic;
                m_fontWeight = FontWeight.FromOpenTypeWeight(700);
            }

        }

        private void CreateTypeFace()
        {
            m_typeface = new Typeface(new FontFamily(m_textFont.Name),
                            m_fontStyle,
                            m_fontWeight,
                            new FontStretch());
            try
            {
                m_typeface.TryGetGlyphTypeface(out m_gtf);
            }
            catch
            {
                m_isValid = false;
            }

        }


        private void CreateGeomDescriptAsReg(int count)
        {

            double size = 10;
            double totalWidth = 0;
            var origin = new Point(0, 0);
            for (var n = 0; n < count; n++)
            {
                try
                {
                    var glyphIndex = m_gtf.CharacterToGlyphMap[m_textString[n]];
                    var glyphIndices = new ushort[1];
                    glyphIndices[0] = glyphIndex;
                    var width = m_gtf.AdvanceWidths[glyphIndex] * size;
                    m_widths[0] = width;
                    totalWidth += width;

                    m_glyphRunList.Add(
                                new GlyphRun(m_gtf,
                                0,
                                false,
                                size,
                                glyphIndices,
                                origin,
                                m_widths,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null));
                    origin = new Point(totalWidth, 0);
                }
                catch
                {
                    m_isValid = false;
                    return;
                }

            }
        }

        private void CreateGeomDescriptAsGeom(int count)
        {

            double size = 10;
            var glyphIndices = new ushort[count];
            var advanceWidths = new double[count];
            double totalWidth = 0;
            var origin = new Point(0, 0);
            for (var n = 0; n < count; n++)
            {
                try
                {
                    var glyphIndex = m_gtf.CharacterToGlyphMap[m_textString[n]];
                    glyphIndices[n] = glyphIndex;
                    var width = m_gtf.AdvanceWidths[glyphIndex] * size;
                    advanceWidths[n] = width;
                    totalWidth += width;

                    m_glyphRunList.Add(
                                new GlyphRun(m_gtf,
                                0,
                                false,
                                size,
                                glyphIndices,
                                origin,
                                advanceWidths,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null));
                }
                catch
                {
                    m_isValid = false;
                    return;
                }

            }
        }

        private void RegionProcess()
        {
            foreach (var gr in m_glyphRunList)//Text Scope
            {

                using (var sp = ProcessGlyphs(gr))
                {
                    if (sp == null) continue;
                    var reg = m_rde.CreateLetter();
                    if (reg != null) m_dbObjText.Add(reg);
                    sp.CleanUp();
                }
                m_rde.Reset();
            }
        }

        private void GeometryProcess()
        {
            foreach (var gr in m_glyphRunList)//Text Scope
            {
                var sp = ProcessGlyphs(gr);
                m_dbObjText = sp.DbObjColl; 
            }
        }

        private SegmentProcessor ProcessGlyphs(GlyphRun gr)
        {
            var sp = new SegmentProcessor();
            Geometry geom;
            PathGeometry pGeom;
            PathFigureCollection pFigureColl;
            geom = gr.BuildGeometry();
            pGeom = geom.GetOutlinedPathGeometry();
            pFigureColl = pGeom.Figures;
            var countFigure = pFigureColl.Count;
            if (countFigure < 1) return null; //For spaces in Text
            foreach (var pf in pFigureColl)//LetterScope
            {
                PathSegmentCollection pSegColl;
                pSegColl = pf.Segments;
                var countSeg = pSegColl.Count;
                sp.WinStPt = pf.StartPoint;

                foreach (var ps in pSegColl)//LoopScope
                {
                    if (countFigure == 1 && countSeg == 1) sp.FigureClosed = pf.IsClosed;
                    sp.AddPathSegment(ps);
                }
                if (m_asRegion)//to subreact internal geometry out of Region letters
                {
                    m_rde.ProcessObjectCollection(sp.DbObjColl);
                    sp.Reset();
                }
            }
            return sp;
        }

        private Entity CreateRegText()
        {
            var count = m_dbObjText.Count;
            if (count < 1)
            {
                m_isValid = false;
                return null;
            }
            var textReg = m_dbObjText[0] as Region;
            if (count > 1)
            {
                for (var i = 1; i < count; i++)
                {
                    var next = m_dbObjText[i] as Region;
                    textReg.BooleanOperation(BooleanOperationType.BoolUnite, next);
                }
            }
            return textReg;
        }


        private void CreateEnt(Entity ent)
        {

            var textBb = ent.GeometricExtents;
            var origin = new Point3d();
            var factor = m_height / origin.GetVectorTo(textBb.MaxPoint).Y;
            ent.TransformBy(Matrix3d.Scaling(factor, origin));

            using (var trans = m_db.TransactionManager.StartTransaction())
            {
                try
                {
                    var currSpace = trans.GetObject(m_db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                    ent.SetDatabaseDefaults();
                    currSpace.AppendEntity(ent);
                    trans.AddNewlyCreatedDBObject(ent, true);
                    var gp = new GeometryPlacer(ent);
                    gp.PlaceEntity();

                }
                catch
                {
                    m_isValid = false;
                }
                finally
                {
                    trans.Commit();
                }
            }
        }

        private ObjectId InitializeBlk(Database db, Transaction trans)
        {
            var blockName = UniqueName(db, trans);
            BlockTableRecord btrText;
            ObjectId oid;
            var bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            btrText = new BlockTableRecord();
            btrText.Name = blockName;
            bt.UpgradeOpen();
            bt.Add(btrText);
            trans.AddNewlyCreatedDBObject(btrText, true);
            foreach(DBObject dbo in m_dbObjText)
            {
                var crv = dbo as Curve;
                crv.SetDatabaseDefaults();
                btrText.AppendEntity(crv);
                trans.AddNewlyCreatedDBObject(crv, true);
            }
            oid = btrText.ObjectId;
            return oid;
        }

        private string UniqueName(Database db, Transaction trans)
        {
            string strTemp;
            var i = 0;
            var bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            do
            {
                strTemp = "T2G" + i;
                i++;
            } while (bt.Has(strTemp));
            return strTemp;
        }
    }
}
