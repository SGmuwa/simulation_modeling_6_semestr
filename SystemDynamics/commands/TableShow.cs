using System;
using System.Collections.Generic;

namespace SystemDynamics.commands
{
    internal class TableShow : AbstractCommand
    {
        private readonly List<string> table;

        public TableShow(List<string> table) : base("TableShow")
            => this.table = table;

        public override string Info
            => $"Показывает текущую таблицу.";

        protected override void Action(string[] args)
            => Console.WriteLine(string.Join("\n", table));
    }
}