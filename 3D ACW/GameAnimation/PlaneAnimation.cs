using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ThreeD_ACW.GameAnimation
{
    class PlaneAnimation : AbstractAnimation
    {
        private int angle = 0;

        /// <summary>
        /// Calculates the animation matrix for the plane
        /// </summary>
        /// <param name="currentMatrix">The current animation matrix</param>
        public override void Animation(ref Matrix4 currentMatrix)
        {
            DateTime now = DateTime.Now;

            int timePassed = (int)(currentTime - now).TotalMilliseconds;

            if (PauseAnimation(timePassed))
            {
                currentTime = now;
                return;
            }

            Vector3 currentTrans = currentMatrix.ExtractTranslation();
            currentMatrix *= Matrix4.CreateTranslation(-currentTrans);

            angle -= timePassed;

            float z = 2f * (float)Math.Cos((float)angle / 1000); //Rotates the plane around a point
            float x = 2f * (float)Math.Sin((float)angle / 1000);
            float y = 0.5f * (float)Math.Sin((float)angle / 1000); //Moves the plane up and down

            currentMatrix *= Matrix4.CreateTranslation(new Vector3(x, y, z));

            currentMatrix *= Matrix4.CreateRotationY(-(float)timePassed / 1000); //Points the plane in the right direction

            currentTime = now;
        }
    }
}
