using System;

namespace SystemDynamics
{
    abstract class AbstractCommand
    {
        protected AbstractCommand(string name)
            => Name = name ?? throw new ArgumentNullException();

        /// <summary>
        /// Имя команды.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Запуск выполнения команды с проверкой соответсвия её имени.
        /// </summary>
        /// <param name="name">Имя команды.</param>
        /// <param name="args">Аргументы команды.</param>
        public void Invoke(string name, string[] args)
        {
            if (Name.Equals(name))
                Action(args);
        }

        /// <summary>
        /// Запуск тела команды.
        /// </summary>
        /// <param name="args">Аргументы команды.</param>
        protected abstract void Action(string[] args);
    }
}
