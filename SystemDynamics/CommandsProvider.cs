using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics
{
    class CommandsProvider
    {
        private readonly List<AbstractCommand> commands
            = new List<AbstractCommand>()
            {
                new commands.Edit()
            };
    }
}
