using HtmlAgilityPack;
using Newsticker.Commands;
using Newsticker.Models;
using Newsticker.Reader;
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
        #region properties
        private bool WeatherIsLoading { get; set; }
        private BackgroundWorker WeatherBackgroundWorker { get; set; }
        private BackgroundWorker BackgroundWorker { get; set; }
        public Dictionary<string, string> LocationRssLookup { get; set; }
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
        private double fontSize;
        public double FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                this.fontSize = value;
                this.OnPropertyChanged("FontSize");
            }
        }
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
        private string weatherSpinnerVisibility = "Hidden";
        public string WeatherSpinnerVisibility
        {
            get
            {
                return this.weatherSpinnerVisibility;
            }
            set
            {
                this.weatherSpinnerVisibility = value;
                this.OnPropertyChanged("WeatherSpinnerVisibility");
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
            InitializeCommands();
            InitializeLocationRssLookup();
            if (loadingState.Equals("preLoad"))
            {
                LoadAllComponents();
                WaitForWeatherBackgroundWorker();
                LoadingScreenVisibility = "Hidden"; // Wird fälschlicherweise noch kurz angezeigt, obwohl schon geladen im Pre-Load-Aufruf
            }
        }
        private void InitializeLocationRssLookup()
        {
            //WeatherLocationsList.Add("");
            //locationRssLookup.Add("", "");
            LocationRssLookup = new Dictionary<string, string>();
            WeatherLocationsList.Add("München");
            LocationRssLookup.Add("München", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160007&obs=1&fc=1");
            WeatherLocationsList.Add("London");
            LocationRssLookup.Add("London", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160127&obs=1&fc=1");
            WeatherLocationsList.Add("Washington DC");
            LocationRssLookup.Add("Washington DC", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160136&obs=1&fc=1");
            WeatherLocationsList.Add("Breslau");
            LocationRssLookup.Add("Breslau", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160911&obs=1&fc=1");
            WeatherLocationsList.Add("Warschau");
            LocationRssLookup.Add("Warschau", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160277&obs=1&fc=1");
            WeatherLocationsList.Add("Beijing");
            LocationRssLookup.Add("Beijing", "https://rss.weatherzone.com.au/?u=12994-1285&lt=twcid&lc=160059&obs=1&fc=1");
        }
        private void InitializeCommands()
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
        }
        #endregion

        #region commands
        // TODO: Wenn zwischen den einzelnen reitern der Zeitschriften rumgesprungen wird, soll der scroller immer wieder am start der seite sein, bislang wird diese information weitergegeben, wenn zb sz zu ende gelesen und auf cnn geklickt wird befindet man sich am ende der seite
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
            BackgroundWorker = new BackgroundWorker();
            BackgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_LoadAllComponents);
            BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
            BackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            BackgroundWorker.WorkerReportsProgress = true;
            WeatherIsLoading = true;
            BackgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker.Dispose();
            LoadingScreenVisibility = "Hidden";
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BackgroundWorker_LoadAllComponents(object sender, DoWorkEventArgs e)
        {
            LoadWeather(new object());
            LoadCNN(new object());
            LoadSZ(new object());
            LoadFAZ(new object());
            LoadTagesschau(new object());
            LoadZeit(new object());
            LoadNZZ(new object());
        }
        private void WaitForWeatherBackgroundWorker()
        {
            while (WeatherIsLoading)
            {
                // Wait for it
            }
        }
        private void LoadNZZ(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            NzzRssTransformer nzzRssTransformer = new NzzRssTransformer("https://www.nzz.ch/recent.rss");
            // Neuste Artikel https://www.nzz.ch/recent.rss
            // Topthemen der Startseite https://www.nzz.ch/startseite.rss
            // International https://www.nzz.ch/international.rss

            if (!Cache.ContainsKey("NZZ"))
            {
                Cache.Add("NZZ", nzzRssTransformer.GetArticles());
            }
            else
            {
                Cache["NZZ"] = nzzRssTransformer.GetArticles();
            }
        }
        private void LoadZeit(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            ZeitRssTransformer zeitRssTransformer = new ZeitRssTransformer("http://newsfeed.zeit.de/");
            // Die Zet Zeitung RSS Verzeivhnis http://newsfeed.zeit.de/
            // Zeit Online News http://newsfeed.zeit.de/news/index
            // Zeit Online Homepage http://newsfeed.zeit.de/index

            if (!Cache.ContainsKey("Zeit"))
            {
                Cache.Add("Zeit", zeitRssTransformer.GetArticles());
            }
            else
            {
                Cache["Zeit"] = zeitRssTransformer.GetArticles();
            }
        }
        private void LoadCNN(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            CnnRssTransformer cnnRssTransformer = new CnnRssTransformer("http://rss.cnn.com/rss/cnn_latest.rss");
            // Top Stories http://rss.cnn.com/rss/edition.rss
            // World http://rss.cnn.com/rss/edition_world.rss
            // Most Recent http://rss.cnn.com/rss/cnn_latest.rss

            if (!Cache.ContainsKey("CNN"))
            {
                Cache.Add("CNN", cnnRssTransformer.GetArticles());
            }
            else
            {
                Cache["CNN"] = cnnRssTransformer.GetArticles();
            }
        }
        private void LoadSZ(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            SzRssTransformer szRssTransformer = new SzRssTransformer("https://rss.sueddeutsche.de/rss/Topthemen");
            // Top-Themen (alle Artikel der Homepage) https://rss.sueddeutsche.de/rss/Topthemen
            //Alle Artikel (alle Artikel auf ganz SZ.de) https://rss.sueddeutsche.de/app/service/rss/alles/index.rss?output=rss
            // Allgemeine Eilmeldung (wichtige Meldungen aus allen Ressorts) https://rss.sueddeutsche.de/rss/Eilmeldungen

            if (!Cache.ContainsKey("SZ"))
            {
                Cache.Add("SZ", szRssTransformer.GetArticles());
            }
            else
            {
                Cache["SZ"] = szRssTransformer.GetArticles();
            }
        }
        private void LoadFAZ(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            FazRssTransformer fazRssTransformer = new FazRssTransformer("https://www.faz.net/rss/aktuell");
            // Aktuell https://www.faz.net/rss/aktuell

            if (!Cache.ContainsKey("FAZ"))
            {
                Cache.Add("FAZ", fazRssTransformer.GetArticles());
            }
            else
            {
                Cache["FAZ"] = fazRssTransformer.GetArticles();
            }
        }
        private void LoadTagesschau(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            TagesschauRssTransformer tagesschauRssTransformer = new TagesschauRssTransformer("https://www.tagesschau.de/xml/rss2");
            // Tagesschau RSS Feed https://www.tagesschau.de/xml/rss2

            if (!Cache.ContainsKey("Tagesschau"))
            {
                Cache.Add("Tagesschau", tagesschauRssTransformer.GetArticles());
            }
            else
            {
                Cache["Tagesschau"] = tagesschauRssTransformer.GetArticles();
            }
        }
        public void LoadWeather(object obj)
        {
            WeatherIsLoading = true;
            WeatherBackgroundWorker = new BackgroundWorker();
            WeatherSpinnerVisibility = "Visible";
            // Weathericons from here https://www.flaticon.com/packs/weather-19
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            WeatherBackgroundWorker.DoWork += delegate
            {
                SyndicationFeed feed2;
                using (XmlReader reader = XmlReader.Create(LocationRssLookup[WeatherLocationsList.Current]))
                {
                    feed2 = SyndicationFeed.Load(reader);
                };
                HtmlDocument htmlCurrentWeather = new HtmlDocument();
                htmlCurrentWeather.LoadHtml(feed2.Items.ElementAt(0).Summary.Text);
                string temperature = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[2].InnerText, "[ \\n]", string.Empty);
                string feel = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[8].InnerText, "[ \\n]", string.Empty);
                string dewPoint = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[12].InnerText, "[ \\n]", string.Empty);
                string relativeHumidity = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[18].InnerText, "[ \\n]", string.Empty);
                Weather.Temperature = HttpUtility.HtmlDecode(temperature);
                Weather.Feel = HttpUtility.HtmlDecode(feel);
                Weather.DewPoint = HttpUtility.HtmlDecode(dewPoint);
                Weather.RelativeHumidity = HttpUtility.HtmlDecode(relativeHumidity);
                Weather.UpdateTime = feed2.LastUpdatedTime.LocalDateTime;
                Weather.WeekDay = Weather.UpdateTime.ToString("dddd", new CultureInfo("de-DE"));

                Weather.Location = feed2.Items.ElementAt(1).Title.Text.Split(' ').First();
                HtmlDocument htmlWeatherForecast = new HtmlDocument();
                htmlWeatherForecast.LoadHtml(feed2.Items.ElementAt(1).Summary.Text);
                // First Data Package in the weather forecast is for the current day
                // ImageSource
                string weatherConditionAsGif = htmlWeatherForecast.DocumentNode.Descendants("img").ElementAt(0).GetAttributeValue("src", string.Empty).Split('/').Last(); // e.g. sunny.gif
                string imageSource = "/Images/Weather/" + Path.GetFileNameWithoutExtension(weatherConditionAsGif) + ".png";
                Weather.ImageSource = imageSource;

                string temperatureRange = HttpUtility.HtmlDecode(Regex.Replace(htmlWeatherForecast.DocumentNode.ChildNodes[8].InnerText, "[ \\n]", string.Empty));
                Weather.Low = temperatureRange.Split('C')[0]+"C";
                Weather.High = (temperatureRange.Split('C')[1]+"C").Substring(1);
                string pressure = Regex.Replace(htmlCurrentWeather.DocumentNode.ChildNodes[32].InnerText, "[ \\n]", string.Empty);
                Weather.Pressure = HttpUtility.HtmlDecode(pressure);

                Weather.NextWeekDay = Weather.UpdateTime.AddDays(1.0).ToString("dddd", new CultureInfo("de-DE"));
                Weather.NextDayInfo = HttpUtility.HtmlDecode(Regex.Replace(htmlWeatherForecast.DocumentNode.ChildNodes[19].InnerText, "[ \\n]", string.Empty)).Replace("C-", "C bis ");
                string nextDayWeatherConditionAsGif = htmlWeatherForecast.DocumentNode.Descendants("img").ElementAt(1).GetAttributeValue("src", string.Empty).Split('/').Last();
                string nextDayImageSource = "/Images/Weather/" + Path.GetFileNameWithoutExtension(nextDayWeatherConditionAsGif) + ".png";
                weather.NextDayImageSource=nextDayImageSource;

                Weather.OverNextWeekDay = Weather.UpdateTime.AddDays(2.0).ToString("dddd", new CultureInfo("de-DE"));
                Weather.OverNextDayInfo = HttpUtility.HtmlDecode(Regex.Replace(htmlWeatherForecast.DocumentNode.ChildNodes[30].InnerText, "[ \\n]", string.Empty)).Replace("C-", "C bis ");
                string overNextDayWeatherConditionAsGif = htmlWeatherForecast.DocumentNode.Descendants("img").ElementAt(2).GetAttributeValue("src", string.Empty).Split('/').Last();
                string overNextDayImageSource = "/Images/Weather/" + Path.GetFileNameWithoutExtension(overNextDayWeatherConditionAsGif) + ".png";
                weather.OverNextDayImageSource = overNextDayImageSource;

                Weather.OverOverNextWeekDay = Weather.UpdateTime.AddDays(3.0).ToString("dddd", new CultureInfo("de-DE"));
                Weather.OverOverNextDayInfo = HttpUtility.HtmlDecode(Regex.Replace(htmlWeatherForecast.DocumentNode.ChildNodes[41].InnerText, "[ \\n]", string.Empty)).Replace("C-", "C bis ");
                string overOverNextDayWeatherConditionAsGif = htmlWeatherForecast.DocumentNode.Descendants("img").ElementAt(3).GetAttributeValue("src", string.Empty).Split('/').Last();
                string overOverNextDayImageSource = "/Images/Weather/" + Path.GetFileNameWithoutExtension(overOverNextDayWeatherConditionAsGif) + ".png";
                weather.OverOverNextDayImageSource = overOverNextDayImageSource;
            };
            WeatherBackgroundWorker.RunWorkerCompleted += delegate
            {
                WeatherSpinnerVisibility = "Hidden";
                WeatherBackgroundWorker.Dispose();
                WeatherIsLoading = false;
            };
            WeatherBackgroundWorker.RunWorkerAsync();

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
}
