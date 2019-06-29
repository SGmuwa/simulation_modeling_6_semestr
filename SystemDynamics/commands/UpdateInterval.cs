using System;

namespace SystemDynamics.commands
{
    internal class UpdateInterval : AbstractCommand
    {
        private readonly Drawer drawer;

        public UpdateInterval(Drawer drawer) : base(nameof(UpdateInterval)) => this.drawer = drawer;

        public override string Info
            => $"Использование: {nameof(UpdateInterval)} <интервал>.\n" +
            $"Задаёт миллисекунды, с какой переодичностью обновлять систему. " +
            $"Диапазон значений: от 0 до 1000. Допустимы только целые числа.";

        protected override void Action(string[] args)
        {
            if (args.Length < 1)
                Console.WriteLine("Недостаточно количество аргументов. Используйте один аргумент. Получено: " + args.Length);
            else if (ushort.TryParse(Console.ReadLine(), out ushort result))
                if (0 <= result && result <= 1000)
                    drawer.IntervalUpdate = result;
                else
                    Console.WriteLine("Допустимый диапазон: 0 ... 1000 миллисекунд.");
            else
                Console.WriteLine("Неудалось распознать в аргументе число.");
        }
    }
}