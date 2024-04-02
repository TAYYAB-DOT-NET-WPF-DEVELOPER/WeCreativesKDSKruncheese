using System;
using System.ComponentModel;

namespace WeCreatives_KDSPJ.Connections
{
    public class KDSModel : INotifyPropertyChanged
    {
        private string _transact;
        private string _descript;
        private string _type;
        private string _time;
        private int _trno;
        private int _bumped;
        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged(nameof(StartTime));
                    // Recalculate the DisplayTime immediately when StartTime is set
                    UpdateDisplayTime();
                }
            }
        }

        private string _displayTime;
        public string DisplayTime
        {
            get => _displayTime;
            set
            {
                if (_displayTime != value)
                {
                    _displayTime = value;
                    OnPropertyChanged(nameof(DisplayTime));
                }
            }
        }

        public void UpdateDisplayTime()
        {
            TimeSpan timeSpan = DateTime.Now - _startTime;
            DisplayTime = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        public string Transact
        {
            get => _transact;
            set
            {
                if (_transact != value)
                {
                    _transact = value;
                    OnPropertyChanged(nameof(Transact));
                }
            }
        }

        public string Descript
        {
            get => _descript;
            set
            {
                if (_descript != value)
                {
                    _descript = value;
                    OnPropertyChanged(nameof(Descript));
                }
            }
        }

        public string TYPE
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(TYPE));
                }
            }
        }

        public string Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        public int TRNO
        {
            get => _trno;
            set
            {
                if (_trno != value)
                {
                    _trno = value;
                    OnPropertyChanged(nameof(TRNO));
                }
            }
        }

        public int Bumped
        {
            get => _bumped;
            set
            {
                if (_bumped != value)
                {
                    _bumped = value;
                    OnPropertyChanged(nameof(Bumped));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
