using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newsticker.Models
{
    class WeatherModel : ModelBase
    {
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
        private string wind;
        public string Wind
        {
            get { return wind; }
            set
            {
                if (wind != value)
                {
                    wind = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string rain;
        public string Rain
        {
            get { return rain; }
            set
            {
                if (rain != value)
                {
                    rain = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string pressure;
        public string Pressure
        {
            get { return pressure; }
            set
            {
                if (pressure != value)
                {
                    pressure = value;
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
    }
}
