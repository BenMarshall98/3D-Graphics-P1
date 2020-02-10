using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.GameAnimation;

namespace ThreeD_ACW.GameAnimation
{
    class TeapotRightAnimation : AbstractAnimation
    {
        private int angle = 0;

        /// <summary>
        /// Calculates the animation movement for the right teapot
        /// </summary>
        /// <param name="currentMatrix">The current animation matrix</param>
        public override void Animation(ref Matrix4 currentMatrix)
        {
            DateTime now = DateTime.Now;

            int timePassed = (int)(currentTime - now).TotalMilliseconds;

            if (PauseAnimation(timePassed)) //Do nothing if time passed is too large, due to either low frame rate or paused
            {
                currentTime = now;
                return;
            }

            angle -= timePassed;

            float height = -((float)Math.Sin((float)angle / 1000) * 8) - 8; //Moves the teapot left to right and back again

            currentMatrix = Matrix4.CreateRotationY(-(float)angle / 500); //Rotates the teapot
            currentMatrix *= Matrix4.CreateTranslation(height, 0, 0);

            currentTime = now;
        }
    }
}
