using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace WeatherTarczykowski
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string dateOpenweathermap = "http://api.openweathermap.org/data/2.5/weather?q=Bydgoszcz&lang=pl&units=metric&type=like&mode=xml&appid=5e79a54fbe78a6ba372226ca3e136581";
        List<CurrentWeather> listOfCurrentWeather = new List<CurrentWeather>();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var serverOpenweathermap = new HttpClient();
            string date = "";
            try
            {
                date = await serverOpenweathermap.GetStringAsync(new Uri(dateOpenweathermap));
            }
            catch(Exception)
            {
                throw;
            }
            if (date != "")
            {
                XDocument dateOfWeather = XDocument.Parse(date);
                listOfCurrentWeather = (from item in dateOfWeather.Descendants("current")
                         select new CurrentWeather()
                         {
                             CityName = (item.Element("city").Attribute("name").Value),
                             Temperature = (item.Element("temperature").Attribute("value").Value),
                             Sunrise = (item.Element("city").Element("sun").Attribute("rise").Value),
                             Sunset = (item. Element("city").Element("sun").Attribute("set").Value),
                             Humidity = (item.Element("humidity").Attribute("value").Value),
                             Pressure = (item.Element("pressure").Attribute("value").Value),
                             WindSpeedValue = (item.Element("wind").Element("speed").Attribute("value").Value),
                             WindSpeedName = (item.Element("wind").Element("speed").Attribute("name").Value),
                             WindDirectionCode = (item.Element("wind").Element("direction").Attribute("code").Value),
                             WindDirectionName = (item.Element("wind").Element("direction").Attribute("name").Value),
                             CloudsName = (item.Element("clouds").Attribute("name").Value)
                         }).ToList();
            }
        }
    }
}
