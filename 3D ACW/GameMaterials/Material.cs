using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.Utility;

namespace ThreeD_ACW.GameMaterials
{
    class Material
    {
        private Vector4 mAmbient;
        private Vector4 mDiffuse;
        private Vector4 mSpecular;
        private float mShininess;

        /// <summary>
        /// Creates a material for a object
        /// </summary>
        /// <param name="pAmbient">The ambient reflectivity of the material</param>
        /// <param name="pDiffuse">The diffuse reflectivity of the material</param>
        /// <param name="pSpecular">The specular reflectivity of the material</param>
        /// <param name="pShininess">The shininess of the material</param>
        public Material(Vector4 pAmbient, Vector4 pDiffuse, Vector4 pSpecular, float pShininess)
        {
            mAmbient = pAmbient;
            mDiffuse = pDiffuse;
            mSpecular = pSpecular;
            mShininess = pShininess;
        }

        /// <summary>
        /// Places the material data onto the shader program
        /// </summary>
        /// <param name="pShaderID"></param>
        public void UseMaterial(int pShaderID)
        {
            int uAmbientLocation = GL.GetUniformLocation(pShaderID, "uMaterial.Ambient");
            GL.Uniform4(uAmbientLocation, mAmbient);

            int uDiffuseLocation = GL.GetUniformLocation(pShaderID, "uMaterial.Diffuse");
            GL.Uniform4(uDiffuseLocation, mDiffuse);

            int uSpecularLocation = GL.GetUniformLocation(pShaderID, "uMaterial.Specular");
            GL.Uniform4(uSpecularLocation, mSpecular);

            int uShininessLocation = GL.GetUniformLocation(pShaderID, "uMaterial.Shininess");
            GL.Uniform1(uShininessLocation, mShininess);
        }
    }
}
