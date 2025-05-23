﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Models
{
    class ArticleModel : ModelBase
    {
        public string Header { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public string ImageSource { get; set; }
        public ArticleModel(string header, string summary, string link, DateTime date, string imageSource=null)
        {
            Header = header;
            Summary = summary;
            Link = link;
            Date = date;
            ImageSource = imageSource;
        }
    }
}
