using System;

namespace SystemDynamics
{
    public struct Result<T>
    {
        public Result(T value)
        {
            Value = value;
            Problem = null;
        }

        public Result(Exception problem)
        {
            Value = default(T);
            Problem = problem;
        }

        public bool Ok => Problem == null;
        public readonly Exception Problem;
        public readonly T Value;
    }
}
