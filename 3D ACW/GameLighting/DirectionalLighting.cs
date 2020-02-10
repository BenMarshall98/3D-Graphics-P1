using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeD_ACW.GameLighting
{
    class DirectionalLighting : Lighting
    {

        /// <summary>
        /// CReates a directional light
        /// </summary>
        /// <param name="pDirection">The direction the light is point towards</param>
        /// <param name="pColour">The colour of the light</param>
        public DirectionalLighting(Vector3 pDirection, Vector3 pColour) : base(pDirection, pColour, 2, 0)
        {
        }
    }
}
