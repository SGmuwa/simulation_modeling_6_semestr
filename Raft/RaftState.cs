using System;
using System.Diagnostics;

namespace Raft
{
    class RaftState
    {
        private State currentState = State.LEFT_INTO;
        private Coast a;
        private Coast b;
        private int countPeopleOnRaft = 0;
        private double raftPosition = 0.0;

        private readonly Action<double> SetProgressBar;
        private readonly Action<string> SetLabel_Raft;
        private readonly Func<double> GetChanceAppearancePerson;
        private readonly Func<double> GetRaftSpeedPercentPerSecond;
        private readonly Func<int>    GetMaxPeopleInRaft;

        private readonly Random ran = new Random();
        private Stopwatch timerLogic = new Stopwatch();
        private Stopwatch timerVisual = new Stopwatch();


        public RaftState(
            Action<string> setLabel_A,
            Action<string> setLabel_B,
            Action<string> setLabel_Raft,
            Action<double> setProgressBar,
            Func<double> getChanceAppearancePerson,
            Func<double> getRaftSpeedPercentPerSecond,
            Func<int> getMaxPeopleInRaft)
        {
            a = new Coast() { SetLabel = setLabel_A };
            b = new Coast() { SetLabel = setLabel_B };

            SetProgressBar = setProgressBar;
            SetLabel_Raft = setLabel_Raft;
            GetChanceAppearancePerson = getChanceAppearancePerson;
            GetRaftSpeedPercentPerSecond = getRaftSpeedPercentPerSecond;
            GetMaxPeopleInRaft = getMaxPeopleInRaft;

            timerLogic.Start();
            timerVisual.Start();
        }

        public void UpdateLogic()
        {
            switch (currentState)
            {
                case State.LEFT_OUT:
                    if (countPeopleOnRaft > 0)
                    {
                        countPeopleOnRaft--;
                        a.CountPeopleFinish++;
                    }
                    else
                        currentState = State.LEFT_INTO;
                    break;
                case State.RIGHT_OUT:
                    if (countPeopleOnRaft > 0)
                    {
                        countPeopleOnRaft--;
                        b.CountPeopleFinish++;
                    }
                    else
                        currentState = State.RIGHT_INTO;
                    break;
                case State.LEFT_INTO:
                    if (countPeopleOnRaft < GetMaxPeopleInRaft() && a.CountPeople > 0)
                    {
                        a.CountPeople--;
                        countPeopleOnRaft++;
                    }
                    else if(countPeopleOnRaft > 0)
                        currentState = State.LEFT_TO_RIGHT;
                    break;
                case State.RIGHT_INTO:
                    if (countPeopleOnRaft < GetMaxPeopleInRaft() && b.CountPeople > 0)
                    {
                        b.CountPeople--;
                        countPeopleOnRaft++;
                    }
                    else if (countPeopleOnRaft > 0)
                        currentState = State.RIGHT_TO_LEFT;
                    break;
            }
            if (ran.NextDouble() < GetChanceAppearancePerson())
                a.CountPeople++;
            if (ran.NextDouble() < GetChanceAppearancePerson())
                b.CountPeople++;
        }

        public void UpdateVisual()
        {
            if (currentState == State.LEFT_TO_RIGHT || currentState == State.RIGHT_TO_LEFT)
            {
                raftPosition += (currentState == State.LEFT_TO_RIGHT ? 1 : -1) * (timerVisual.Elapsed.TotalSeconds * GetRaftSpeedPercentPerSecond());
                if (raftPosition >= 1.0)
                {
                    currentState = State.RIGHT_OUT;
                    raftPosition = 1.0;
                }
                if (raftPosition <= 0.0)
                {
                    currentState = State.LEFT_OUT;
                    raftPosition = 0.0;
                }
                SetProgressBar(raftPosition);
            }
            a.UpdateLabel();
            b.UpdateLabel();
            SetLabel_Raft(ToString());
            timerVisual.Restart();
        }

        public override string ToString()
            => $"People: {countPeopleOnRaft}, State: {currentState}, Pos: {raftPosition:N2}";

        enum State
        {
            LEFT_INTO,
            LEFT_TO_RIGHT,
            RIGHT_OUT,
            RIGHT_INTO,
            RIGHT_TO_LEFT,
            LEFT_OUT
        }
    }

    struct Coast
    {
        public int CountPeople;
        public int CountPeopleFinish;
        public Action<string> SetLabel;

        public void UpdateLabel()
            => SetLabel(ToString());

        public override string ToString()
            => $"Ждут:\n{CountPeople}\nПриехало:\n{CountPeopleFinish}";
    }
}
