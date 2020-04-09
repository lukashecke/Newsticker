using Newsticker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Reader
{
    internal class TagesschauRssTransformer : RssTransformerBase
    {
        public TagesschauRssTransformer(string uri) : base(uri)
        {
        }

        public override ObservableCollection<ArticleModel> GetArticles()
        {
            ObservableCollection<ArticleModel> tagesschauArticles = new ObservableCollection<ArticleModel>();
            foreach (var item in this.SyndicationFeed.Items)
            {
                // TODO: Exceptionsicher machen und das auch nicht nur hier!!!!!!!
                /*System.NullReferenceException: "Der Objektverweis wurde nicht auf eine Objektinstanz festgelegt."
                   System.ServiceModel.Syndication.SyndicationFeed.ImageUrl.get hat null zurückgegeben.
                 */

                //if (feed.ImageUrl?.AbsoluteUri == null)
                //{
                //    imageSource = "/Images/tagesschau.png";
                //}
                //else
                //{
                string imageSource = this.SyndicationFeed.ImageUrl?.AbsoluteUri;
                //}
                tagesschauArticles.Add(new ArticleModel
                (item.Title.Text,
                    item.Summary.Text,
                    item.Id,
                    item.PublishDate.DateTime,
                    imageSource
                ));
            }
            return tagesschauArticles;
        }
    }
}