using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.GameFramebuffer;
using ThreeD_ACW.GameModels;
using ThreeD_ACW.Utility;

namespace ThreeD_ACW.GameLighting
{
    abstract class Lighting
    {
        protected Vector3 mPosition;
        protected Vector3 mColour;
        protected int mType;
        protected float mAttenuation;

        /// <summary>
        /// Creates a light
        /// </summary>
        /// <param name="pPosition">The position of the light</param>
        /// <param name="pColour">The colour of the light</param>
        /// <param name="pType">The type of the light (0 is point, 1 is spotlight)</param>
        /// <param name="pAttenuation">The attenuation of the light</param>
        public Lighting(Vector3 pPosition, Vector3 pColour, int pType, float pAttenuation)
        {
            mPosition = pPosition;
            mColour = pColour;
            mType = pType;
            mAttenuation = pAttenuation;
        }

        /// <summary>
        /// Places the light data on ot the shader
        /// </summary>
        /// <param name="pArrayPosition">The light number (in array) to place the data at</param>
        /// <param name="pShader">The shader to use</param>
        public virtual void UseLight(int pArrayPosition, ShaderUtility pShader)
        {
            GL.UseProgram(pShader.ShaderProgramID);

            int uTypeLocation = GL.GetUniformLocation(pShader.ShaderProgramID,
                "uLight[" + pArrayPosition + "].Type");
            GL.Uniform1(uTypeLocation, mType);

            int uLightPositionLocation = GL.GetUniformLocation(pShader.ShaderProgramID,
                "uLight[" + pArrayPosition + "].LightPosition");
            GL.Uniform4(uLightPositionLocation, new Vector4(mPosition, 1));

            int uColourLocation = GL.GetUniformLocation(pShader.ShaderProgramID,
                "uLight[" + pArrayPosition + "].Colour");
            GL.Uniform4(uColourLocation, new Vector4(mColour, 1));

            int uAttenuationLocation = GL.GetUniformLocation(pShader.ShaderProgramID,
                "uLight[" + pArrayPosition + "].Attenuation");
            GL.Uniform1(uAttenuationLocation, mAttenuation);
        }
    }
}
