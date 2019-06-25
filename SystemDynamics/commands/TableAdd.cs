using System;

namespace SystemDynamics.commands
{
    internal class TableAdd : AbstractCommand
    {
        private readonly Drawer drawer;

        public TableAdd(Drawer drawer) : base("TableAdd") => this.drawer = drawer ?? throw new ArgumentNullException();

        public override string Info
            => $"Сохраняет текущее краткое состояние в таблицу.";

        protected override void Action(string[] args)
            => drawer.Table.Add(drawer.state.ToString(false));
    }
}