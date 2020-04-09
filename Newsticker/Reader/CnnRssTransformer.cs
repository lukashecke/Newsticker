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
    internal class CnnRssTransformer : RssTransformerBase
    {
        public CnnRssTransformer(string uri) : base(uri)
        {
        }

        public override ObservableCollection<ArticleModel> GetArticles()
        {
            ObservableCollection<ArticleModel> cnnArticles = new ObservableCollection<ArticleModel>();
            // TODO: Generell überall auf null und so abfragen den rss Feed
            
                foreach (var item in this.SyndicationFeed.Items)
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
                    cnnArticles.Add(new ArticleModel
                    (item.Title.Text,
                        summary,
                        item.Id,
                        item.PublishDate.DateTime.ToLocalTime()
                    ));
                }
            return cnnArticles;
        }
    }
}
