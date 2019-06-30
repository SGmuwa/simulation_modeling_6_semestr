using System;

namespace SystemDynamics.commands
{
    internal class IntervalUpdate : AbstractCommand
    {
        private readonly Drawer drawer;

        public IntervalUpdate(Drawer drawer) : base(nameof(IntervalUpdate)) => this.drawer = drawer;

        public override string Info
            => $"Использование: {nameof(IntervalUpdate)} <интервал>.\n" +
            $"Задаёт миллисекунды, с какой переодичностью обновлять систему. " +
            $"Диапазон значений: от 0 до 1000. Допустимы только целые числа.";

        protected override void Action(string[] args)
        {
            if (args.Length < 1)
                Console.WriteLine($"{nameof(IntervalUpdate)} = {drawer.IntervalUpdate}");
            else if (ushort.TryParse(args[0], out ushort result))
                if (0 <= result && result <= 1000)
                    drawer.IntervalUpdate = result;
                else
                    Console.WriteLine("Допустимый диапазон: 0 ... 1000 миллисекунд.");
            else
                Console.WriteLine("Неудалось распознать в аргументе число.");
        }
    }
}