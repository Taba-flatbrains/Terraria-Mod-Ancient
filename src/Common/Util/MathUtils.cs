using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ancient.src.Common.Util
{
    internal static class MathUtils
    {
        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return MathF.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
        }


        // credit: https://stackoverflow.com/questions/69245724/rotate-a-vector-around-an-axis-in-3d-space
        public static Vector3 RotateVector(Vector3 vector, Vector3 axis, float angle)
        {
            Vector3 vxp = Vector3.Cross(axis, vector);
            Vector3 vxvxp = Vector3.Cross(axis, vxp);
            return vector + MathF.Sin(angle) * vxp + (1 - MathF.Cos(angle)) * vxvxp;
        }

        /// <summary>
        /// Rotates a vector about a point in space.
        /// </summary>
        /// <param name="vector">The vector to be rotated.</param>
        /// <param name="pivot">The pivot point.</param>
        /// <param name="axis">The rotation axis.</param>
        /// <param name="angle">The rotation angle.</param>
        /// <returns>The rotated vector</returns>
        public static Vector3 RotateVectorAboutPoint(Vector3 vector, Vector3 pivot, Vector3 axis, float angle)
        {
            return pivot + RotateVector(vector - pivot, axis, angle);
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="limit">Saturation / highest value when x -> inf</param>
        /// <param name="midpoint">x value for half of saturation</param>
        /// <param name="x">x value input for function</param>
        /// <param name="k">steepness</param>
        /// <returns>y value</returns>
        public static float LogisticFunction(float x, float limit, float midpoint, float k = 0.03f) 
        {
            return (limit / (1 + limit * MathF.Exp(-k * (x - midpoint)))) - (limit / (1 + limit * MathF.Exp(-k * (- midpoint)))); // not actually a logistic function, only useful for animation
        }
    }
}
