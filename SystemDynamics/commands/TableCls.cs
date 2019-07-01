using System;
using System.Collections.Generic;

namespace SystemDynamics.commands
{
    internal class TableCls : AbstractCommand
    {
        private readonly List<string> table;

        public TableCls(List<string> table)
            : base(nameof(TableCls)) => this.table = table ?? throw new ArgumentNullException();

        public override string Info
            => "Очищает текущую таблицу.";

        protected override void Action(string[] args)
            => table.Clear();
    }
}