using Android.App;
using Android.Runtime;
using System;

namespace SharpConstraintLayout.Maui.Pure.Example
{
    [Application]
    public class MainApplication : Application
    {
        protected MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}