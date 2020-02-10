using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.Utility;

namespace ThreeD_ACW.GameLighting
{
    class SpotLighting : Lighting
    {
        private Vector3 mDirection;
        private float mAngle;

        /// <summary>
        /// Creates a spot light
        /// </summary>
        /// <param name="pPosition">The position of the spot light</param>
        /// <param name="pColour">The colour of the light</param>
        /// <param name="pDirection">The direction the light is pointing at</param>
        /// <param name="pAngle">The cutoff angle of the light</param>
        /// <param name="pAttenuation">The attenuation of the light</param>
        public SpotLighting(Vector3 pPosition, Vector3 pColour, Vector3 pDirection, float pAngle, float pAttenuation) : base (pPosition, pColour, 1, pAttenuation)
        {
            mDirection = pDirection;
            mAngle = pAngle;
        }

        /// <summary>
        /// Places the light data onto the shader
        /// </summary>
        /// <param name="pArrayPosition"></param>
        /// <param name="pShader"></param>
        public override void UseLight(int pArrayPosition, ShaderUtility pShader)
        {
            base.UseLight(pArrayPosition, pShader);
            int uLightDirectionLocation = GL.GetUniformLocation(pShader.ShaderProgramID,
                "uLight[" + pArrayPosition + "].LightDirection");
            GL.Uniform4(uLightDirectionLocation, new Vector4(mDirection, 1));

            int uAngleLocation = GL.GetUniformLocation(pShader.ShaderProgramID,
                "uLight[" + pArrayPosition + "].Angle");
            GL.Uniform1(uAngleLocation, mAngle);
        }
    }
}
