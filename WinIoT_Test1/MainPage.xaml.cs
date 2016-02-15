using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Text;
using Windows.Networking.Connectivity;
using Windows.Networking;
using Windows.Web.Http.Filters;
using System.Xml;
using Windows.Data.Json;
using Windows.Storage;
using System.Runtime.Serialization.Json;
using Windows.Storage.FileProperties;
using System.Runtime.InteropServices;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace WinIoT_Test1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    ///
        public class Astro
        {
            public string sr { get; set; }  //日出时间
            public string ss { get; set; }  //日落时间
        }
        public class Cond
        {
            public string code_d { get; set; }  //白天天气代码
            public string code_n { get; set; }  //夜间天气代码
            public string txt_d { get; set; }   //白天天气描述
            public string txt_n { get; set; }   //夜间天气描述        
        }
        public class Tmp
        {
            public string max { get; set; } //最高温度(摄氏度)
            public string min { get; set; } //最低温度(摄氏度)
        }
        public class Wind
        {
            public string deg { get; set; } //风向(角度)
            public string dir { get; set; } //风向(方向)
            public string sc { get; set; }  //风力等级
            public string spd { get; set; } //风速(Kmph)
        }
        public class daily_forecast
        {
            public Astro astro { get; set; }
            public Cond cond { get; set; }
            public string date { get; set; }    //当地日期
            public string hum { get; set; } //湿度(%)
            public string pcpn { get; set; }    //降雨量(mm)
            public string pop { get; set; } //降水概率
            public string pres { get; set; }    //气压
            public Tmp tmp { get; set; }
            public string vis { get; set; } //能见度(km)
            public Wind wind { get; set; }
        }


        public class now_cond
        {
            public string code { get; set; } //天气代码
            public string txt { get; set; }  //天气描述
        }



        public class now
        {
            public now_cond cond { get; set; }  
            public string fl { get; set; }  //体感温度
            public string hum { get; set; } //湿度(%)
            public string pcpn { get; set; }    //降雨量(mm)
            public string pres { get; set; }    //气压
            public string tmp { get; set; } //当前温度(摄氏度)
            public string vis { get; set; } //能见度(km)
            public Wind wind { get; set; }  
        }



    public sealed partial class MainPage : Page
    {
        private static Uri imgUrl;
        private static DateTime Time;
        private DateTime dtNow;
        private static EdgeTransitionLocation edge = EdgeTransitionLocation.Left;
        daily_forecast[] DailyForecast = new daily_forecast[7];
        DispatcherTimer DisplayBingImage = new DispatcherTimer();
        DispatcherTimer DisplayNowTime = new DispatcherTimer();
        public MainPage()
        {
            getImage(null, null);
            GetNetTime();
            GetWeather();
            DisplayBingImage.Interval = new TimeSpan(2, 0, 0);
            DisplayBingImage.Tick += getImage;
            DisplayBingImage.Start();
            DisplayNowTime.Interval = new TimeSpan(0, 0, 1);
            DisplayNowTime.Tick += GetNowTime;
            DisplayNowTime.Start();
            this.InitializeComponent();
        }
        private void GetNowTime(object sender,object e)
        {
            String NowTime = Time.TimeOfDay.Hours.ToString()+":"+ Time.TimeOfDay.Minutes.ToString()+":"+Time.TimeOfDay.Seconds.ToString();
            String NowDay = Time.Date.ToString();
            this.TimeDisplay.Text = NowTime;
            this.DateDisplay.Text = NowDay;
            Time = Time.AddSeconds(1.0);
        }
        private async void getImage(object sender, object e)
        {
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            Uri BingImageXML = new Uri("http://cn.bing.com/HPImageArchive.aspx?idx=0&n=1");
            HttpClient LinkToBing = new HttpClient(filter);                            //加了filter才会刷新HttpClient的内容
            System.Xml.XmlDocument doc2 = new System.Xml.XmlDocument();
            var BingStream = await LinkToBing.GetInputStreamAsync(BingImageXML);
            doc2.Load(BingStream.AsStreamForRead());                                    //使用stream读取网页上的XML信息
            XmlNodeList lis=doc2.GetElementsByTagName("url");
            String str = lis[0].InnerText;
            imgUrl = new Uri("http://cn.bing.com" + str);
            BitmapImage BingBitmapImage = new BitmapImage(imgUrl);
            image.Source = BingBitmapImage;
        }

        private async void GetNetTime()
        {
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;    
            Uri BaiduTimeURL = new Uri("http://open.baidu.com/special/time/");
            HttpClient LinkToBaidu = new HttpClient(filter);                            //加了filter才会刷新HttpClient的内容
            String ResultHTML = await LinkToBaidu.GetStringAsync(BaiduTimeURL);
            LinkToBaidu.Dispose();
            int Start = ResultHTML.LastIndexOf("window.baidu_time");
            Start += 18;
            int End = ResultHTML.IndexOf(")",Start);
            String Result = ResultHTML.Substring(Start, End - Start);
            DateTime dtStart = DateTime.Parse("1970-01-01");
            dtNow = dtStart.AddTicks(System.Convert.ToInt64(Result + "0000"));
            Time = dtNow.ToLocalTime();
            Time = Time.AddSeconds(1.0);
        }
        private async void GetWeather()
        {

            String Cityid = "CN101010100";
            var filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
            Uri BaiduLocationURL = new Uri("http://api.map.baidu.com/location/ip?ak=NRO2ATGIWpMTgCoqlP5fjRpX&ip=&coor=bd09ll");
            HttpClient LinkToBaidu = new HttpClient(filter);                            //加了filter才会刷新HttpClient的内容
            String ResultIPJSON = await LinkToBaidu.GetStringAsync(BaiduLocationURL);
            JsonObject BaiduLocationJSONObject = JsonObject.Parse(ResultIPJSON);
            var UnzipIPJSON = BaiduLocationJSONObject.GetNamedObject("content");
            var UnzipAddress = UnzipIPJSON.GetNamedObject("address_detail");
            String City = UnzipAddress.GetNamedString("city");
            City=City.Replace("市", "");

            Uri CityList = new Uri("https://api.heweather.com/x3/citylist?search=allchina&key=50432a54f03a463085038da279aa72a3");
            HttpClient LinkToHeweather = new HttpClient(filter);                            //加了filter才会刷新HttpClient的内容
            String ResultCityListJSON = await LinkToHeweather.GetStringAsync(CityList);
            JsonObject CityListJSONObject = JsonObject.Parse(ResultCityListJSON);
            JsonArray UnzipCityListJSON = CityListJSONObject.GetNamedArray("city_info");
            for(int i = 0; i < UnzipCityListJSON.Count(); i++)
            {
                JsonObject CityName=JsonObject.Parse(UnzipCityListJSON[i].ToString());
                if(CityName.GetNamedString("city") == City)
                {
                    Cityid = CityName.GetNamedString("id");
                    break;
                }
            }

            String ResultCityWeather;
            var files= await ApplicationData.Current.LocalFolder.TryGetItemAsync("Weather.json");
            if (files != null)
            {
                BasicProperties filesType = await files.GetBasicPropertiesAsync();
                DateTime FileModTime = filesType.DateModified.DateTime;
                if(FileModTime <= DateTime.Now.AddHours(-2.0))
                {
                    Uri CityWeather = new Uri("https://api.heweather.com/x3/weather?cityid=" + Cityid + "&key=50432a54f03a463085038da279aa72a3");
                    HttpClient LinkToWeather = new HttpClient(filter);                            //加了filter才会刷新HttpClient的内容
                    ResultCityWeather = await LinkToWeather.GetStringAsync(CityWeather);
                    StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("Weather.json", CreationCollisionOption.ReplaceExisting);
                    await Windows.Storage.FileIO.WriteTextAsync(file, ResultCityWeather);
                }
                else
                {
                    Stream file= await Windows.Storage.ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("Weather.json");
                    StreamReader fileStream = new StreamReader(file);
                    ResultCityWeather = fileStream.ReadToEnd();
                }
            }
            else
            {
                Uri CityWeather = new Uri("https://api.heweather.com/x3/weather?cityid=" + Cityid + "&key=50432a54f03a463085038da279aa72a3");
                HttpClient LinkToWeather = new HttpClient(filter);                            //加了filter才会刷新HttpClient的内容
                ResultCityWeather = await LinkToWeather.GetStringAsync(CityWeather);
                StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("Weather.json", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(file, ResultCityWeather);
            }
            JsonObject CityWeatherJSONObject = JsonObject.Parse(ResultCityWeather);
            JsonArray UnzipCityWeatherJSON=CityWeatherJSONObject.GetNamedArray("HeWeather data service 3.0");
            JsonObject CityWeatherJSON = UnzipCityWeatherJSON.GetObjectAt(0);
            JsonArray DailyForecastsJSON = CityWeatherJSON.GetNamedArray("daily_forecast");
            JsonObject NowWeatherJSON = CityWeatherJSON.GetNamedObject("now");
            for(uint i=0;i<7;i++)
            {
                DataContractJsonSerializer dataContractSerializer = new DataContractJsonSerializer(typeof(daily_forecast));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(DailyForecastsJSON.GetObjectAt(i).ToString()));
                DailyForecast[i] = (daily_forecast)dataContractSerializer.ReadObject(ms);
            }
            DataContractJsonSerializer NowSerialier = new DataContractJsonSerializer(typeof(now));
            MemoryStream nows = new MemoryStream(Encoding.UTF8.GetBytes(NowWeatherJSON.GetObject().ToString()));
            now NowWeather = new now();
            NowWeather = (now)NowSerialier.ReadObject(nows);
            String Path = "ms-appx:///Assets/WeatherImage/" + NowWeather.cond.code + ".png";
            Uri test = new Uri(Path);
            BitmapImage NowWeatherBitmapImage = new BitmapImage(test);
            WeatherImage.Source = NowWeatherBitmapImage;
            Weather.Text = NowWeather.cond.txt;
            CityName.Text = City;
            Temperature.Text = NowWeather.tmp+ " ℃";
            
            //http://api.map.baidu.com/location/ip?ak=NRO2ATGIWpMTgCoqlP5fjRpX&ip=&coor=bd09ll  //IP定位
            //https://api.heweather.com/x3/citylist?search=allchina&key=50432a54f03a463085038da279aa72a3    //城市列表
            //https://api.heweather.com/x3/weather?cityid=CN101220101&key=50432a54f03a463085038da279aa72a3  //当地天气
        }

        private new void ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var trans = e.Cumulative.Translation;
            double DeltaX = Math.Abs(trans.X);
            if (Math.Abs(trans.Y) * 1.2 < DeltaX && DeltaX > 50)
            {
                if (trans.X < 0)
                {
                    navAnimate.Edge = EdgeTransitionLocation.Left;
                    edge = navAnimate.Edge;
                    DisplayBingImage.Stop();
                    DisplayNowTime.Stop();
                    this.Frame.Navigate(typeof(MainPage2),DailyForecast);
                }
            }
        }
    }
}
