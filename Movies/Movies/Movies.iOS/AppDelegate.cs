using FFImageLoading.Forms.Platform;
using Foundation;
using Prism;
using Prism.Ioc;
using Sharpnado.MaterialFrame.iOS;
using UIKit;

namespace Movies.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            iOSMaterialFrameRenderer.Init();
            CachedImageRenderer.Init();
            //Prevent linker to remove Xamanimation
            _ = new Xamanimation.FadeInAnimation();
            LoadApplication(new App(new IosInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    internal class IosInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}

