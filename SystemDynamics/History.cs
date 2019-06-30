using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics
{
    internal class History<T>
    {
        private readonly List<T> list
            = new List<T>();

        public Result<T> Last
            => list.Count > 0
            ? new Result<T>(list[list.Count - 1])
            : new Result<T>(new IndexOutOfRangeException("list.Count = 0"));

        public void Add(T v)
        {
            new Result<Exception>(new IndexOutOfRangeException()).ToString();
            list.Add(v);
        }

        public Result<T> Move(ConsoleKeyInfo info)
        {
            switch(info.Key)
            {
                case ConsoleKey.UpArrow:
                    return Move(HistoryMove.UP);
                case ConsoleKey.DownArrow:
                    return Move(HistoryMove.DOWN);
                default:
                    return new Result<T>(new NotSupportedException(info.Key.ToString()));
            }
        }

        public Result<T> Move(HistoryMove info)
        {

        }
    }

    enum HistoryMove
    {
        UP,
        DOWN
    }
}