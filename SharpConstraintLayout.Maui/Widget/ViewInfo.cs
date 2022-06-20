using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    public class ViewInfo
    {
        public double X;
        public double Y;
        public Size Size;
        public double TranlateX;
        public double TranlateY;
        public double Scale;
        public double ScaleX;
        public double ScaleY;
        public double Alpha;
        public double Rotation;
        public double RotationX;
        public double RotationY;

        public override bool Equals(object? obj)
        {
            var finish = obj as ViewInfo;
            if (finish == null)
                return false;
            if (Math.Round(X, 3) != Math.Round(finish.X))
                return false;
            if (Math.Round(Y) != Math.Round(finish.Y))
                return false;
            if (Math.Round(Size.Width) != Math.Round(finish.Size.Width))
                return false;
            if (Math.Round(Size.Height) != Math.Round(finish.Size.Height))
                return false;
            if (Math.Round(TranlateX) != Math.Round(finish.TranlateX))
                return false;
            if (Math.Round(TranlateY) != Math.Round(finish.TranlateY))
                return false;
            if (Math.Round(Scale) != Math.Round(finish.Scale))
                return false;
            if (Math.Round(ScaleX) != Math.Round(finish.ScaleX))
                return false;
            if (Math.Round(ScaleY) != Math.Round(finish.ScaleY))
                return false;
            if (Math.Round(Alpha) != Math.Round(finish.Alpha))
                return false;
            if (Math.Round(Rotation) != Math.Round(finish.Rotation))
                return false;
            if (Math.Round(RotationX) != Math.Round(finish.RotationX))
                return false;
            if (Math.Round(RotationY) != Math.Round(finish.RotationY))
                return false;

            return true;
        }

        /// <summary>
        /// 求差值
        /// </summary>
        /// <param name="finish"></param>
        /// <returns></returns>
        public ViewInfo Diff(ViewInfo finish)
        {
            return new ViewInfo()
            {
                X = finish.X - X,
                Y = finish.Y - Y,
                Size = new Size(finish.Size.Width - Size.Width, finish.Size.Height - Size.Height),
                TranlateX = finish.TranlateX - TranlateX,
                TranlateY = finish.TranlateY - TranlateY,
                Scale = finish.Scale - Scale,
                ScaleX = finish.ScaleX - ScaleX,
                ScaleY = finish.ScaleY - ScaleY,
                Alpha = finish.Alpha - Alpha,
                Rotation = finish.Rotation - Rotation,
                RotationX = finish.RotationX - RotationX,
                RotationY = finish.RotationY - RotationY
            };
        }
    }
}
