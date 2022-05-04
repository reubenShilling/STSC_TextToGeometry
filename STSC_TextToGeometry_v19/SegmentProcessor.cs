using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DoubleCollection = Autodesk.AutoCAD.Geometry.DoubleCollection;

namespace STSC_TextToGeometry_v19
{
    internal class SegmentProcessor : IDisposable
    {
        //Fields
        private bool m_isValid = true;
        private Point2d m_pt; //Kept as 2d for polylines
        private Database m_db;
        private Entity m_dbEntity;
        private DoubleCollection m_dcw;
        private Point m_winPt;
        private KnotCollection m_kc = new KnotCollection();
        private bool m_figureClosed;
        private bool m_initial;
        private Matrix3d m_m3d;
        private DBObjectCollection m_dBoc;
        private bool m_isDisposed;


        //Constructor

        public SegmentProcessor()
        {
            m_db = HostApplicationServices.WorkingDatabase;
            InitializeCollections();
        }

        //Destructor
        ~SegmentProcessor()
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
            if (m_kc != null) m_kc.Dispose();
            if (m_dBoc != null) m_dBoc.Dispose();

        }



        //Properties
        public Point WinStPt
        {
            get { return m_winPt; }
            set { m_winPt = value; }
        }


        public bool IsValid
        {
            get { return m_isValid; }
        }

        public Point2d Pt
        {
            get { return m_pt; }
            set { m_pt = value; }
        }

        public bool FigureClosed
        {
            set { m_figureClosed = value; }
        }


        public DBObjectCollection DbObjColl
        {
            get { return m_dBoc; }
        }



        //Methods
        public void AddPathSegment(PathSegment PathSeg)
        {
            IdentifySegmentType(PathSeg);
        }


        public void Reset()
        {
            if (m_dBoc.Count > 0)
            {
                foreach (DBObject dbo in m_dBoc)
                {
                    dbo.Dispose();
                }
                m_dBoc.Clear();
            }
            

        }

        public void CleanUp()
        {
            Reset();
            m_dBoc.Dispose();
        }



        //Internal
        private void IdentifySegmentType(PathSegment PathSeg)
        {
            var typ = PathSeg.GetType();

            if (typ.IsAssignableFrom(typeof(PolyBezierSegment)))
            {
                NurbSpline(PathSeg, true, false);
                return;
            }

            if (typ.IsAssignableFrom(typeof(BezierSegment)))//May need to be further isolated
            {
                var bezs = PathSeg as BezierSegment;
                var pt = new Point[4];
                pt[0] = m_winPt;
                pt[1] = bezs.Point1;
                pt[2] = bezs.Point2;
                pt[3] = bezs.Point3;
                BezierUnit(pt);
                AddEnt();
                m_winPt = pt[3]; 
                return;
            }
            if (typ.IsAssignableFrom(typeof(PolyLineSegment)))
            {
                Pline(PathSeg);
                AddEnt();
                return;
            }
            if (typ.IsAssignableFrom(typeof(LineSegment)))
            {
                SingleLine(PathSeg);
                AddEnt();
                return;
            }

            if (typ.IsAssignableFrom(typeof(PolyQuadraticBezierSegment)))
            {
                NurbSpline(PathSeg, true, true);
                return;
            }
            if (typ.IsAssignableFrom(typeof(QuadraticBezierSegment)))
            {
                NurbSpline(PathSeg, false, true);
                return;
            }
            if (typ.IsAssignableFrom(typeof(ArcSegment)))
            {
                EllipseArc();
            }

        }


        private void NurbSpline(PathSegment PathSeg, bool IsComposite, bool IsCubic)
        {
            var bezs = PathSeg as PolyBezierSegment;
            var pc = bezs.Points;
            var count = pc.Count / 3;
            var pt = new Point[4];
            for (var j = 0; j < count; j++)
            {
                pt[0] = m_winPt;
                pt[1] = pc[(j * 3)];
                pt[2] = pc[(j * 3) + 1];
                pt[3] = pc[(j * 3) + 2];
                BezierUnit(pt);
                AddEnt();
                m_winPt = pt[3]; 
            }

        }


