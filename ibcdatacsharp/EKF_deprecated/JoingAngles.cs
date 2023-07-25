using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Numerics;

namespace ibcdatacsharp.EKF_deprecated
{
    class JoingAngles 
    {
        private Transform qbase_trans;
        private Transform qmov_trans;
        private UnityEngine.Vector3 vector3;

        public Transform t1;
        public JoingAngles() 
        {
          
            
        }

        // Método para actualizar/convertir quaternions

        public double GetRelativeAngles(System.Numerics.Quaternion qbase, System.Numerics.Quaternion qmov)
        {
            qbase_trans.rotation = new UnityEngine.Quaternion(qbase.X, qbase.Y, qbase.Z, qbase.W);
            qmov_trans.rotation = new UnityEngine.Quaternion(qmov.X, qmov.Y, qmov.Z, qmov.W);

            
            double angle = UnityEngine.Quaternion.Angle(qbase_trans.rotation, qmov_trans.rotation);
            return angle;
        }
    }
}
