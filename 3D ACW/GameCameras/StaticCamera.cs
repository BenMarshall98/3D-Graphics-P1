using OpenTK;

namespace ThreeD_ACW.GameCameras
{
    class StaticCamera : Camera
    {

        /// <summary>
        /// Creates a static camera
        /// </summary>
        /// <param name="pCameraPosition">The position of the camera</param>
        /// <param name="pLookingAt">The point that the camera is looking at</param>
        /// <param name="pUpDirection">The up direction of the camera</param>
        public StaticCamera(Vector3 pCameraPosition, Vector3 pLookingAt, Vector3 pUpDirection) : base(pCameraPosition, pLookingAt, pUpDirection)
        {
        }

        /// <summary>
        /// Moves the camera (does nothing with the static camera)
        /// </summary>
        /// <param name="e"></param>
        public override void MoveCamera(KeyPressEventArgs e)
        {
            return;
        }

    }
}
