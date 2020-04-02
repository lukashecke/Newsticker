using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Models
{
    class ArticleModel
    {
        public string Header { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public string ImageSource { get; set; }
    }
}
