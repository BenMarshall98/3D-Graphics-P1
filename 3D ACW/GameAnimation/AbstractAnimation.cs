using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ThreeD_ACW.GameAnimation
{
    abstract class AbstractAnimation
    {
        protected DateTime currentTime;

        /// <summary>
        /// Calculates the animation matrix for the given object
        /// </summary>
        /// <param name="currentMatrix">The current animation matrix</param>
        public abstract void Animation(ref Matrix4 currentMatrix);

        /// <summary>
        /// Checks if the total time passed in milliseconds is below a threshold (currentTime - now)
        /// </summary>
        /// <param name="pMilliseconds">Time passed</param>
        /// <returns>True, if animation should be paused, or false is animation can continue</returns>
        protected bool PauseAnimation(int pMilliseconds)
        {
            if(pMilliseconds < -50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
