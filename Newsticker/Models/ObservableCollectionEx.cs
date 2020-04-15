using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Models
{
    class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        private T current;
        public T Current
        {
            get
            {
                if (this.current == null)
                {
                    current = this.First();
                }
                return this.current;
            }
            set
            {
                this.current = value;
            }
        }
    }
}
