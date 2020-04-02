using Newsticker.Commands;
using Newsticker.Models;
using Newsticker.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Newsticker.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Commands
        public ICommand CloseCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand TagesschauCommand { get; set; }
        public ICommand FAZCommand { get; set; }
        public ICommand SZCommand { get; set; }
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

        public MainWindowViewModel()
        {
            this.CloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteClose);
            this.MinimizeCommand = new RelayCommand(ExecuteMinimizeCommand, CanExecuteMinimize);
            this.TagesschauCommand = new RelayCommand(ExecuteTagesschauCommand, CanExecuteTagesschau);
            this.FAZCommand = new RelayCommand(ExecuteFAZCommand, CanExecuteFAZ);
            this.SZCommand = new RelayCommand(ExecuteSZCommand, CanExecuteSZ);
        }



        private void ExecuteSZCommand(object obj)
        {
            using (XmlReader reader = XmlReader.Create("https://rss.sueddeutsche.de/rss/Topthemen"))
            {
                Articles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    //XmlDocument xmlDocument = new XmlDocument();
                    //xmlDocument.LoadXml("<root>"+item.Summary.Text+"</root>");
                    //var image = xmlDocument.GetElementsByTagName("root");

                    // TODO: Das hier über HTML bzw. XML lösen, keine Stringspielereien!
                    string temp = item.Summary.Text.Substring(10);
                    string temp2 = temp.Substring(0, temp.IndexOf('"'));
                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = item.Summary.Text,
                        Link = item.Links[0].Uri.AbsoluteUri,
                        Date = item.PublishDate.DateTime,
                        //Text = "<img src=\"https://media-cdn.sueddeutsche.de/image/sz.1.4865667/208x156\" alt=\"Boateng unterstützt Münchner und Berliner Tafel\" data-portal-copyright=\"dpa\" /><p>Der Spieler sagt, er habe nur seinen kranken Sohn in Berlin besucht. Ajax-Sportdirektor Ove...
                        ImageSource = temp2
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
                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = item.Summary.Text,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime
                    });
                }
            };

            // TODO: Eigenständige Aktualisierung des Wetters
            using (XmlReader reader = XmlReader.Create("https://www.wetter.com/wetter_rss/wetter.xml"))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                ((MainWindow)Application.Current.MainWindow).WeatherFeed.Text = feed.Items.First().Summary.Text; // Allgemeine Wetterlage
                //((MainWindow)Application.Current.MainWindow).WeatherFeed.Text = feed.Items.ElementAt(1).Summary.Text; // html?
            };
        }

        private void ExecuteTagesschauCommand(object obj)
        {
            using (XmlReader reader = XmlReader.Create("https://www.tagesschau.de/xml/rss2")) // https://rss.sueddeutsche.de/rss/Topthemen
            {
                Articles.Clear();
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {

                    Articles.Add(new ArticleModel
                    {
                        Header = item.Title.Text,
                        Summary = item.Summary.Text,
                        Link = item.Id,
                        Date = item.PublishDate.DateTime
                    });
                }
            };
        }

        private void ExecuteMinimizeCommand(object obj)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
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
        #endregion
    }
}
