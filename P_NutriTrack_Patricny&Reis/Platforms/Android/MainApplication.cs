using Android.App;
using Android.Runtime;

// assembly en dehors du namespace car il s applique a tout le projet compilé et est déclaré après les using 
[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]

namespace P_NutriTrack_Patricny_Reis
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}