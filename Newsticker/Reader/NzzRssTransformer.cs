using HtmlAgilityPack;
using Newsticker.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace Newsticker.Reader
{
    internal class NzzRssTransformer : RssTransformerBase
    {
        public NzzRssTransformer(string uri) : base(uri)
        {
        }
        public override ObservableCollection<ArticleModel> GetArticles()
        {
            ObservableCollection<ArticleModel> nZZArticles = new ObservableCollection<ArticleModel>();
            int i = 0;
            string htmlString = new WebClient().DownloadString("https://www.nzz.ch/recent.rss");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);
            foreach (var item in this.SyndicationFeed.Items)
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
                nZZArticles.Add(new ArticleModel(
                    item.Title.Text, 
                    item.Summary?.Text, 
                    item.Id,
                    item.PublishDate.DateTime.ToLocalTime(),
                    imageSource));
                i++;
            }
            return nZZArticles;
        }
    }
}