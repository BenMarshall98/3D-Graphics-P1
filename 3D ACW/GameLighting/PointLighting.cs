using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeD_ACW.GameLighting
{
    class PointLighting : Lighting
    {
        /// <summary>
        /// Creates a point light
        /// </summary>
        /// <param name="pPosition">The position of the point light</param>
        /// <param name="pColour">The colour of the point light</param>
        /// <param name="pAttenuation">The attenuation of the point light</param>
        public PointLighting(Vector3 pPosition, Vector3 pColour, float pAttenuation) : base (pPosition, pColour, 0, pAttenuation)
        {

        }
    }
}
