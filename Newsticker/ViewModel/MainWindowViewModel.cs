using HtmlAgilityPack;
using Newsticker.Commands;
using Newsticker.Models;
using Newsticker.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace Newsticker.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Commands
        public ICommand CloseCommand { get; set; }
        public ICommand RestoreCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand TagesschauCommand { get; set; }
        public ICommand FAZCommand { get; set; }
        public ICommand SZCommand { get; set; }
        public ICommand CNNCommand { get; set; }
        #endregion

        private ObservableCollection<ArticleModel> articles;
        public ObservableCollection<ArticleModel> Articles
        {
            get
            {
                if (this.articles == null)
                {
                    this.articles = new ObservableCollection<ArticleModel>();
                }
                return this.articles;
            }
            set
            {
                this.articles = value;
                this.OnPropertyChanged("Articles");
            }
        }
        private string restoreButton;
        public string RestoreButton
        {
            get
            {
                if (this.restoreButton == null)
                {
                    this.restoreButton = "/Images/maximize.png";
                }
                return this.restoreButton;
            }
            set
            {
                this.restoreButton = value;
                this.OnPropertyChanged("RestoreButton");
            }
        }
        private WeatherModel weather;
        public WeatherModel Weather
        {
            get
            {
                if (this.weather == null)
                {
                    this.weather = new WeatherModel();
                }
                return this.weather;
            }
            set
            {
                this.weather = value;
                this.OnPropertyChanged("Weather");
            }
        }

        public MainWindowViewModel()
        {
            this.CloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteClose);
            this.RestoreCommand = new RelayCommand(ExecuteRestoreCommand, CanExecuteRestore);
            this.MinimizeCommand = new RelayCommand(ExecuteMinimizeCommand, CanExecuteMinimize);
            this.TagesschauCommand = new RelayCommand(ExecuteTagesschauCommand, CanExecuteTagesschau);
            this.FAZCommand = new RelayCommand(ExecuteFAZCommand, CanExecuteFAZ);
            this.SZCommand = new RelayCommand(ExecuteSZCommand, CanExecuteSZ);
            this.CNNCommand = new RelayCommand(ExecuteCNNCommand, CanExecuteCNN);
        }



        private void ExecuteCNNCommand(object obj)
        {
            // TODO: Generell überall auf null und so abfragen den rss Feed
            using (XmlReader reader = XmlReader.Create("http://rss.cnn.com/rss/edition.rss"))
            {
                Articles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = item.Summary?.Text,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime
                    });
                }
            };
        }

        private void ExecuteSZCommand(object obj)
        {
            using (XmlReader reader = XmlReader.Create("https://rss.sueddeutsche.de/rss/Topthemen"))
            {
                Articles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Summary.Text);
                    string imageSource = string.Empty;
                    string summary = string.Empty;
                    // TODO: TryCatch Summary und ImageUrl trennen, irgendwa sgutes einfallen lassen
                    try
                    {
                        imageSource = doc.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
                        summary = doc.DocumentNode.Descendants("p").First().InnerText;
                    }
                    catch (Exception)
                    {
                        imageSource = feed.ImageUrl.AbsoluteUri;
                    }
                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = summary,
                        Link = item.Links[0].Uri.AbsoluteUri,
                        Date = item.PublishDate.DateTime,
                        ImageSource = imageSource
                    });
                }
            };
        }

        private void ExecuteFAZCommand(object obj)
        {
            using (XmlReader reader = XmlReader.Create("https://www.faz.net/rss/aktuell"))
            {
                Articles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Summary.Text);
                    string imageSource = string.Empty;
                    string summary = string.Empty;
                    try
                    {
                        imageSource = doc.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
                        summary = doc.DocumentNode.Descendants("p").First().InnerText;
                    }
                    catch (Exception)
                    {
                        imageSource = feed.ImageUrl.AbsoluteUri;
                    }
                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = summary,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime,
                        ImageSource = imageSource
                    });
                }
            };

            SyndicationFeed feed2;
            // TODO: Eigenständige Aktualisierung des Wetters
            using (XmlReader reader = XmlReader.Create("https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160007&obs=1&fc=1")) // Munich Weather
            {
                feed2 = SyndicationFeed.Load(reader);
            };
            HtmlDocument htmlCurrentWeather = new HtmlDocument();
            htmlCurrentWeather.LoadHtml(feed2.Items.First().Summary.Text);
            string temperature = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[2].InnerText, "[ \\n]", string.Empty);
            string feel = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[8].InnerText, "[ \\n]", string.Empty);
            string dewPoint = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[12].InnerText, "[ \\n]", string.Empty);
            string relativeHumidity = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[18].InnerText, "[ \\n]", string.Empty);
            Weather.Temperature = HttpUtility.HtmlDecode(temperature);
            Weather.Feel = HttpUtility.HtmlDecode(feel);
            Weather.DewPoint = HttpUtility.HtmlDecode(dewPoint);
            Weather.RelativeHumidity = HttpUtility.HtmlDecode(relativeHumidity);
            Weather.UpdateTime = feed2.LastUpdatedTime.LocalDateTime; // Im Notfall das hier, das Beste bis jetzt...
            //Weather.UpdateTime = Convert.ToDateTime(feed2.LastUpdatedTime.LocalDateTime, new CultureInfo("de-DE", false).DateTimeFormat);
            HtmlDocument htmlWeatherForecast = new HtmlDocument();
            htmlWeatherForecast.LoadHtml(feed2.Items.ElementAt(1).Summary.Text);
            Weather.ImageSource = htmlWeatherForecast.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
            string temperatureRange = HttpUtility.HtmlDecode(Regex.Replace(htmlWeatherForecast.DocumentNode.ChildNodes[8].InnerText, "[ \\n]", string.Empty));
            Weather.Low = temperatureRange.Split('-')[0];
            Weather.High = temperatureRange.Split('-')[1];
            string wind = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[22].InnerText, "[ \\n]", string.Empty);
            Weather.Wind = HttpUtility.HtmlDecode(wind);
            // TODO: Check if this gets a value while raining or always nothing
            string rain = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[28].InnerText, "[ \\n]", string.Empty);
            Weather.Rain = HttpUtility.HtmlDecode(rain);
            string pressure = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[32].InnerText, "[ \\n]", string.Empty);
            Weather.Pressure = HttpUtility.HtmlDecode(pressure);
        }

        private void ExecuteTagesschauCommand(object obj)
        {
            ((MainWindow)Application.Current.MainWindow).TagesschauButton.Background = new SolidColorBrush() { Color = (Color)ColorConverter.ConvertFromString("#FF1E1E1E") };
            using (XmlReader reader = XmlReader.Create("https://www.tagesschau.de/xml/rss2")) // https://rss.sueddeutsche.de/rss/Topthemen
            {
                Articles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    // TODO: Exceptionsicher machen und das auch nicht nur hier!!!!!!!
                    /*System.NullReferenceException: "Der Objektverweis wurde nicht auf eine Objektinstanz festgelegt."

System.ServiceModel.Syndication.SyndicationFeed.ImageUrl.get hat null zurückgegeben.
                     */
                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = item.Summary.Text,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime,
                        ImageSource = feed.ImageUrl.AbsoluteUri
                    });
                }
            };
        }

        private void ExecuteMinimizeCommand(object obj)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void ExecuteRestoreCommand(object obj)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                RestoreButton = "/Images/maximize.png";
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                RestoreButton = "/Images/restore.png";
            }
        }
        private void ExecuteCloseCommand(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        #region CanExecutes
        private bool CanExecuteClose(object arg)
        {
            return true;
        }
        private bool CanExecuteRestore(object arg)
        {
            return true;
        }
        private bool CanExecuteMinimize(object arg)
        {
            return true;
        }
        private bool CanExecuteTagesschau(object arg)
        {
            return true;
        }
        private bool CanExecuteFAZ(object arg)
        {
            return true;
        }
        private bool CanExecuteSZ(object arg)
        {
            return true;
        }
        private bool CanExecuteCNN(object arg)
        {
            return true; ;
        }
        #endregion
    }
}
