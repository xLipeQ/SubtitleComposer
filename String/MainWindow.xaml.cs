﻿using Microsoft.Win32;
using String.Model;
using SubtitlePlugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace String
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private Members
        /// <summary>
        /// Represents whether mouse is pressed when going on slider
        /// </summary>
        private static bool IsDragged = false;

        private static bool DraggedPerformed = true;

        private Border Popup;

        private bool BorderJustCreated = false;

        private bool VideoLoaded = false;

        /// <summary>
        /// Indiicates whether video is plating
        /// </summary>
        private bool _VideoIsPlaying = false;

        private bool _TranlationOn = false;

        /// <summary>
        /// How long the video is
        /// </summary>
        private TimeSpan _VideoDuration = TimeSpan.FromSeconds(0);

        /// <summary>
        /// Path to a video
        /// </summary>
        private string _Video = string.Empty;

        /// <summary>
        /// Timer to increse the position
        /// </summary>
        private DispatcherTimer timer;

        private int milisec = 17;

        private TimeSpan _position = TimeSpan.FromSeconds(0);

        private ObservableCollection<Subtitle> _Captions = new ObservableCollection<Subtitle>();

        private string _Caption = string.Empty;
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Properties
        /// <summary>
        /// Collections of Film descriptions
        /// </summary>
        public ObservableCollection<Subtitle> FilmsColl { get; set; } = new ObservableCollection<Subtitle>();

        /// <summary>
        /// Captions to be diplayed on current time
        /// </summary>
        public ObservableCollection<Subtitle> Captions
        {
            get { return _Captions; }
            set
            {
                if (_Captions == value)
                    return;

                _Captions = value;
                OnPropertyChanged(nameof(Captions));
            }
        }

        public string Caption
        {
            get => _Caption;
            set
            {
                if (_Caption == value)
                    return;

                _Caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public string Video
        {
            get => _Video;
            set
            {
                _Video = value;
                OnPropertyChanged(nameof(Video));
            }
        }

        public TimeSpan Position
        {
            get => _position;
            set
            {
                _position = value;
                //if(Math.Abs(value.TotalMilliseconds - VideoPlayer.Position.TotalMilliseconds) > 300)
                //    VideoPlayer.Position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public TimeSpan VideoDuration
        {
            get => _VideoDuration;
            set
            {
                _VideoDuration = value;
                OnPropertyChanged(nameof(VideoDuration));
            }
        }  

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new MainViewModel();
            DataContext = this;

            Films.AutoGeneratedColumns += Sort;

            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(2324)));
            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(567), TimeSpan.FromMilliseconds(4456)));
            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(4512), TimeSpan.FromMilliseconds(9329)));
            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(2052), TimeSpan.FromMilliseconds(3620)));
           
            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(6093), TimeSpan.FromMilliseconds(10134)));
            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(8130), TimeSpan.FromMilliseconds(15041)));
            FilmsColl.Add(new Subtitle(TimeSpan.FromMilliseconds(10456), TimeSpan.FromMilliseconds(13001)));

            for (int i = 0; i < 21; i++)
                FilmsColl.Add(new Subtitle());


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(milisec);
            timer.Tick += Timer_Tick;
            timer.Start();


        }

        private void Sort(object sender, EventArgs e)
        {
            Films.AutoGenerateColumns = false;

            var firstCol = Films.Columns.First();
            firstCol.SortDirection = ListSortDirection.Ascending;
            Films.Items.SortDescriptions.Add(new SortDescription(firstCol.SortMemberPath, ListSortDirection.Ascending));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_VideoIsPlaying || (!DraggedPerformed && !IsDragged))
                return;

            Position = VideoPlayer.Position;
            Position += TimeSpan.FromMilliseconds(milisec);

            foreach(var item in FilmsColl)
            {
                bool ifshow = item.ShouldShow(Position);
                if(!Captions.Contains(item) && ifshow)
                    Captions.Add(item);

                else if(!ifshow)
                    Captions.Remove(item);
            }
            var c = new StringBuilder(Captions.Count * 20);
            foreach (Subtitle film in Captions)
            {
                if(!_TranlationOn)
                    c.AppendLine(film.Text);
                else
                    c.AppendLine(film.Translation);

            }
            Caption = c.ToString();
        }

        /// <summary>
        /// Closes app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Tells about app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Subtitle Composer","About Message", MessageBoxButton.OK);
        }

        /// <summary>
        /// Changes the Transition flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trans_Click(object sender, RoutedEventArgs e)
        {
            var se = (MenuItem)sender;
            se.IsChecked = se.IsChecked ? false : true;

            _TranlationOn = se.IsChecked;
        }

        /// <summary>
        /// Changes the volume of Media player (VideoPlayer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoMenager_MouseWheel(object sender, MouseWheelEventArgs e)
        {


            if (!_VideoIsPlaying)
                return;

            VideoPlayer.Volume += (e.Delta) > 0 ? 0.05 : -0.05;
            if (VideoPlayer.Volume <= 0)
                VideoPlayer.Volume = 0;
            else if (VideoPlayer.Volume >= 1)
                VideoPlayer.Volume = 1;
        }

        /// <summary>
        /// Plays or Pauses the Video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoMenager_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (VideoPlayer.Source == null)
                return;

            if(_VideoIsPlaying)
            {
                VideoPlayer.Pause();
                _VideoIsPlaying = false;
            }

            else
            {
                VideoPlayer.Play();
                _VideoIsPlaying = true;
            }
        }

        #region Video Func
        private void VideoPause()
        {
            VideoPlayer.Pause();
            _VideoIsPlaying = false;
        }

        private void VideoPlay()
        {
            VideoPlayer.Play();
            _VideoIsPlaying = true;
        }

        private void VideoStopAndReset()
        {
            VideoPlayer.Pause();
            Position = VideoPlayer.Position=TimeSpan.Zero;
            _VideoIsPlaying = false;
        }
        #endregion

        #region Film Change
        /// <summary>
        /// Opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDialog_Click(object sender, RoutedEventArgs e)
        {
            ChooseFilm();
        }

        /// <summary>
        /// Opens a dialog for choosing a file
        /// </summary>
        /// <returns></returns>
        private void ChooseFilm()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video files (*.mp4)|*.mp4";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

            if (openFileDialog.ShowDialog() == true)
            {
                Video = openFileDialog.FileName;

                VideoPlayer.Play();
                _VideoIsPlaying = true;
                VideoPlayer.Position = TimeSpan.Zero;
            }
            
        }

        private void Films_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            try
            {
                if ((Subtitle)grid.SelectedItem != null)
                    VideoPlayer.Position = ((Subtitle)grid.SelectedItem).Show;
            }
            catch { }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            VideoDuration = VideoPlayer.NaturalDuration.TimeSpan;
        }

        #endregion

        #region Slider

        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            timer.Stop();
            IsDragged = true;
            DraggedPerformed = false;
        }
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            IsDragged = false;
            SliderChangedValue(((Slider)sender).Value);
        }
        private void SliderChangedValue(double value)
        {
            VideoPlayer.Position = TimeSpan.FromMilliseconds(value);
            Position = TimeSpan.FromMilliseconds(value);
            DraggedPerformed = true;
            timer.Start();
        }

        #endregion

        #region Buttons
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            VideoPlay();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            VideoPause();   
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            VideoStopAndReset();
        }

        #endregion

        /// <summary>
        /// Creates Popup for adding, adding after, and deleting subtitle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Films_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(SubtitleCanvas);

            SubtitleCanvas.Children.Remove(Popup);

            Popup = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
            };

            StackPanel sp = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };

            Popup.Child = sp;

            Button add = new Button()
            {
                Content = "Add",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.LightGray,
                Padding = new Thickness(3),
            };

            Button add_aft = new Button()
            {
                Width = 100,
                Content = "Add After",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.LightGray,
            };
            Button delete = new Button()
            {
                Content = "Delete",
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.LightGray,
            };
            
            add.Click += Add_Click;
            add_aft.Click += Add_aft_Click;
            delete.Click += Delete_Click;

            sp.Children.Add(add);
            sp.Children.Add(add_aft);
            sp.Children.Add(delete);
            Canvas.SetLeft(Popup, p.X);
            Canvas.SetTop(Popup, p.Y);
            SubtitleCanvas.Children.Add(Popup);
            BorderJustCreated = true;
        }

        /// <summary>
        /// Deletes selected items from subtitle collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<Subtitle> list = new List<Subtitle>(Films.SelectedItems.Count);
            foreach (Subtitle item in Films.SelectedItems)
                list.Add(item);
            
            foreach(var item in list)
                FilmsColl.Remove(item);
            
            SubtitleCanvas.Children.Remove(Popup);

            Films.SelectedItem = null;
        }

        /// <summary>
        /// Adds new Subtitle behind the last selected subtitle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_aft_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan max = TimeSpan.Zero;
            foreach (Subtitle item in Films.SelectedItems)
                if(item.Hide > max)
                    max = item.Hide;

            FilmsColl.Add(new Subtitle(max, max));
            SubtitleCanvas.Children.Remove(Popup);

        }

        /// <summary>
        /// Adds new Subtitle to the end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            FilmsColl.Add(new Subtitle(Subtitle.MaxHide, Subtitle.MaxHide));
            SubtitleCanvas.Children.Remove(Popup);
        }

        private void LoadSubRip_Click(object sender, RoutedEventArgs e)
        {
            ISubtitlePlugin plugin = new SubtitlePlugin.SubtitlePlugin(TextSaved.Text);

            var col = plugin.Load("plik");

            foreach (var item in col)
                FilmsColl.Add(item);
        }

        private void SaveText_Click(object sender, RoutedEventArgs e)
        {
            ISubtitlePlugin plugin = new SubtitlePlugin.SubtitlePlugin(TextSaved.Text);

            if (Films.SelectedItem == null)
                plugin.Save("plik", FilmsColl);

            else
            {
                ICollection<Subtitle> subtitles = new List<Subtitle>(Films.SelectedItems.Count);
                foreach (Subtitle v in Films.SelectedItems)
                    subtitles.Add(v);

                plugin.Save("plik", subtitles);
            }
        }

        private void SaveTrans_Click(object sender, RoutedEventArgs e)
        {
            ISubtitlePlugin plugin = new SubtitlePlugin.SubtitlePlugin(TextSaved.Translation);

            if (Films.SelectedItem == null)
                plugin.Save("plik", FilmsColl);

            else
            {
                ICollection<Subtitle> subtitles = new List<Subtitle>(Films.SelectedItems.Count);
                foreach (Subtitle v in Films.SelectedItems)
                    subtitles.Add(v);

                plugin.Save("plik", subtitles);
            }
        }

    }
}