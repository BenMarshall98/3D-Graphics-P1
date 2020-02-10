using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using ThreeD_ACW.GameAnimation;
using ThreeD_ACW.GameMaterials;
using ThreeD_ACW.GameTextures;
using ThreeD_ACW.Utility;

namespace ThreeD_ACW.GameModels
{
    class Model
    {
        private int[] mVBO_IDs = new int[2];
        private int mVAO_ID;
        private int mShaderID;
        private ModelUtility mModel;
        private Matrix4 mWorldPosition = Matrix4.Identity;
        private Matrix4 mAnimationPosition = Matrix4.Identity;
        private Texture mTexture;
        private AbstractAnimation mAnimation = null;
        private PrimitiveType mType;
        private List<Model> mModels = new List<Model>();
        private Material mMaterial;

        /// <summary>
        /// Creates a model
        /// </summary>
        /// <param name="pModel">The model data to use</param>
        /// <param name="pShaderID">The shader to use thoughtout the program</param>
        public Model(ModelUtility pModel, int pShaderID)
        {
            mShaderID = pShaderID;
            mModel = pModel;
            GL.UseProgram(mShaderID);
            int vPositionLocation = GL.GetAttribLocation(mShaderID, "vPosition");
            int vTexCoordLocation = GL.GetAttribLocation(mShaderID, "vTexCoord");
            int vNormalLocation = GL.GetAttribLocation(mShaderID, "vNormal");
            int vTangentLocation = GL.GetAttribLocation(mShaderID, "vTangent");
            int vBinormalLocation = GL.GetAttribLocation(mShaderID, "vBinormal");

            mVAO_ID = GL.GenVertexArray();
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);
            int size;

            GL.BindVertexArray(mVAO_ID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]); //Places the vertices data onto the GPU
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mModel.Vertices.Length * sizeof(float)), mModel.Vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if(mModel.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]); //Places the indices data onto the GPU
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mModel.Indices.Length * sizeof(float)), mModel.Indices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if(mModel.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.EnableVertexAttribArray(vTexCoordLocation);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.EnableVertexAttribArray(vTangentLocation);
            GL.EnableVertexAttribArray(vBinormalLocation);

            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 0);
            GL.VertexAttribPointer(vTexCoordLocation, 2, VertexAttribPointerType.Float, false, 14 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 14 * sizeof(float), 5 * sizeof(float));
            GL.VertexAttribPointer(vTangentLocation, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 8 * sizeof(float));
            GL.VertexAttribPointer(vBinormalLocation, 3, VertexAttribPointerType.Float, false, 14 * sizeof(float), 11 * sizeof(float));

            if(mModel.Indices.Length % 3 > 0) //Sets if the draw type should be a triangle fan or triangles
            {
                mType = PrimitiveType.TriangleFan;
            }
            else
            {
                mType = PrimitiveType.Triangles;
            }
        }

        /// <summary>
        /// Applies a transformation to the model matrix
        /// </summary>
        /// <param name="pTransformation">Matrix to be transformated by</param>
        public void ApplyTransformation(Matrix4 pTransformation)
        {
            mWorldPosition *= pTransformation;
        }

        /// <summary>
        /// Applies a texture to the model
        /// </summary>
        /// <param name="pTexture">The texture to be applied</param>
        public void ApplyTexture(Texture pTexture)
        {
            mTexture = pTexture;
        }

        /// <summary>
        /// Applies a animation to the model
        /// </summary>
        /// <param name="pAnimation">The animation to be applied</param>
        public void ApplyAnimation(AbstractAnimation pAnimation)
        {
            mAnimation = pAnimation;
        }

        /// <summary>
        /// Applies a material to the model
        /// </summary>
        /// <param name="pMaterial">The material to be applied</param>
        public void ApplyMaterial(Material pMaterial)
        {
            mMaterial = pMaterial;
        }

        /// <summary>
        /// Adds a model to inherit the matrix of 'this' model
        /// </summary>
        /// <param name="pModel">The model to add</param>
        public void AddModel(Model pModel)
        {
            mModels.Add(pModel);
        }

        /// <summary>
        /// Draws the model
        /// </summary>
        /// <param name="pWorldMatrix"></param>
        public void Draw(Matrix4 pWorldMatrix)
        {
            GL.UseProgram(mShaderID);
            int uModel = GL.GetUniformLocation(mShaderID, "uModel");
            Matrix4 fullPosition = mAnimationPosition * mWorldPosition * pWorldMatrix; //Calculates the full world matrix, with the inherited matrix, the model world matrix and the animation matrix.
            GL.UniformMatrix4(uModel, true, ref fullPosition);

            int uTextureType = GL.GetUniformLocation(mShaderID, "uTextureType");

            if (mTexture != null) //Applies the texture
            {
                mTexture.Draw(mShaderID);
            }

            mMaterial.UseMaterial(mShaderID); //Applies the material

            GL.BindVertexArray(mVAO_ID); //Draws the model
            GL.DrawElements(mType, mModel.Indices.Length, DrawElementsType.UnsignedInt, 0);

            for(int i = 0; i < mModels.Count; i++) //Calls draw on any models that this model contains
            {
                mModels[i].Draw(fullPosition);
            }
        }

        /// <summary>
        /// Applies the animation to the model
        /// </summary>
        public void Animate()
        {
            if(mAnimation != null)
            {
                mAnimation.Animation(ref mAnimationPosition);
            }

            for (int i = 0; i < mModels.Count; i++)
            {
                mModels[i].Animate();
            }
        }

        /// <summary>
        /// Deletes the model data from the GPU
        /// </summary>
        public void Delete()
        {
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArray(mVAO_ID);

            for (int i = 0; i < mModels.Count; i++)
            {
                mModels[i].Delete();
            }
        }
    }
}
