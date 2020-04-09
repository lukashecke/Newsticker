using HtmlAgilityPack;
using Newsticker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Reader
{
    internal class SzRssTransformer : RssTransformerBase
    {
        public SzRssTransformer(string uri) : base(uri)
        {
        }

        public override ObservableCollection<ArticleModel> GetArticles()
        {
            ObservableCollection<ArticleModel> sZArticles = new ObservableCollection<ArticleModel>();
            foreach (var item in this.SyndicationFeed.Items)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(item.Summary.Text);
                string summary = string.Empty;
                string imageSource;
                // TODO: TryCatch Summary und ImageUrl trennen, irgendwa sgutes einfallen lassen
                try
                {
                    imageSource = doc.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
                    summary = doc.DocumentNode.Descendants("p").First().InnerText;
                }
                catch (Exception)
                {
                    imageSource = this.SyndicationFeed.ImageUrl.AbsoluteUri;
                }
                sZArticles.Add(new ArticleModel(
                    item.Title.Text, 
                    summary, 
                    item.Links[0].Uri.AbsoluteUri, 
                    item.PublishDate.DateTime.ToLocalTime(), 
                    imageSource));
            }
            return sZArticles;
        }
    }
}