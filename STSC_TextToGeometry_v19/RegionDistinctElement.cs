using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace STSC_TextToGeometry_v19
{
    internal class RegionDistinctElement : IDisposable
    {
        //Fields
        private readonly DBObjectCollection m_regParts;
        private bool m_isDisposed;



        //Constructor
        public RegionDistinctElement()
        {
            m_regParts = new DBObjectCollection();
        }

                //Destructor
        ~RegionDistinctElement()
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
            if (m_regParts != null) m_regParts.Dispose();

        }


        //Properties


        //Methods
        public void ProcessObjectCollection(DBObjectCollection DbObjColl)
        {
            InterimRegion(DbObjColl);
        }

        public void Reset()
        {
            if (m_regParts != null && m_regParts.Count > 0)
            {
                m_regParts.Clear(); 
            }  
        }
        

        public void CleanUp()
        {
            Reset();
            m_regParts.Dispose();
        }
        

        public Region CreateLetter()
        {
            Region singleComplexRegion;
            var count = m_regParts.Count - 1;
            if (count < 0) return null;
            singleComplexRegion = m_regParts[count] as Region;

            var initialExt = singleComplexRegion.GeometricExtents;
            if (count > 0)
            {
                
                for (var i = 0; i < count; i++)
                {
                    var subjectExt = initialExt;
                    var temp = m_regParts[i] as Region;
                    var objectExt = temp.GeometricExtents;
                    subjectExt.AddExtents(objectExt);
                    if (temp != null)
                    {
                        if (subjectExt == initialExt)//Operation to distiguish inside from outside
                        {
                            singleComplexRegion.BooleanOperation(BooleanOperationType.BoolSubtract, temp);
                        }
                        else
                        {
                            singleComplexRegion.BooleanOperation(BooleanOperationType.BoolUnite, temp);
                        }
                    }
                }

            }
            return singleComplexRegion; 
        }


        //Internal
        private void InterimRegion(DBObjectCollection DboColl)
        {
            DBObjectCollection tempReg;
            try
            {
                tempReg = Region.CreateFromCurves(DboColl);
                if (tempReg[0] != null) m_regParts.Add(tempReg[0]);
            }
            catch
            {
                LoopRepair(ref DboColl);
                try
                {
                    tempReg = Region.CreateFromCurves(DboColl);
                    if (tempReg[0] != null) m_regParts.Add(tempReg[0]);
                }
                catch
                {

                }
            }
        }

        private void LoopRepair(ref DBObjectCollection DBoCollWithSi)
        {
            var count = DBoCollWithSi.Count - 1;
            for (var i = 0; i < count; i++)
            {
                var p3dc = new Point3dCollection();
                var first = DBoCollWithSi[i] as Curve;
                var second = DBoCollWithSi[i + 1] as Curve;
                first.IntersectWith(second, Intersect.OnBothOperands, p3dc, new IntPtr(), new IntPtr());
                if (p3dc.Count == 2)
                {
                    if (p3dc[0] == first.EndPoint) p3dc.RemoveAt(0);
                    else p3dc.RemoveAt(1);
                    var firCol = first.GetSplitCurves(p3dc);
                    var secCol = second.GetSplitCurves(p3dc);
                    DBoCollWithSi.Remove(first);
                    DBoCollWithSi.Remove(second);
                    DBoCollWithSi.Add(firCol[0]);
                    DBoCollWithSi.Add(secCol[1]);
                    firCol[1].Dispose();
                    secCol[0].Dispose();
                    firCol.Dispose();
                    secCol.Dispose();
                }

            }

        }


    }
}
