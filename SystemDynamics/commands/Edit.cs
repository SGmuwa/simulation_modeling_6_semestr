using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics.commands
{
    class Edit : AbstractCommand
    {
        public Edit(WaterIceSteamState state) : base("edit") => State = state;

        private readonly WaterIceSteamState State;

        protected override void Action(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
