using OpenTK;

namespace ThreeD_ACW.GameCameras
{
    class MovingCamera : Camera
    {
        /// <summary>
        /// Creates a moveable camera
        /// </summary>
        /// <param name="pCameraPosition">The position of the camera</param>
        /// <param name="pLookingAt">The point that the camera is looking at</param>
        /// <param name="pUpDirection">The up direction of the camera</param>
        public MovingCamera(Vector3 pCameraPosition, Vector3 pLookingAt, Vector3 pUpDirection) : base(pCameraPosition, pLookingAt, pUpDirection)
        {
        }

        /// <summary>
        /// Moves the camera depending on the key pressed
        /// </summary>
        /// <param name="e"></param>
        public override void MoveCamera(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f);
            }
            if (e.KeyChar == 's')
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, -0.05f);
            }
            if (e.KeyChar == 'a')
            {
                mView = mView * Matrix4.CreateRotationY(-0.025f);
            }
            if (e.KeyChar == 'd')
            {
                mView = mView * Matrix4.CreateRotationY(0.025f);
            }
            if (e.KeyChar == 'q')
            {
                mView = mView * Matrix4.CreateRotationZ(-0.025f);
            }
            if (e.KeyChar == 'e')
            {
                mView = mView * Matrix4.CreateRotationZ(0.025f);
            }
        }
    }
}
