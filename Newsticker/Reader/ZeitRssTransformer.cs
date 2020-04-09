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
    internal class ZeitRssTransformer : RssTransformerBase
    {
        public ZeitRssTransformer(string uri) : base(uri)
        {
        }

        public override ObservableCollection<ArticleModel> GetArticles()
        {
            ObservableCollection<ArticleModel> zeitArticles = new ObservableCollection<ArticleModel>();

                zeitArticles.Clear();
                foreach (var item in this.SyndicationFeed.Items)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Summary.Text);
                    string summary = string.Empty;
                    string imageSource;
                    try
                    {
                        imageSource = doc.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
                        summary = doc.DocumentNode.InnerText.Remove(0, 1);
                    }
                    catch (Exception)
                    {
                        imageSource = this.SyndicationFeed.ImageUrl.AbsoluteUri;
                    }
                    zeitArticles.Add(new ArticleModel
                    (
                        item.Title.Text,
                        summary,
                        item.Links[0].Uri.AbsoluteUri,
                        item.PublishDate.DateTime.ToLocalTime(),
                        imageSource
                    ));
                }
            return zeitArticles;
        }
    }
}
