using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace String.Model
{
    public class Subtitle : INotifyPropertyChanged
    {
        private static int _id = 0;
        private static TimeSpan _MaxHide = TimeSpan.FromMilliseconds(0);
        private string _Text;
        private string _Translation;
        private TimeSpan _Show;
        private TimeSpan _Hide;

        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public string Translation
        {
            get => _Translation;
            set
            {
                _Translation = value;
                OnPropertyChanged(nameof(Translation));
            }
        }

        public TimeSpan Show 
        { 
            get => _Show;
            set
            {
                if (value > _Hide)
                {
                    _Show = _Hide;
                    return;
                }
                _Show = value;
                OnPropertyChanged(nameof(Show));
                OnPropertyChanged(nameof(Duration));
            }
        }

        public TimeSpan Hide
        {
            get => _Hide;
            set
            {
                if (value < _Show)
                {
                    _Hide = _Show;
                    return;
                }

                _Hide = value;
                OnPropertyChanged(nameof(Hide));
                OnPropertyChanged(nameof(Duration));
            }
        }

        public TimeSpan Duration
        {
            get => Hide - Show;
            set
            {
                Hide = Show + value;
                OnPropertyChanged(nameof(Hide));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Subtitle() 
        {
            Hide = _MaxHide + TimeSpan.FromMilliseconds(1123);
            Show = (_MaxHide - TimeSpan.FromMilliseconds(500)).TotalMilliseconds > 0 ? _MaxHide - TimeSpan.FromMilliseconds(500) : _MaxHide;

            _MaxHide = Hide;

            Text = "Text no." + _id;

            Translation = "Translation no." + _id;
            _id++;
        }

        public Subtitle(TimeSpan show, TimeSpan hide, string text, string translation = "")
        {
            Hide = hide;
            if (translation.Length == 0)
                Translation = "Translation no." + _id;
            else
                Translation = translation;
            Text = text;
            Show = show;

            _id++;

            if (Hide > _MaxHide)
                _MaxHide = Hide;

        }

        public Subtitle(TimeSpan show, TimeSpan hide)
        {
            Hide = hide;
            Show = show;
            if (Hide > _MaxHide)
                _MaxHide = Hide;

            Text = "Text no." + _id;

            Translation = "Translation no." + _id;

            _id++;
        }

        public bool ShouldShow(TimeSpan time)
        {
            return Show <= time && time <= Hide;
        }

        public static TimeSpan MaxHide => _MaxHide;

    }
}
