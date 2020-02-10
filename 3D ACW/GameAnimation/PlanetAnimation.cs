using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeD_ACW.GameAnimation;

namespace ThreeD_ACW.GameAnimation
{
    class PlanetAnimation : AbstractAnimation
    {

        /// <summary>
        /// Calculates the animation matrix for the planets
        /// </summary>
        /// <param name="currentMatrix">The current animation matrix</param>
        public override void Animation(ref Matrix4 currentMatrix)
        {
            DateTime now = DateTime.Now;

            int timePassed = (int)(currentTime - now).TotalMilliseconds;

            if (PauseAnimation(timePassed)) //Do nothing if too much time has passed
            {
                currentTime = now;
                return;
            }

            currentMatrix *= Matrix4.CreateRotationY((float)timePassed / 1000); //Simply rotates the planes around

            currentTime = now;
        }
    }
}
