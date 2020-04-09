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
    internal class FazRssTransformer : RssTransformerBase
    {
        public FazRssTransformer(string uri) : base(uri)
        {
        }

        public override ObservableCollection<ArticleModel> GetArticles()
        {
            ObservableCollection<ArticleModel> fAZArticles = new ObservableCollection<ArticleModel>();

                fAZArticles.Clear();
                foreach (var item in this.SyndicationFeed.Items)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(item.Summary.Text);
                    string summary = string.Empty;
                    string imageSource;
                    try
                    {
                        imageSource = doc.DocumentNode.Descendants("img").First().GetAttributeValue("src", string.Empty);
                        summary = doc.DocumentNode.Descendants("p").First().InnerText;
                    }
                    catch (Exception)
                    {
                        imageSource = this.SyndicationFeed.ImageUrl.AbsoluteUri;
                    }
                    fAZArticles.Add(new ArticleModel
                    (item.Title.Text,
                        summary,
                        item.Id,
                        item.PublishDate.DateTime,
                        imageSource
                    ));
                }
            return fAZArticles;
        }
    }
}
