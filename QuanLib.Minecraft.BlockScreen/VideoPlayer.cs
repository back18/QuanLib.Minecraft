using QuanLib.Minecraft.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    /// <summary>
    /// 视频播放器
    /// </summary>
    public class VideoPlayer : Screen
    {
        public VideoPlayer(Vector3<int> startPosition, Facing xFacing, Facing yFacing, int width, int height) :
            base(startPosition, xFacing, yFacing, width, height)
        {
            _frames = new();
        }

        private readonly List<ArrayFrame> _frames;

        private int _index;

        public void Load(string[] files)
        {
            int width = 120;
            int height = 90;
            foreach (string file in files)
            {
                string[,] ids = new string[width, height];
                byte[] bytes = File.ReadAllBytes(file);
                int index = 0;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        ids[x, y] = bytes[index++] switch
                        {
                            0 => ConcretePixel.ToBlockID(MinecraftColor.Black),
                            1 => ConcretePixel.ToBlockID(MinecraftColor.Gray),
                            2 => ConcretePixel.ToBlockID(MinecraftColor.LightGray),
                            3 => ConcretePixel.ToBlockID(MinecraftColor.White),
                            _ => throw new FormatException(),
                        };
                    }
                _frames.Add(new(ids));
            }
        }

        public void Play()
        {
            _index = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            long totalTime = 0;
            long nextFrameTime = 100;
            while (_index < _frames.Count)
            {
                stopwatch.Start();

                ShowNewFrame(_frames[_index]);

                Console.WriteLine($"frame:{_index} time: {stopwatch.ElapsedMilliseconds}");
                while (totalTime + stopwatch.ElapsedMilliseconds  < nextFrameTime)
                    Thread.Yield();

                totalTime += 100;
                nextFrameTime += 100;
                _index++;

                stopwatch.Reset();
            }
        }
    }
}
