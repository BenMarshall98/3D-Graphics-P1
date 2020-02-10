using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.GameAnimation;

namespace ThreeD_ACW.GameAnimation
{
    class TeapotLeftAnimation : AbstractAnimation
    {
        private int angle = 0;

        /// <summary>
        /// Calculates the animation matrix for the left teapot
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

            float height = (float)Math.Sin((float)angle / 1000) * 3; //Moves the teapot up and down

            if (height < 0) //If the teapot goes below orginal height do nothing
            {
                currentMatrix = Matrix4.CreateTranslation(0, 0, 0);
            }
            else {
                currentMatrix = Matrix4.CreateTranslation(0, height, 0);
            }

            currentMatrix *= Matrix4.CreateRotationY((float)angle / 500); //Rotate the teapot around

            currentTime = now;
        }
    }
}
