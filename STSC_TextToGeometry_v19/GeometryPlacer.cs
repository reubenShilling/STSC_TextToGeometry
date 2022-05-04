using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace STSC_TextToGeometry_v19
{
    internal class GeometryPlacer
    {
        //Fields
        private readonly Entity m_ent;
        private Matrix3d m_moveMatrix;
        private readonly Matrix3d m_ucsMatrix;
        private readonly Matrix3d m_working;
        private readonly Editor m_ed;
        private readonly Vector3d m_worldCompensate;
        private readonly Point3d m_wcsOrigin;

        //Constructor
        public GeometryPlacer(Entity PlaceableObject)
        {
            m_ent = PlaceableObject;
            var doc = Application.DocumentManager.MdiActiveDocument;
            m_ed = doc.Editor;
            m_ucsMatrix = m_ed.CurrentUserCoordinateSystem;
            m_wcsOrigin = new Point3d().TransformBy(m_ucsMatrix);
            m_worldCompensate = m_wcsOrigin.GetAsVector().Negate();
            var initialMove = Matrix3d.Displacement(m_worldCompensate);
            m_working = initialMove.PostMultiplyBy(m_ucsMatrix);
            m_ent.TransformBy(m_working);
        }

        //Properties
        public Matrix3d MoveMatrix
        {
            get { return m_moveMatrix.PostMultiplyBy(m_working); }
        }


        //Methods
        public void PlaceEntity()
        {
            var pjig = new EntPlaceJig(m_ent, m_working);//Recently changed from m_ucsMatrix
            var res = m_ed.Drag(pjig);
            if (res.Status == PromptStatus.OK)
            {
                m_moveMatrix = pjig.MoveMat();    
            }
            //if (m_ent != null) m_ent.Dispose(); //This is a bit perplexing
        }


        private class EntPlaceJig : EntityJig
        {
            private Point3d m_insertPoint;

            private Point3d m_prev;
            //Matrix3d m_ucs;

            public EntPlaceJig(Entity ent, Matrix3d ucs)
                : base(ent)
            {
                m_prev = new Point3d();
                //m_ucs = ucs;
            }

            protected override SamplerStatus Sampler(JigPrompts jp)
            {
                var jppo = new JigPromptPointOptions("\nPlacement Location: ");
                jppo.UserInputControls = UserInputControls.Accept3dCoordinates;
                var ppr = jp.AcquirePoint(jppo);
                if (ppr.Status == PromptStatus.OK)
                {
                    if (m_prev == ppr.Value)
                    {
                        return SamplerStatus.NoChange;
                    }

                    m_insertPoint = ppr.Value;
                    return SamplerStatus.OK;
                }
                return SamplerStatus.Cancel;
            }



            protected override bool Update()
            {
                var trans = Matrix3d.Displacement(m_insertPoint - m_prev);
                Entity.TransformBy(trans);
                m_prev = m_insertPoint;
                return true;
            }

            public Entity GetEntity()
            {
                return Entity;
            }

            public Matrix3d MoveMat()
            {
                var temp = Matrix3d.Displacement(m_insertPoint.GetAsVector());
                return temp; 
            }
        }
    }
}
