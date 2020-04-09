using Newsticker.Models;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;

namespace Newsticker.Reader
{
    internal abstract class RssTransformerBase
    {
        public Uri Uri { get; set; }
        public SyndicationFeed SyndicationFeed { get; set; }
        public RssTransformerBase(string uri)
        {
            this.Uri = new Uri(uri);
            using (XmlReader r = new SyndicationFeedValidXmlReader(uri))
            {
                this.SyndicationFeed = SyndicationFeed.Load(r);
            }
        }
        public abstract ObservableCollection<ArticleModel> GetArticles();
        // @MAW So kleinkariert geht das nicht, da Teile der Daten nicht ausgelesen werden können und deshalb direkt aus der html kommen und mititeriert werden pro syndicationitem
        // einzelne Daten sind abhängig von einem gemeinsamen überliegendem objekt
        //public abstract string GetHeader();
        //public abstract string GetSummary();
        //public abstract string GetLink();
        //public abstract DateTime GetDate();
        //public abstract string GetImageSource();
    } 
}
