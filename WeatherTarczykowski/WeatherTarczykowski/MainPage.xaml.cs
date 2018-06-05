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
using WeatherTarczykowski;
using Windows.UI.Popups;
using Windows.Storage;
using System.Collections.ObjectModel;
using System.ComponentModel;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace WeatherTarczykowski
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        const string firstPartDateOpenweathermap = "http://api.openweathermap.org/data/2.5/weather?q=";
        const string secoundPartDateOpenweathermap = "&lang=pl&units=metric&type=like&mode=xml&appid=5e79a54fbe78a6ba372226ca3e136581";
        CurrentWeather currentWeather = new CurrentWeather();
        ObservableCollection<string> listOfMostSearchedCity = new ObservableCollection<string>();
        Dictionary<string, int> dictionaryOfMostSearched = new Dictionary<string, int>();
        string rootPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        MediaElement mediaHandler = new MediaElement();
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;
            Application.Current.Suspending += Current_Suspending;
            textBlockInformationAboutApplication.Text = AboutApplication.aboutApplication;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var top3tmp = listOfMostSearchedCity.Take(3);
            ApplicationData.Current.LocalSettings.Values.Clear();
            int i = 3;
            foreach (string item in top3tmp)
            {
                ApplicationData.Current.LocalSettings.Values.Add(item, i--);
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ((PivotItem)pivot.Items[1]).IsEnabled = false;
            var folder = await StorageFolder.GetFolderFromPathAsync(rootPath + @"\Assets\Music");
            if(folder != null)
            {
                var file = await folder.GetFileAsync("Shakin Stevens - Snow is falling.mp3");
                if(file != null)
                {
                    var stream = await file.OpenReadAsync();
                    mediaHandler.SetSource(stream, file.ContentType);
                    mediaHandler.Volume = 100;
                    mediaHandler.Play();
                }
            }
            if(ApplicationData.Current.LocalSettings.Values.Count != 0)
            {
                dictionaryOfMostSearched.Clear();
                foreach (var item in ApplicationData.Current.LocalSettings.Values)
                {
                    dictionaryOfMostSearched.Add(item.Key, (int)item.Value);
                }
            }
            //dictionaryOfMostSearched.Add("Bydgoszcz", 1);
            var top3 = dictionaryOfMostSearched.OrderByDescending(pair => pair.Value).Take(3);
            listOfMostSearchedCity.Clear();
            foreach (var item in top3)
            {
                listOfMostSearchedCity.Add(item.Key);
            }
            listBoxMostSearched.ItemsSource = listOfMostSearchedCity;
        }

        private string ReturnWindDirectionName (string windDirectionCode)
        {
            string windDirectionName = "";
            switch(windDirectionCode)
            {
                case "N":
                    windDirectionName = "Połnocny";
                    break;
                case "E":
                    windDirectionName = "Wschodni";
                    break;
                case "S":
                    windDirectionName = "Połódniowy";
                    break;
                case "W":
                    windDirectionName = "Zachodni";
                    break;
                case "NE":
                    windDirectionName = "Połnocno-Wschodni";
                    break;
                case "NW":
                    windDirectionName = "Połnocno-Zachodni";
                    break;
                case "SE":
                    windDirectionName = "Połódniowo-Wschodni";
                    break;
                case "SW":
                    windDirectionName = "Połódniowo-Zachodni";
                    break;
                case "NNE":
                    windDirectionName = "Północno-Północno-Wschodni";
                    break;
                case "NNW":
                    windDirectionName = "Północno-Północno-Zachodni";
                    break;
                case "SSE":
                    windDirectionName = "Połódniowo-Połódniowo-Wschodni";
                    break;
                case "SSW":
                    windDirectionName = "Połódniowo-Połódniowo-Zachodni";
                    break;
            }
            return windDirectionName;
        }

        private string ReturnSunriseOrSunset(string x)
        {
            string sunriseorsunset = string.Format("{0}{1}.{2}{3}.{4}{5}{6}{7} {8}{9}:{10}{11}:{12}{13}", x[8], x[9], x[5], x[6], x[0], x[1], x[2], x[3], x[11], x[12], x[14], x[15], x[17], x[18]);
            return sunriseorsunset;
        }

        private async void buttonSearchWeather_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxCity.Text != "")
            {
                var serverOpenweathermap = new HttpClient();
                string date = "";
                try
                {
                    date = await serverOpenweathermap.GetStringAsync(new Uri(firstPartDateOpenweathermap + textBoxCity.Text + secoundPartDateOpenweathermap));
                }
                catch (Exception)
                {
                    var messagedialog = new MessageDialog("Nie znaleziono wyników dla podanego miasta");
                    messagedialog.Title = "Brak wyników";
                    messagedialog.Commands.Add(new UICommand { Label = "Ok"});
                    var res = await messagedialog.ShowAsync();
                }
                if (date != "")
                {
                    if(dictionaryOfMostSearched.ContainsKey(textBoxCity.Text))
                    {
                        dictionaryOfMostSearched[textBoxCity.Text]++;
                        var top3 = dictionaryOfMostSearched.OrderByDescending(pair => pair.Value).Take(3);
                        listOfMostSearchedCity.Clear();
                        foreach (var item in top3)
                        {
                            listOfMostSearchedCity.Add(item.Key);
                        }
                    }
                    else
                    {
                        dictionaryOfMostSearched.Add(textBoxCity.Text, 1);
                        var top3 = dictionaryOfMostSearched.OrderByDescending(pair => pair.Value).Take(3);
                        listOfMostSearchedCity.Clear();
                        foreach (var item in top3)
                        {
                            listOfMostSearchedCity.Add(item.Key);
                        }
                    }
                    XDocument dateOfWeather = XDocument.Parse(date);
                    var tmpList = (from item in dateOfWeather.Descendants("current")
                                   select new CurrentWeather()
                                   {
                                       CityName = (item.Element("city").Attribute("name").Value),
                                       Temperature = "Temperatura: " + (item.Element("temperature").Attribute("value").Value) + "°C",
                                       Sunrise = "Wschód słońca: \n" + ReturnSunriseOrSunset((item.Element("city").Element("sun").Attribute("rise").Value)),
                                       Sunset = "Zachód słońca: \n" + ReturnSunriseOrSunset((item.Element("city").Element("sun").Attribute("set").Value)),
                                       Humidity = "Wilgotność: " + (item.Element("humidity").Attribute("value").Value) + "%",
                                       Pressure = (item.Element("pressure").Attribute("value").Value),
                                       WindSpeedValue = "Prędkość wiatru: " + (item.Element("wind").Element("speed").Attribute("value").Value) + " m/s",
                                       WindDirectionCode = "(" + (item.Element("wind").Element("direction").Attribute("code").Value) + ")",
                                       WindDirectionName = "Kierunek wiatru: " + ReturnWindDirectionName((item.Element("wind").Element("direction").Attribute("code").Value)),
                                       CloudsName = "Niebo: " + (item.Element("clouds").Attribute("name").Value)
                                   }).ToList();
                    CWeather = tmpList[0];
                }
            }
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            textBoxCity.Text = ((TextBlock)sender).Text;
        }
        public CurrentWeather CWeather
        {
            get { return currentWeather; }
            set
            {
                if (value != currentWeather)
                {
                    currentWeather = value;
                    OnPropertyChanged("CWeather");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
