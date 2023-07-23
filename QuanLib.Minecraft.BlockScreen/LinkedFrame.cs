using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class LinkedFrame : AbstractFrame
    {
        public LinkedFrame()
        {
            _ids = new();
        }

        public LinkedFrame(string[,] ids) : this()
        {
            if (ids is null)
                throw new ArgumentNullException(nameof(ids));

            int width = ids.GetLength(0);
            int height = ids.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                LinkedList<string> list = new();
                for (int y = 0; y < height; y++)
                    list.AddLast(ids[x, y]);
                _ids.AddLast(list);
            }
        }

        public LinkedFrame(ArrayFrame frame) : this(frame?.GetAllBlockID() ?? throw new ArgumentNullException(nameof(frame))) { }

        private readonly LinkedList<LinkedList<string>> _ids;

        public override int Width => _ids.Count;

        public override int Height => _ids.First?.Value?.Count ?? 0;

        public void AddBorder(string id, int count = 1)
        {
            if (count == 0)
                return;

            AddTop(id, count);
            AddBottom(id, count);
            AddLeft(id, count);
            AddRight(id, count);
        }

        public void AddLeft(string id, int count = 1)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            if (count == 0)
                return;

            int height = Height;
            string[] array = new string[height];
            Array.Fill(array, id);
            _ids.AddFirst(new LinkedList<string>(array));
            count--;
            for (int i = 0; i < count; i++)
            {
                string[] newArray = new string[height];
                array.CopyTo(newArray, 0);
                _ids.AddFirst(new LinkedList<string>(newArray));
            }
        }

        public void AddLeft(string[,] ids)
        {
            if (ids is null)
                throw new ArgumentNullException(nameof(ids));

            int width = ids.GetLength(0);
            int height = ids.GetLength(1);
            for (int x = width - 1; x >= 0; x--)
            {
                LinkedList<string> list = new();
                for (int y = 0; y < height; y++)
                    list.AddLast(ids[x, y]);
                _ids.AddFirst(list); 
            }
        }

        public void AddLeft(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            AddLeft(frame.GetAllBlockID());
        }

        public void AddLeft(LinkedFrame frameBuilder)
        {
            LinkedListNode<LinkedList<string>>? node = frameBuilder._ids.Last;
            while (node is not null)
            {
                _ids.AddFirst(node.Value);
                node = node.Previous;
            }
        }

        public void AddRight(string id, int count = 1)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            if (count == 0)
                return;

            int height = Height;
            string[] array = new string[height];
            Array.Fill(array, id);
            _ids.AddLast(new LinkedList<string>(array));
            count--;
            for (int i = 0; i < count; i++)
            {
                string[] newArray = new string[height];
                array.CopyTo(newArray, 0);
                _ids.AddLast(new LinkedList<string>(newArray));
            }
        }

        public void AddRight(string[,] ids)
        {
            if (ids is null)
                throw new ArgumentNullException(nameof(ids));

            int width = ids.GetLength(0);
            int height = ids.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                LinkedList<string> list = new();
                for (int y = 0; y < height; y++)
                    list.AddLast(ids[x, y]);
                _ids.AddLast(list);
            }
        }

        public void AddRight(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            AddRight(frame.GetAllBlockID());
        }

        public void AddRight(LinkedFrame frameBuilder)
        {
            LinkedListNode<LinkedList<string>>? node = frameBuilder._ids.First;
            while (node is not null)
            {
                _ids.AddLast(node.Value);
                node = node.Previous;
            }
        }

        public void AddTop(string id, int count = 1)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            foreach (var list in _ids)
            {
                for (int i = 0; i < count; i++)
                    list.AddFirst(id);
            }
        }

        public void AddTop(string[,] ids)
        {
            if (ids is null)
                throw new ArgumentNullException(nameof(ids));

            int x = 0;
            foreach (var list in _ids)
            {
                for (int y = ids.GetLength(1) - 1; y >= 0; y--)
                    list.AddFirst(ids[x, y]);
                x++;
            }
        }

        public void AddTop(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            AddTop(frame.GetAllBlockID());
        }

        public void AddTop(LinkedFrame frameBuilder)
        {
            var thisNode = _ids.First;
            var newNode = frameBuilder._ids.Last;
            while (thisNode is not null && newNode is not null)
            {
                var newSubNode = newNode.Value.Last;
                while (newSubNode is not null)
                {
                    thisNode.Value.AddFirst(newSubNode.Value);
                    newSubNode = newSubNode.Previous;
                }

                thisNode = thisNode.Next;
                newNode = newNode.Next;
            }
        }

        public void AddBottom(string id, int count = 1)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            foreach (var list in _ids)
            {
                for (int i = 0; i < count; i++)
                    list.AddLast(id);
            }
        }

        public void AddBottom(string[,] ids)
        {
            if (ids is null)
                throw new ArgumentNullException(nameof(ids));

            int height = ids.GetLength(1);
            int x = 0;
            foreach (var list in _ids)
            {
                for (int y = 0; y < height; y++)
                    list.AddLast(ids[x, y]);
                x++;
            }
        }

        public void AddBottom(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            AddBottom(frame.GetAllBlockID());
        }

        public void AddBottom(LinkedFrame frameBuilder)
        {
            var thisNode = _ids.First;
            var newNode = frameBuilder._ids.Last;
            while (thisNode is not null && newNode is not null)
            {
                foreach (var newSubNode in newNode.Value)
                    thisNode.Value.AddLast(newSubNode);

                thisNode = thisNode.Next;
                newNode = newNode.Next;
            }
        }

        public void RemoveLeft(int count)
        {
            if (count > Width)
            {
                _ids.Clear();
                return;
            }

            for (int i = 0; i < count; i++)
                _ids.RemoveFirst();
        }

        public void RemoveRight(int count)
        {
            if (count > Width)
            {
                _ids.Clear();
                return;
            }

            for (int i = 0; i < count; i++)
                _ids.RemoveLast();
        }

        public void RemoveTop(int count)
        {
            if (count > Height)
            {
                _ids.Clear();
                return;
            }

            foreach (var list in _ids)
            {
                for (int i = 0; i < count; i++)
                    list.RemoveFirst();
            }
        }

        public void RemoveBottom(int count)
        {
            if (count > Height)
            {
                _ids.Clear();
                return;
            }

            foreach (var list in _ids)
            {
                for (int i = 0; i < count; i++)
                    list.RemoveLast();
            }
        }

        public override ArrayFrame ToArrayFrame()
        {
            string[,] array = new string[Width, Height];
            int x = 0;
            foreach (var list in _ids)
            {
                int y = 0;
                foreach (var id in list)
                {
                    array[x, y] = id;
                    y++;
                }
                x++;
            }
            return new ArrayFrame(array);
        }

        public override LinkedFrame ToLinkedFrame()
        {
            return this;
        }

        public override void CorrectSize(Size size, ContentAnchor anchor, string background)
        {
            if (Width == size.Width && Height == size.Height)
                return;

            switch (anchor)
            {
                case ContentAnchor.UpperLeft:
                    if (Width < size.Width)
                        AddRight(background, size.Width - Width);
                    else if (Width > size.Width)
                        RemoveRight(Width - size.Width);
                    if (Height < size.Height)
                        AddBottom(background, size.Height - Height);
                    else if (Height > size.Height)
                        RemoveBottom(Height - size.Height);
                    break;
                case ContentAnchor.UpperRight:
                    if (Width < size.Width)
                        AddLeft(background, size.Width - Width);
                    else if (Width > size.Width)
                        RemoveLeft(Width - size.Width);
                    if (Height < size.Height)
                        AddBottom(background, size.Height - Height);
                    else if (Height > size.Height)
                        RemoveBottom(Height - size.Height);
                    break;
                case ContentAnchor.LowerLeft:
                    if (Width < size.Width)
                        AddRight(background, size.Width - Width);
                    else if (Width > size.Width)
                        RemoveRight(Width - size.Width);
                    if (Height < size.Height)
                        AddTop(background, size.Height - Height);
                    else if (Height > size.Height)
                        RemoveTop(Height - size.Height);
                    break;
                case ContentAnchor.LowerRight:
                    if (Width < size.Width)
                        AddLeft(background, size.Width - Width);
                    else if (Width > size.Width)
                        RemoveLeft(Width - size.Width);
                    if (Height < size.Height)
                        AddTop(background, size.Height - Height);
                    else if (Height > size.Height)
                        RemoveTop(Height - size.Height);
                    break;
                case ContentAnchor.Centered:
                    if (Width < size.Width)
                    {
                        AddLeft(background, (size.Width - Width) / 2);
                        AddRight(background, size.Width - Width);
                    }
                    else if (Width > size.Width)
                    {
                        RemoveLeft((Width - size.Width) / 2);
                        RemoveRight(Width - size.Width);
                    }
                    if (Height < size.Height)
                    {
                        AddTop(background, (size.Height - Height) / 2);
                        AddBottom(background, size.Height - Height);
                    }
                    else if (Height > size.Height)
                    {
                        RemoveTop((Height - size.Height) / 2);
                        RemoveBottom(Height - size.Height);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
