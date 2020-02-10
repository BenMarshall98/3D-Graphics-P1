using OpenTK;
using OpenTK.Graphics.OpenGL;
using ThreeD_ACW.Utility;

namespace ThreeD_ACW.GameCameras
{
    abstract class Camera
    {
        protected Matrix4 mView;

        /// <summary>
        /// Creates a camera, either static or moveable
        /// </summary>
        /// <param name="pCameraPosition">The position of the camera</param>
        /// <param name="pLookingAt">The point that the camera is looking at</param>
        /// <param name="pUpDirection">The up direction of the camera</param>
        public Camera(Vector3 pCameraPosition, Vector3 pLookingAt, Vector3 pUpDirection)
        {
            Vector3 zaxis = pLookingAt - pCameraPosition; //Calculate the x, y, and z axis
            zaxis.Normalize();
            Vector3 xaxis = Vector3.Cross(pUpDirection, zaxis);
            xaxis.Normalize();
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            float x = Vector3.Dot(xaxis, pCameraPosition);
            float y = Vector3.Dot(yaxis, pCameraPosition);
            float z = Vector3.Dot(zaxis, pCameraPosition);

            Vector3 lookAt = new Vector3(-x, -y, -z);

            //Makes the view matrix for the camera
            mView = new Matrix4(
                new Vector4(xaxis, 0),
                new Vector4(yaxis, 0),
                new Vector4(zaxis, 0),
                new Vector4(lookAt, 1));
        }

        /// <summary>
        /// The movement controls for the camera
        /// </summary>
        /// <param name="e"></param>
        public abstract void MoveCamera(KeyPressEventArgs e);

        /// <summary>
        /// Places the camera data onto the shader
        /// </summary>
        /// <param name="mShader">The shader to be used</param>
        public void UseCamera(ShaderUtility mShader)
        {
            GL.UseProgram(mShader.ShaderProgramID);
            int uViewLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uViewLocation, true, ref mView);

            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, new Vector4(mView.ExtractTranslation(), 1));
        }
    }
}
