using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class FileList : IReadOnlyList<string>
    {
        private FileList(string directory, List<string> files, int index)
        {
            Directory = directory;
            _files = files;
            CurrentIndex = index;
        }

        private readonly List<string> _files;

        public string this[int index] => _files[index];

        public int Count => _files.Count;

        public int CurrentIndex { get; private set; }

        public string? CurrentFile
        {
            get
            {
                if (_files.Count == 0)
                    return null;
                return _files[CurrentIndex];
            }
        }

        public string Directory { get; }

        public bool Contains(string item)
        {
            return _files.Contains(item);
        }

        public string? GetPrevious()
        {
            if (_files.Count == 0)
                return null;

            CurrentIndex--;
            if (CurrentIndex < 0)
                CurrentIndex = _files.Count - 1;

            return _files[CurrentIndex];
        }

        public string? GetNext()
        {
            if (_files.Count == 0)
                return null;

            CurrentIndex++;
            if (CurrentIndex >= _files.Count)
                CurrentIndex = 0;

            return _files[CurrentIndex];
        }

        public static FileList LoadDirectory(string directory, IEnumerable<string> Extension)
        {
            if (!System.IO.Directory.Exists(directory))
                throw new DirectoryNotFoundException();

            string[] items = System.IO.Directory.GetFiles(directory);
            List<string> extensionList = new(Extension);
            List<string> files = new();
            foreach (string item in items)
                if (extensionList.Contains(Path.GetExtension(item).TrimStart('.')))
                    files.Add(item);
            int index;
            if (files.Count > 0)
                index = 0;
            else
                index = -1;

            return new(directory, files, index);
        }

        public static FileList LoadFile(string file, IEnumerable<string> Extension)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException();

            string directory = Path.GetDirectoryName(file) ?? throw new DirectoryNotFoundException();

            string[] items = System.IO.Directory.GetFiles(directory);
            List<string> extensionList = new(Extension);
            List<string> files = new();
            foreach (string item in items)
                if (extensionList.Contains(Path.GetExtension(item).TrimStart('.')))
                    files.Add(item);

            int index = files.IndexOf(file);

            return new(directory, files, index);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_files).GetEnumerator();
        }
    }
}
