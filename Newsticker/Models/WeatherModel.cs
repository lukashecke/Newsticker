using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Models
{
    class WeatherModel : ModelBase
    {
        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                if (location != value)
                {
                    location = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string temperature;
        public string Temperature
        {
            get { return temperature; }
            set
            {
                if (temperature != value)
                {
                    temperature = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string feel;
        public string Feel
        {
            get { return feel; }
            set
            {
                if (feel != value)
                {
                    feel = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string dewPoint;
        public string DewPoint
        {
            get { return dewPoint; }
            set
            {
                if (dewPoint != value)
                {
                    dewPoint = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string relativeHumidity;
        public string RelativeHumidity
        {
            get { return relativeHumidity; }
            set
            {
                if (relativeHumidity != value)
                {
                    relativeHumidity = value;
                    this.OnPropertyChanged();
                }
            }
        }
        // I don't need/ want this
        //private string wind;
        //public string Wind
        //{
        //    get { return wind; }
        //    set
        //    {
        //        if (wind != value)
        //        {
        //            wind = value;
        //            this.OnPropertyChanged();
        //        }
        //    }
        //}
        // WeatherZone doesn't offer this information through their RSS-Feed
        //private string rain;
        //public string Rain
        //{
        //    get { return rain; }
        //    set
        //    {
        //        if (rain != value)
        //        {
        //            rain = value;
        //            this.OnPropertyChanged();
        //        }
        //    }
        //}
        private string pressure;
        public string Pressure
        {
            get { return pressure; }
            set
            {
                if (pressure != value)
                {
                    if (value.Equals("hPa"))
                    {
                        pressure = "Keine Informationen verfügbar";
                    }
                    else
                    {
                        pressure = value;
                    }
                    this.OnPropertyChanged();
                }
            }
        }
        private string forecast;
        public string Forecast
        {
            get { return forecast; }
            set
            {
                if (forecast != value)
                {
                    forecast = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private DateTime updateTime;
        public DateTime UpdateTime
        {
            get { return updateTime; }
            set
            {
                if (updateTime != value)
                {
                    updateTime = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string high;
        public string High
        {
            get { return high; }
            set
            {
                if (high != value)
                {
                    high = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string low;
        public string Low
        {
            get { return low; }
            set
            {
                if (low != value)
                {
                    low = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string weekDay;
        public string WeekDay
        {
            get { return weekDay; }
            set
            {
                if (weekDay != value)
                {
                    weekDay = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string nextWeekDay;
        public string NextWeekDay
        {
            get { return nextWeekDay; }
            set
            {
                if (nextWeekDay != value)
                {
                    nextWeekDay = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string overNextWeekDay;
        public string OverNextWeekDay
        {
            get { return overNextWeekDay; }
            set
            {
                if (overNextWeekDay != value)
                {
                    overNextWeekDay = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string nextDayInfo;
        public string NextDayInfo
        {
            get { return nextDayInfo; }
            set
            {
                if (nextDayInfo != value)
                {
                    nextDayInfo = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string nextDayImageSource;
        public string NextDayImageSource
        {
            get { return nextDayImageSource; }
            set
            {
                if (nextDayImageSource != value)
                {
                    nextDayImageSource = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string overNextDayInfo;
        public string OverNextDayInfo
        {
            get { return overNextDayInfo; }
            set
            {
                if (overNextDayInfo != value)
                {
                    overNextDayInfo = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string overNextDayImageSource;
        public string OverNextDayImageSource
        {
            get { return overNextDayImageSource; }
            set
            {
                if (overNextDayImageSource != value)
                {
                    overNextDayImageSource = value;
                    this.OnPropertyChanged();
                }
            }
        }
    }
}
