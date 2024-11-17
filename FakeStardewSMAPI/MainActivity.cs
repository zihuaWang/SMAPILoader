using Android.Content.PM;

namespace FakeStardewSMAPI
{
    [Activity(
     MainLauncher = true, AlwaysRetainTaskState = true,
     LaunchMode = LaunchMode.SingleInstance, ScreenOrientation = ScreenOrientation.SensorLandscape,
     ConfigurationChanges = (ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden
         | ConfigChanges.Orientation | ConfigChanges.ScreenLayout
         | ConfigChanges.ScreenSize | ConfigChanges.UiMode))]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
    }
}