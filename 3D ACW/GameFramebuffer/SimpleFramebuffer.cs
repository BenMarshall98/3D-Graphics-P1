using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.Utility;

namespace ThreeD_ACW.GameFramebuffer
{
    class Framebuffer
    {
        private int mFBO_ID;
        private int mRBO_ID;
        private int mTexture_ID;
        private int mVAO_ID;
        private int[] mVBO_IDs = new int[2];
        float[] mVertices;
        uint[] mIndices;
        int mWidth = 2048;
        int mHeight = 2048;

        // The following code has been adapted from https://learnopengl.com/Advanced-OpenGL/Framebuffers

        /// <summary>
        /// Creates a frame buffer for special effects
        /// </summary>
        /// <param name="pShader">The shader of the framebuffer (special effects shader)</param>
        public Framebuffer(ShaderUtility pShader)
        {
            GL.UseProgram(pShader.ShaderProgramID);
            mFBO_ID = GL.GenFramebuffer(); //Generate the framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFBO_ID);

            mTexture_ID = GL.GenTexture(); //Generate a blank texture
            GL.ActiveTexture(TextureUnit.Texture0 + mTexture_ID);
            GL.BindTexture(TextureTarget.Texture2D, mTexture_ID);

            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, mWidth, mHeight, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte,
                IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, mTexture_ID, 0); //Adds the texture to the frambuffer

            mRBO_ID = GL.GenRenderbuffer(); //Generate a renderbuffer, for depth testing. Note renderbuffer is write only
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mRBO_ID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth32fStencil8, mWidth, mHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, mRBO_ID); //Adds the render buffer to the framebuffer

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            mVertices = new float[] //Generate a basic shape for the framebuffer texture to be applied to
            {
                -1, -1, 0, 0,
                -1,  1, 0, 1,
                 1, -1, 1, 0,
                 1,  1, 1, 1
            };

            mIndices = new uint[]
            {
                2, 1, 0,
                1, 2, 3
            };

            int vPositionLocation = GL.GetAttribLocation(pShader.ShaderProgramID, "vPosition");
            int vTexCoordLocation = GL.GetAttribLocation(pShader.ShaderProgramID, "vTexCoord");

            mVAO_ID = GL.GenVertexArray();
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);
            int size;

            GL.BindVertexArray(mVAO_ID);

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]); //Place the vertices onto the GPU
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mVertices.Length * sizeof(float)), mVertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (mVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]); //Places the indices onto the GPU
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mIndices.Length * sizeof(float)), mIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if (mIndices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.EnableVertexAttribArray(vTexCoordLocation);

            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.VertexAttribPointer(vTexCoordLocation, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        /// <summary>
        /// Render the scene onto the framebuffer
        /// </summary>
        public void FirstPassStart()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFBO_ID); //Sets the program to render to the framebuffers texture
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Viewport(0, 0, mWidth, mHeight);
        }

        /// <summary>
        /// Use the texture generated on the framebuffer to create post processing effects
        /// </summary>
        /// <param name="pShader">The post processing shader</param>
        /// <param name="pClientWindow">The client window</param>
        /// <param name="pEffect">The effect to use</param>
        public void SecondPass(ShaderUtility pShader, Rectangle pClientWindow, int pEffect)
        {
            GL.UseProgram(pShader.ShaderProgramID);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);

            int uTextureLocation = GL.GetUniformLocation(pShader.ShaderProgramID, "uTexture");
            GL.Uniform1(uTextureLocation, mTexture_ID);

            //Calculate the difference between the client window and the framebuffer texture
            Vector2 clientScreenDif = new Vector2(pClientWindow.Width / (float)mWidth, pClientWindow.Height / (float)mHeight);
            int uClientScreenDifLocation = GL.GetUniformLocation(pShader.ShaderProgramID, "uClientScreenDif");
            GL.Uniform2(uClientScreenDifLocation, clientScreenDif);

            int uEffectLocation = GL.GetUniformLocation(pShader.ShaderProgramID, "uEffect");
            GL.Uniform1(uEffectLocation, pEffect);

            GL.BindVertexArray(mVAO_ID);
            GL.DrawElements(BeginMode.Triangles, mIndices.Length, DrawElementsType.UnsignedInt, 0);
        }

        /// <summary>
        /// Deletes the framebuffer from the GPU
        /// </summary>
        public void Delete()
        {
            GL.DeleteTexture(mTexture_ID);
            GL.DeleteRenderbuffer(mRBO_ID);
            GL.DeleteFramebuffer(mFBO_ID);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArray(mVAO_ID);
        }
    }
}
