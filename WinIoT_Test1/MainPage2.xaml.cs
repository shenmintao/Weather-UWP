using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace WinIoT_Test1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage2 : Page
    {
        private static EdgeTransitionLocation edge = EdgeTransitionLocation.Right;
        daily_forecast[] DailyForecast = new daily_forecast[7];
        public MainPage2()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //这个e.Parameter是获取传递过来的参数，其实大家应该再次之前判断这个参数是否为null的，我偷懒了
            DailyForecast = (daily_forecast[])e.Parameter;
            Day1.dayWeather(DailyForecast[0]);
            Day2.dayWeather(DailyForecast[1]);
            Day3.dayWeather(DailyForecast[2]);
            Day4.dayWeather(DailyForecast[3]);
            Day5.dayWeather(DailyForecast[4]);
            Day6.dayWeather(DailyForecast[5]);
            Day7.dayWeather(DailyForecast[6]);
        }
        private new void ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var trans = e.Cumulative.Translation;
            double DeltaX = Math.Abs(trans.X);
            if (Math.Abs(trans.Y) * 1.2 < DeltaX && DeltaX > 50)
            {
                if (trans.X > 0)
                {
                    navAnimate.Edge = EdgeTransitionLocation.Right;
                    edge = navAnimate.Edge;
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
        }
    }
}
