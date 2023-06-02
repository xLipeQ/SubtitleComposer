using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using String.Model;

namespace SubtitlePlugin
{
    public enum TextSaved
    {
        Text = 0,
        Translation = 1,
    }

    public interface ISubtitlePlugin
    {
        TextSaved Name { get; }

        string Extension { get; }

        ICollection<Subtitle> Load(string path);

        void Save(string path, ICollection<Subtitle> records);

    }

    public class SubtitlePlugin : ISubtitlePlugin
    {
        private TextSaved _Name;
        private string _Extension;

        public TextSaved Name => _Name;

        public string Extension => _Extension;

        public ICollection<Subtitle> Load(string path)
        {
            if (!path.Contains(Extension))
                path += Extension;

            ICollection<Subtitle> records = new ObservableCollection<Subtitle>();

            if (!File.Exists(path))
                return records;

            int cur_line = 1;
            var lines = File.ReadAllLines(path);

            for (int item = 0; item < lines.Length;)
            {
                if (cur_line.ToString() == lines[item])
                {
                    string text = string.Empty;
                    int i = item + 2;
                    while(i < lines.Length && (cur_line+1).ToString() != lines[i])
                    {
                        text += lines[i];
                        i++;

                    }

                    records.Add(new Subtitle(TimeSpan.Parse(lines[item + 1].Substring(0, lines[item + 1].LastIndexOf("-->")).Trim()),
                        TimeSpan.Parse(lines[item + 1].Substring(lines[item + 1].LastIndexOf("-->") + 3).Trim()), text));
                    
                    cur_line++;

                    // Iterate one block at a time
                    item = i;
                }
            }


            return records;

        }

        public void Save(string path, ICollection<Subtitle> records)
        {
            if (!path.Contains(Extension))
                path += Extension;

            using (StreamWriter writer = File.CreateText(path))
            {
                int i = 1;
                foreach (Subtitle sub in records)
                {
                    writer.WriteLine(i);
                    writer.WriteLine(sub.Show.ToString(@"hh\:mm\:ss\.fff") + " --> " + sub.Hide.ToString(@"hh\:mm\:ss\.fff"));
                    if(Name == TextSaved.Text)
                        writer.WriteLine(sub.Text);
                    else
                        writer.WriteLine(sub.Translation);
                    i++;
                }
                writer.WriteLine();
            }
        }

        public SubtitlePlugin(TextSaved name, string extension = ".srt")
        {
            _Name = name;
            _Extension = extension;
        }
    }
}
