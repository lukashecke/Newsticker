using HtmlAgilityPack;
using Newsticker.Commands;
using Newsticker.Models;
using Newsticker.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml.Linq;

namespace Newsticker.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        Dictionary<string, string> locationRssLookup = new Dictionary<string, string>();
        #region properties
        public ICommand CloseCommand { get; set; }
        public ICommand RestoreCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand TagesschauCommand { get; set; }
        public ICommand FAZCommand { get; set; }
        public ICommand SZCommand { get; set; }
        public ICommand CNNCommand { get; set; }
        public ICommand NZZCommand { get; set; }
        public ICommand ZeitCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        #endregion

        #region entities
        private bool zeitChecked = false;
        public bool ZeitChecked
        {
            get
            {
                return this.zeitChecked;
            }
            set
            {
                this.zeitChecked = value;
                this.OnPropertyChanged("ZeitChecked");
            }
        }
        private bool tagesschauChecked = false;
        public bool TagesschauChecked
        {
            get
            {
                return this.tagesschauChecked;
            }
            set
            {
                this.tagesschauChecked = value;
                this.OnPropertyChanged("TagesschauChecked");
            }
        }
        private bool fAZChecked = false;
        public bool FAZChecked
        {
            get
            {
                return this.fAZChecked;
            }
            set
            {
                this.fAZChecked = value;
                this.OnPropertyChanged("FAZChecked");
            }
        }
        private bool sZChecked = false;
        public bool SZChecked
        {
            get
            {
                return this.sZChecked;
            }
            set
            {
                this.sZChecked = value;
                this.OnPropertyChanged("SZChecked");
            }
        }
        private bool cNNChecked = false;
        public bool CNNChecked
        {
            get
            {
                return this.cNNChecked;
            }
            set
            {
                this.cNNChecked = value;
                this.OnPropertyChanged("CNNChecked");
            }
        }
        private bool nZZChecked = false;
        public bool NZZChecked
        {
            get
            {
                return this.nZZChecked;
            }
            set
            {
                this.nZZChecked = value;
                this.OnPropertyChanged("NZZChecked");
            }
        }
        private Dictionary<string, ObservableCollection<ArticleModel>> cache;
        public Dictionary<string, ObservableCollection<ArticleModel>> Cache
        {
            get
            {
                if (this.cache == null)
                {
                    this.cache = new Dictionary<string, ObservableCollection<ArticleModel>>();
                }
                return this.cache;
            }
            set
            {
                this.cache = value;
                this.OnPropertyChanged("Cache");
            }
        }
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
                this.WelcomeTextVisibility = "Hidden";
                this.OnPropertyChanged("Articles");
                this.OnPropertyChanged("WelcomeTextVisibility");
            }
        }
        private string restoreButton;
        public string RestoreButton
        {
            get
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    this.restoreButton = "/Images/restore.png";
                    return this.restoreButton;
                }
                else
                {
                    this.restoreButton = "/Images/maximize.png";
                    return this.restoreButton;
                }
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
        private string loadingScreenVisibility = "Visible";
        public string LoadingScreenVisibility
        {
            get
            {
                return this.loadingScreenVisibility;
            }
            set
            {
                this.loadingScreenVisibility = value;
                this.OnPropertyChanged("LoadingScreenVisibility");
            }
        }
        private string welcomeTextVisibility = "Visible";
        public string WelcomeTextVisibility
        {
            get
            {
                return this.welcomeTextVisibility;
            }
            set
            {
                this.welcomeTextVisibility = value;
                this.OnPropertyChanged("WelcomeTextVisibility");
            }
        }
        private ObservableCollectionEx<string> weatherLocationsList;
        public ObservableCollectionEx<string> WeatherLocationsList
        {
            get
            {
                if (this.weatherLocationsList == null)
                {
                    this.weatherLocationsList = new ObservableCollectionEx<string>();
                }
                return this.weatherLocationsList;
            }
            set
            {
                this.weatherLocationsList = value;
                this.OnPropertyChanged("WeatherLocationsList");
                LoadWeather(new object());
            }
        }
        #endregion 

        #region constructors
        public MainWindowViewModel(string loadingState)
        {
            this.CloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteClose);
            this.RestoreCommand = new RelayCommand(ExecuteRestoreCommand, CanExecuteRestore);
            this.MinimizeCommand = new RelayCommand(ExecuteMinimizeCommand, CanExecuteMinimize);
            this.TagesschauCommand = new RelayCommand(ExecuteTagesschauCommand, CanExecuteTagesschau);
            this.FAZCommand = new RelayCommand(ExecuteFAZCommand, CanExecuteFAZ);
            this.SZCommand = new RelayCommand(ExecuteSZCommand, CanExecuteSZ);
            this.CNNCommand = new RelayCommand(ExecuteCNNCommand, CanExecuteCNN);
            this.NZZCommand = new RelayCommand(ExecuteNZZCommand, CanExecuteNZZ);
            this.ZeitCommand = new RelayCommand(ExecuteZeitCommand, CanExecuteZeit);
            this.UpdateCommand = new RelayCommand(ExecuteUpdateCommand, CanExecuteUpdate);

            //WeatherLocationsList.Add("");
            //locationRssLookup.Add("", "");
            WeatherLocationsList.Add("München");
            locationRssLookup.Add("München", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160007&obs=1&fc=1");
            WeatherLocationsList.Add("London");
            locationRssLookup.Add("London", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160127&obs=1&fc=1");
            WeatherLocationsList.Add("Washington DC");
            locationRssLookup.Add("Washington DC", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160136&obs=1&fc=1");
            WeatherLocationsList.Add("Wrocław");
            locationRssLookup.Add("Wrocław", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160911&obs=1&fc=1");
            WeatherLocationsList.Add("Warszawa");
            locationRssLookup.Add("Warszawa", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160277&obs=1&fc=1");
            WeatherLocationsList.Add("Beijing");
            locationRssLookup.Add("Beijing", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160059&obs=1&fc=1");

            if (loadingState.Equals("preLoad"))
            {
                LoadAllComponents();
                LoadingScreenVisibility = "Hidden"; // Wird fälschlicherweise noch kurz angezeigt, obwohl schon geladen im Pre-Load-Aufruf
            }
        }
        #endregion

        #region commands
        private void ExecuteUpdateCommand(object obj)
        {
            LoadAllComponents();
        }
        private void ExecuteNZZCommand(object obj)
        {
            NZZChecked = true;
            ZeitChecked = false;
            TagesschauChecked = false;
            FAZChecked = false;
            SZChecked = false;
            CNNChecked = false;
            Articles = Cache["NZZ"];
        }
        private void ExecuteCNNCommand(object obj)
        {
            NZZChecked = false;
            ZeitChecked = false;
            TagesschauChecked = false;
            FAZChecked = false;
            SZChecked = false;
            CNNChecked = true;
            Articles = Cache["CNN"];
        }

        private void ExecuteSZCommand(object obj)
        {
            NZZChecked = false;
            ZeitChecked = false;
            TagesschauChecked = false;
            FAZChecked = false;
            SZChecked = true;
            CNNChecked = false;
            Articles = Cache["SZ"];
        }

        private void ExecuteFAZCommand(object obj)
        {
            NZZChecked = false;
            ZeitChecked = false;
            TagesschauChecked = false;
            FAZChecked = true;
            SZChecked = false;
            CNNChecked = false;
            Articles = Cache["FAZ"];
        }

        private void ExecuteTagesschauCommand(object obj)
        {
            NZZChecked = false;
            ZeitChecked = false;
            TagesschauChecked = true;
            FAZChecked = false;
            SZChecked = false;
            CNNChecked = false;
            Articles = Cache["Tagesschau"];
        }
        private void ExecuteZeitCommand(object obj)
        {
            NZZChecked = false;
            ZeitChecked = true;
            TagesschauChecked = false;
            FAZChecked = false;
            SZChecked = false;
            CNNChecked = false;
            Articles = Cache["Zeit"];
        }
        #region canExecutes
        private bool CanExecuteNZZ(object arg)
        {
            return true;
        }
        private bool CanExecuteZeit(object arg)
        {
            return true;
        }
        private bool CanExecuteUpdate(object arg)
        {
            return true;
        }
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
            return true;
        }
        #endregion
        #endregion

        #region loading Components
        public void LoadAllComponents()
        {
            LoadingScreenVisibility = "Visible";
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_LoadAllComponents);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingScreenVisibility = "Hidden";
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void backgroundWorker_LoadAllComponents(object sender, DoWorkEventArgs e)
        {
            LoadWeather(new object());
            LoadCNN(new object());
            LoadSZ(new object());
            LoadFAZ(new object());
            LoadTagesschau(new object());
            LoadZeit(new object());
            LoadNZZ(new object());
        }

        private void LoadNZZ(object v)
        {
            ObservableCollection<ArticleModel> nZZArticles = new ObservableCollection<ArticleModel>();

            SyndicationFeed feed = new SyndicationFeed();
            using (XmlReader r = new MyXmlReader("https://www.nzz.ch/recent.rss"))
            {
                // Neuste Artikel: https://www.nzz.ch/recent.rss
                // Topthemen der Startseite https://www.nzz.ch/startseite.rss
                // Intenational https://www.nzz.ch/international.rss
                feed = SyndicationFeed.Load(r);
            }
            nZZArticles.Clear();
            int i = 0;
            string htmlString = new WebClient().DownloadString("https://www.nzz.ch/recent.rss");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);
            foreach (var item in feed.Items)
            {
                string imageSource = string.Empty;
                try
                {
                    imageSource = htmlDoc.DocumentNode.Descendants("media:thumbnail").ElementAt(i).GetAttributeValue("url", string.Empty);
                }
                catch (Exception)
                {
                    // TODO: NZZ Default Image hinzufügrn nd generell eigene default bilder weil die die vom feed angeboten werden total schlechte auflösung sind und generell einfach scheiße
                }
                nZZArticles.Add(new ArticleModel
                {
                    Header = item.Title.Text,
                    Summary = item.Summary?.Text,
                    Link = item.Id,
                    Date = item.PublishDate.DateTime.ToLocalTime(), // TODO: Überall PublishDate.LocalDateTime einfach nehmen anstatt manuell zui verändern :)
                    ImageSource = imageSource
                });
                i++;
            }

            if (!Cache.ContainsKey("NZZ"))
            {
                Cache.Add("NZZ", nZZArticles);
            }
            else
            {
                Cache["NZZ"] = nZZArticles;
            }
        }
        private void LoadZeit(object v)
        {
            ObservableCollection<ArticleModel> zeitArticles = new ObservableCollection<ArticleModel>();

            using (XmlReader reader = XmlReader.Create("http://newsfeed.zeit.de/"))
            // Die Zet Zeitung RSS Verzeivhnis http://newsfeed.zeit.de/
            // Zeit Online News http://newsfeed.zeit.de/news/index
            // Zeit Online Homepage http://newsfeed.zeit.de/index
            {
                zeitArticles.Clear();
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
                        summary = doc.DocumentNode.InnerText.Remove(0, 1);
                    }
                    catch (Exception)
                    {
                        imageSource = feed.ImageUrl.AbsoluteUri;
                    }
                    zeitArticles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = summary,
                        Link = item.Links[0].Uri.AbsoluteUri,
                        Date = item.PublishDate.DateTime.ToLocalTime(),
                        ImageSource = imageSource
                    });
                }
            };

            if (!Cache.ContainsKey("Zeit"))
            {
                Cache.Add("Zeit", zeitArticles);
            }
            else
            {
                Cache["Zeit"] = zeitArticles;
            }
        }
        private void LoadCNN(object obj)
        {
            ObservableCollection<ArticleModel> cNNArticles = new ObservableCollection<ArticleModel>();
            // TODO: Generell überall auf null und so abfragen den rss Feed
            using (XmlReader reader = XmlReader.Create("http://rss.cnn.com/rss/cnn_latest.rss"))
            // Top Stories http://rss.cnn.com/rss/edition.rss
            // World http://rss.cnn.com/rss/edition_world.rss
            // Most Recent http://rss.cnn.com/rss/cnn_latest.rss
            {
                cNNArticles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                
                foreach (var item in feed.Items)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Summary.Text);
                    string summary = string.Empty;
                    try
                    {
                        summary = doc.DocumentNode.ChildNodes[0].InnerHtml;
                    }
                    catch (Exception)
                    {
                    }
                    cNNArticles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = summary,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime.ToLocalTime()
                    });
                }
            };
            if (!Cache.ContainsKey("CNN"))
            {
                Cache.Add("CNN", cNNArticles);
            }
            else
            {
                Cache["CNN"] = cNNArticles;
            }
        }
        private void LoadSZ(object obj)
        {
            ObservableCollection<ArticleModel> sZArticles = new ObservableCollection<ArticleModel>();

            using (XmlReader reader = XmlReader.Create("https://rss.sueddeutsche.de/rss/Topthemen"))
            // Top-Themen (alle Artikel der Homepage) https://rss.sueddeutsche.de/rss/Topthemen
            //Alle Artikel (alle Artikel auf ganz SZ.de) https://rss.sueddeutsche.de/app/service/rss/alles/index.rss?output=rss
            // Allgemeine Eilmeldung (wichtige Meldungen aus allen Ressorts) https://rss.sueddeutsche.de/rss/Eilmeldungen
            {
                sZArticles.Clear();
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
                    sZArticles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = summary,
                        Link = item.Links[0].Uri.AbsoluteUri,
                        Date = item.PublishDate.DateTime.ToLocalTime(),
                        ImageSource = imageSource
                    });
                }
            };
            if (!Cache.ContainsKey("SZ"))
            {
                Cache.Add("SZ", sZArticles);
            }
            else
            {
                Cache["SZ"] = sZArticles;
            }
        }
        private void LoadFAZ(object obj)
        {
            ObservableCollection<ArticleModel> fAZArticles = new ObservableCollection<ArticleModel>();

            using (XmlReader reader = XmlReader.Create("https://www.faz.net/rss/aktuell"))
            // Aktuell https://www.faz.net/rss/aktuell
            {
                fAZArticles.Clear();
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
                    fAZArticles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = summary,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime,
                        ImageSource = imageSource
                    });
                }
            };
            if (!Cache.ContainsKey("FAZ"))
            {
                Cache.Add("FAZ", fAZArticles);
            }
            else
            {
                Cache["FAZ"] = fAZArticles;
            }
        }
        private void LoadTagesschau(object obj)
        {
            ObservableCollection<ArticleModel> tagesschauArticles = new ObservableCollection<ArticleModel>();

            using (XmlReader reader = XmlReader.Create("https://www.tagesschau.de/xml/rss2"))
            // Tagesschau RSS Feed https://www.tagesschau.de/xml/rss2
            {
                tagesschauArticles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    // TODO: Exceptionsicher machen und das auch nicht nur hier!!!!!!!
                    /*System.NullReferenceException: "Der Objektverweis wurde nicht auf eine Objektinstanz festgelegt."

System.ServiceModel.Syndication.SyndicationFeed.ImageUrl.get hat null zurückgegeben.
                     */

                    string imageSource = string.Empty;
                    //if (feed.ImageUrl?.AbsoluteUri == null)
                    //{
                    //    imageSource = "/Images/tagesschau.png";
                    //}
                    //else
                    //{
                    imageSource = feed.ImageUrl?.AbsoluteUri;
                    //}
                    tagesschauArticles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = item.Summary.Text,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime,
                        ImageSource = imageSource
                    });
                }
            };
            if (!Cache.ContainsKey("Tagesschau"))
            {
                Cache.Add("Tagesschau", tagesschauArticles);
            }
            else
            {
                Cache["Tagesschau"] = tagesschauArticles;
            }
        }
        public void LoadWeather(object obj)
        {
            // TODO: Weathericons from here https://www.flaticon.com/packs/weather-19
            SyndicationFeed feed2;
            using (XmlReader reader = XmlReader.Create(locationRssLookup[WeatherLocationsList.Current]))
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
            Weather.UpdateTime = feed2.LastUpdatedTime.LocalDateTime;
            Weather.Location = feed2.Items.ElementAt(1).Title.Text.Split(' ').First();
            HtmlDocument htmlWeatherForecast = new HtmlDocument();
            htmlWeatherForecast.LoadHtml(feed2.Items.ElementAt(1).Summary.Text);
            // ImageSource
            string weatherConditionAsGif = htmlWeatherForecast.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty).Split('/').Last(); // e.g. sunny.gif
            string imageSource = "/Images/Weather/" + Path.GetFileNameWithoutExtension(weatherConditionAsGif) + ".png";
            Weather.ImageSource = imageSource;
            //if ()
            //{
            //    Weather.ImageSource = imageSource;
            //}
            //else
            //{
            //    Weather.ImageSource = htmlWeatherForecast.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
            //}

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
        #endregion

        #region window functionality
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
        #endregion
    }
    /// <summary>
    /// Workaround-class to read in xml-files Syndicationfeed-valid DateTime objects
    /// </summary>
    class MyXmlReader : XmlTextReader
    {
        private bool readingDate = false;
        const string CustomUtcDateTimeFormat = "ddd MMM dd HH:mm:ss Z yyyy"; // Wed Oct 07 08:00:07 GMT 2009

        public MyXmlReader(Stream s) : base(s) { }

        public MyXmlReader(string inputUri) : base(inputUri) { }

        public override void ReadStartElement()
        {
            if (string.Equals(base.NamespaceURI, string.Empty, StringComparison.InvariantCultureIgnoreCase) &&
                (string.Equals(base.LocalName, "lastBuildDate", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(base.LocalName, "pubDate", StringComparison.InvariantCultureIgnoreCase)))
            {
                readingDate = true;
            }
            base.ReadStartElement();
        }

        public override void ReadEndElement()
        {
            if (readingDate)
            {
                readingDate = false;
            }
            base.ReadEndElement();
        }

        public override string ReadString()
        {
            if (readingDate)
            {
                string dateString = base.ReadString();
                DateTime dt;
                if (!DateTime.TryParse(dateString, out dt))
                    dt = DateTime.ParseExact(dateString, CustomUtcDateTimeFormat, CultureInfo.InvariantCulture);
                return dt.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
            }
            else
            {
                return base.ReadString();
            }
        }
    }
}
