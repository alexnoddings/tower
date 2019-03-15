using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Tower.Application.DesignTimeServices;
using Tower.Core.Services;
using Tower.Services.BingBackground;
using Tower.Services.Spotify;
using Tower.Services.WmiBrightness;
using Tower.Services.Time;

namespace Tower.Application.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IBackgroundService, DesignTimeBackgroundService>();
                SimpleIoc.Default.Register<IBrightnessService, DesignTimeBrightnessService>();
                SimpleIoc.Default.Register<ISpotifyService, DesignTimeSpotifyService>();
                SimpleIoc.Default.Register<ITimeService, DesignTimeTimeService>();
            }
            else
            {
                SimpleIoc.Default.Register<IBackgroundService, BingBackgroundService>();
                SimpleIoc.Default.Register<IBrightnessService, WmiBrightnessService>();
                SimpleIoc.Default.Register<ISpotifyService, SpotifyWebService>();
                SimpleIoc.Default.Register<ITimeService, TimeService>();
            }

            SimpleIoc.Default.Register<MusicViewModel>();
            SimpleIoc.Default.Register<TimeViewModel>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public MusicViewModel Music => ServiceLocator.Current.GetInstance<MusicViewModel>();
        public TimeViewModel Time => ServiceLocator.Current.GetInstance<TimeViewModel>();

        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<MusicViewModel>().Cleanup();
            ServiceLocator.Current.GetInstance<TimeViewModel>().Cleanup();
        }
    }
}