namespace SharpConstraintLayout.Maui.Widget
{
    public enum ViewInfoType
    {
        X,
        Y,
        Width,
        Height,
        TranlateX,
        TranlateY,
        //Scale,
        ScaleX,
        ScaleY,
        Alpha,
        Rotation,
        RotationX,
        RotationY,
    }

    public class ViewInfo
    {
        double[] Info = new double[(int)ViewInfoType.RotationY+1];

        public double X
        {
            set => Info[(int)ViewInfoType.X] = value;
            get => Info[(int)ViewInfoType.X];
        }
        public double Y
        {
            set => Info[(int)ViewInfoType.Y] = value;
            get => Info[(int)ViewInfoType.Y];
        }
        public double Width
        {
            set => Info[(int)ViewInfoType.Width] = value;
            get => Info[(int)ViewInfoType.Width];
        }

        public double Height
        {
            set => Info[(int)ViewInfoType.Height] = value;
            get => Info[(int)ViewInfoType.Height];
        }

        public double TranlateX
        {
            set=> Info[(int)ViewInfoType.TranlateX] = value;
            get => Info[(int)ViewInfoType.TranlateX];
        }

        public double TranlateY
        {
            set => Info[(int)ViewInfoType.TranlateY] = value;
            get => Info[(int)ViewInfoType.TranlateY];
        }

        //public double Scale
        //{
        //    set => Info[(int)ViewInfoType.Scale] = value;
        //    get => Info[(int)ViewInfoType.Scale];
        //}

        public double ScaleX
        {
            set => Info[(int)ViewInfoType.ScaleX] = value;
            get => Info[(int)ViewInfoType.ScaleX];
        }

        public double ScaleY
        {
            set => Info[(int)ViewInfoType.ScaleY] = value;
            get => Info[(int)ViewInfoType.ScaleY];
        }

        public double Alpha
        {
            set => Info[(int)ViewInfoType.Alpha] = value;
            get => Info[(int)ViewInfoType.Alpha];
        }

        public double Rotation
        {
            set => Info[(int)ViewInfoType.Rotation] = value;
            get => Info[(int)ViewInfoType.Rotation];
        }

        public double RotationX
        {
            set => Info[(int)ViewInfoType.RotationX] = value;
            get => Info[(int)ViewInfoType.RotationX];
        }

        public double RotationY
        {
            set => Info[(int)ViewInfoType.RotationY] = value;
            get => Info[(int)ViewInfoType.RotationY];
        }

        public double GetInfo(ViewInfoType type)
        {
            return Info[(int)type];
        }

        public void SetInfo(ViewInfoType type, double value)
        {
            Info[(int)type] = value;
        }

        public override bool Equals(object? obj)
        {
            var finish = obj as ViewInfo;
            if (finish == null)
                return false;
            if (Math.Round(X, 3) != Math.Round(finish.X))
                return false;
            if (Math.Round(Y) != Math.Round(finish.Y))
                return false;
            if (Math.Round(Width) != Math.Round(finish.Width))
                return false;
            if (Math.Round(Height) != Math.Round(finish.Height))
                return false;
            if (Math.Round(TranlateX) != Math.Round(finish.TranlateX))
                return false;
            if (Math.Round(TranlateY) != Math.Round(finish.TranlateY))
                return false;
            //if (Math.Round(Scale) != Math.Round(finish.Scale))
            //    return false;
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
        /// <param name="ignoreInfoType">忽略的布局信息,此处差异将赋值为0,因此在制作动画时会被忽略</param>
        /// <returns></returns>
        public ViewInfo Diff(ViewInfo finish, params ViewInfoType[] ignoreInfoType)
        {
            var info =  new ViewInfo()
            {
                X = finish.X - X,
                Y = finish.Y - Y,
                Width = finish.Width - Width,
                Height = finish.Height - Height,
                TranlateX = finish.TranlateX - TranlateX,
                TranlateY = finish.TranlateY - TranlateY,
                //Scale = finish.Scale - Scale,
                ScaleX = finish.ScaleX - ScaleX,
                ScaleY = finish.ScaleY - ScaleY,
                Alpha = finish.Alpha - Alpha,
                Rotation = finish.Rotation - Rotation,
                RotationX = finish.RotationX - RotationX,
                RotationY = finish.RotationY - RotationY,
            };

            foreach (var type in ignoreInfoType)
            {
                info.SetInfo(type, 0);
            }

            return info;
        }
    }
}
