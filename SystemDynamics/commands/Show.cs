using System;

namespace SystemDynamics.commands
{
    /// <summary>
    /// Отображает полную информацию о текущем состоянии системы.
    /// </summary>
    internal class Show : AbstractCommand
    {
        private readonly WaterIceSteamState state;

        public Show(WaterIceSteamState state) : base(nameof(Show))
            => this.state = state;

        public override string Info
            => $"Отображает полную информацию о текущем состоянии системы.";

        protected override void Action(string[] args)
            => Console.WriteLine(state);
    }
}