        private void Pline(PathSegment PathSeg)
        {
            var pls = PathSeg as PolyLineSegment;
            var pc = pls.Points;
            var count = pc.Count + 1;
            var pl = new Polyline(count);
            m_pt = new Point2d(m_winPt.X, m_winPt.Y);
            pl.AddVertexAt(0, m_pt, 0.0, 0.0, 0.0);
            for (var i = 1; i < count; i++)
            { 
                var swPt = pc[i-1];
                m_pt = new Point2d(swPt.X, swPt.Y);
                pl.AddVertexAt(i, m_pt, 0.0, 0.0, 0.0);
                
            }
            pl.Closed = m_figureClosed;
            m_dbEntity = pl;
            m_winPt = pc.Last(); 
            
        }

        private void SingleLine(PathSegment PathSeg)
        {
            var ls = PathSeg as LineSegment;
            var stpt = new Point3d(m_winPt.X, m_winPt.Y, 0.0);
            var swPt = ls.Point;
            var ndpt = new Point3d(swPt.X, swPt.Y, 0.0);
            m_winPt = swPt;
            var ln = new Line(stpt, ndpt);
            m_dbEntity = ln;
        }

        private void EllipseArc()
        {

        }

        private void BezierUnit(Point[] Pt) //Measure required for bizarre AutoCAD trait
        {
            var tol = new Tolerance();
            var degree = 3;
            Double per;
            var p3C = new Point3dCollection();

            for (var i = 0; i < 4; i++)
            {
                p3C.Add(new Point3d(Pt[i].X, Pt[i].Y, 0.0));  
            }

            var nc3d = new NurbCurve3d(degree, m_kc, p3C, false);
            m_kc = nc3d.Knots;
            var dblKnots = new Double[m_kc.Count];//Very Strange
            m_kc.CopyTo(dblKnots, 0);
            var knotOffset = dblKnots[0];
            for (var i = 0; i < m_kc.Count; i++)
            {
                dblKnots[i] -= knotOffset;
            }

            var dc = new DoubleCollection(dblKnots);
            var nc3dData = nc3d.DefinitionData;

            var spl = new Spline(nc3d.Degree, nc3d.IsRational, nc3d.IsClosed(), nc3d.IsPeriodic(out per),
                        nc3dData.ControlPoints, dc, nc3dData.Weights, tol.EqualPoint, tol.EqualVector);

            
            if (m_initial)//For anomalous AutoCAD handling of first spline creation
            {
                spl.SetControlPointAt(0, p3C[0]);
                spl.SetControlPointAt(1, p3C[1]);
                spl.SetControlPointAt(2, p3C[2]);
                spl.SetControlPointAt(3, p3C[3]);
                m_initial = false;
            }
             
             
            if (!spl.IsNull) m_dbEntity = spl;
        }


        private void InitializeCollections()
        {
            m_initial = true;
            m_dBoc = new DBObjectCollection();
            var pln = new Plane(new Point3d(0.0, 0.0, 0.0), new Vector3d(0.0, 1.0, 0.0));
            m_m3d = Matrix3d.Mirroring(pln);
            m_kc.Add(0.0);
            m_kc.Add(0.0);
            m_kc.Add(0.0);
            m_kc.Add(1.0);
            m_kc.Add(2.0);
            m_kc.Add(3.0);
            m_kc.Add(3.0);
            m_kc.Add(3.0);

            m_dcw = new DoubleCollection(4);
            m_dcw.Add(1.0);
            m_dcw.Add(1.0);
            m_dcw.Add(1.0);
            m_dcw.Add(1.0);

        }


        private void AddEnt()
        {
            if (m_dbEntity != null)
            {
                m_dbEntity.TransformBy(m_m3d);
                m_dBoc.Add(m_dbEntity);
            }
        }


    }

}
