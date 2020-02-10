using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using ThreeD_ACW.Utility;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeD_ACW.GameTextures
{
    class Texture
    {
        private int[] mTexture_IDs = new int[2];

        /// <summary>
        /// Deals with the textures used in the program
        /// </summary>
        /// <param name="pTextureLocation">The texture to be used</param>
        /// <param name="pNormalLocation">The normal map to be used</param>
        public Texture(string pTextureLocation, string pNormalLocation)
        {
            if(pNormalLocation == "") //Generate only one texture if normal texture is not defined
            {
                mTexture_IDs[0] = GL.GenTexture();
                MakeTexture(pTextureLocation, mTexture_IDs[0]);
            }
            else //Generate both textures
            {
                GL.GenTextures(mTexture_IDs.Length, mTexture_IDs);
                MakeTexture(pTextureLocation, mTexture_IDs[0]);
                MakeTexture(pNormalLocation, mTexture_IDs[1]);
            }
        }

        /// <summary>
        /// Turns the given image into a texture
        /// </summary>
        /// <param name="pFilepath">The filepath of the texture</param>
        /// <param name="pTexture_ID">The texture id in the opengl program</param>
        private void MakeTexture(string pFilepath, int pTexture_ID)
        {
            if (System.IO.File.Exists(pFilepath))
            {
                Bitmap TextureBitmap = new Bitmap(pFilepath);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData TextureData = TextureBitmap.LockBits(new Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(TextureUnit.Texture0 + pTexture_ID); //Offsets the active texture of the texture ID
                GL.BindTexture(TextureTarget.Texture2D, pTexture_ID);

                GL.TexImage2D(TextureTarget.Texture2D,
                    0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                    0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte, TextureData.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                TextureBitmap.UnlockBits(TextureData);
                TextureBitmap.Dispose();
            }
            else
            {
                throw new Exception("Could not find file " + pFilepath);
            }
        }

        /// <summary>
        /// Passes the textures to the given shader
        /// </summary>
        /// <param name="pShaderID">The shader to be used</param>
        public void Draw(int pShaderID)
        { 
            int uTextureLocation = GL.GetUniformLocation(pShaderID, "uTexture0");
            GL.Uniform1(uTextureLocation, mTexture_IDs[0]);

            int uNormalLocation = GL.GetUniformLocation(pShaderID, "uTexture1");
            GL.Uniform1(uNormalLocation, mTexture_IDs[1]);
        }

        /// <summary>
        /// Deletes the texture from the GPU
        /// </summary>
        public void Delete()
        {
            GL.DeleteTextures(mTexture_IDs.Length, mTexture_IDs);
        }
    }
}
