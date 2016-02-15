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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WinIoT_Test1
{
    public sealed partial class DayWeather : UserControl
    {
        public DayWeather()
        {
            this.InitializeComponent();
        }
        public void dayWeather(daily_forecast Weathers)
        {
            Date.Text = Weathers.date;
            Weather.Text= Weathers.cond.txt_d;
            Temperature.Text = Weathers.tmp.min + " ℃～" + Weathers.tmp.max+ " ℃";
            WindDir.Text = Weathers.wind.dir;
            WindSc.Text = Weathers.wind.sc+"级";
            String Path = "ms-appx:///Assets/WeatherImage/" + Weathers.cond.code_d + ".png";
            Uri test = new Uri(Path);
            BitmapImage WeatherBitmapImage = new BitmapImage(test);
            image.Source = WeatherBitmapImage;
        }
    }
}
