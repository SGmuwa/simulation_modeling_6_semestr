using System;
using System.Collections;
using System.Collections.Generic;

namespace SystemDynamics
{
    class CommandsProvider : IEnumerable<AbstractCommand>
    {
        private readonly Drawer drawer;

        public CommandsProvider(Drawer drawer = null)
        {
            if (drawer == null)
                drawer = new Drawer();
            this.drawer = drawer;
            commands = new HashSet<AbstractCommand>()
            {
                new commands.Edit(drawer.state),
                new commands.Help(this),
                new commands.Exit(this),
                new commands.TableAdd(drawer),
                new commands.TableShow(drawer.Table),
                new commands.Show(drawer.state)
            };
        }

        public void Start()
        {
            IsNeedStop = false;
            System.Threading.Tasks.Task.Run((Action)drawer.Run);
            while(!IsNeedStop)
            {
                string text = Console.ReadLine();
                string[] textWithoutSpaces = GetNameAndArgs(text);
                if (textWithoutSpaces.Length < 1)
                    continue;
                string[] args = new string[textWithoutSpaces.Length - 1];
                new List<string>(textWithoutSpaces).CopyTo(1, args, 0, textWithoutSpaces.Length - 1);
                string nameCommand = textWithoutSpaces[0];
                bool found = false;
                foreach(AbstractCommand command in this)
                    found |= command.Invoke(nameCommand, args);
            }
            drawer.Stop();
        }

        private string[] GetNameAndArgs(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        public bool IsNeedStop { get; set; } = true;

        private readonly HashSet<AbstractCommand> commands;

        public IEnumerator<AbstractCommand> GetEnumerator()
            => commands.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => commands.GetEnumerator();
    }
}
