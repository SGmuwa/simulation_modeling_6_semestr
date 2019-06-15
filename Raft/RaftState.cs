using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Raft
{
    /// <summary>
    /// Класс, в котором описывается система берег-плот-берег.
    /// </summary>
    class RaftState
    {
        /// <summary>
        /// Текущее поведение плота.
        /// </summary>
        private State currentState = State.LEFT_INTO;
        /// <summary>
        /// Информация о береге А.
        /// </summary>
        private Coast a;
        /// <summary>
        /// Информация о береге Б.
        /// </summary>
        private Coast b;
        /// <summary>
        /// Текущее количество людей на плоту.
        /// </summary>
        private int countPeopleOnRaft = 0;
        /// <summary>
        /// Положение плота в пространстве.
        /// </summary>
        private double raftPosition = 0.0;

        /// <summary>
        /// Функция для отправки текущего положения плота во внешнюю среду.
        /// </summary>
        private readonly Action<double> SetProgressBar;
        /// <summary>
        /// Функция для отправки текстового описания текущего состояния плота во внешнюю среду.
        /// </summary>
        private readonly Action<string> SetLabel_Raft;
        /// <summary>
        /// Функция получает шанс появления нового человека на берегу из внешней системы.
        /// Ожидается значение от 0 до 1.
        /// </summary>
        private readonly Func<double> GetChanceAppearancePerson;
        /// <summary>
        /// Функция получает скорость плота из внешней системы.
        /// Ожидается значение от 0 до 1.
        /// </summary>
        private readonly Func<double> GetRaftSpeedPercentPerSecond;
        /// <summary>
        /// Получает максимальное количество человек на плоту.
        /// Ожидается от 1 до <see cref="int.MaxValue"/>.
        /// </summary>
        private readonly Func<int>    GetMaxPeopleInRaft;

        /// <summary>
        /// Хранит в себе генератор случайных чисел.
        /// </summary>
        private readonly Random ran = new Random();
        /// <summary>
        /// Секудндомер, который предназначен для отчёта времени между двумя кадрами.
        /// </summary>
        private Stopwatch timerVisual = new Stopwatch();

        /// <summary>
        /// Инициализация состояния берег-плот-берег.
        /// </summary>
        /// <param name="setLabel_A">Функция присовения текстового описания первого берега.</param>
        /// <param name="setLabel_B">Функция присовения текстового описания второго берега.</param>
        /// <param name="setLabel_Raft">Функция присвоения текстового описания плота.</param>
        /// <param name="setProgressBar">Функция присвоения положения плота от 0 до 1.</param>
        /// <param name="getChanceAppearancePerson">Функция получения шанса появления человека на берегу.
        /// Шанс появления человека на обоих берегах одинаковый.</param>
        /// <param name="getRaftSpeedPercentPerSecond">Функция получения скорости плота. Измеряется в единицах: % поля в секунду. Ожидается от 0 до <see cref="double.MaxValue"/>.</param>
        /// <param name="getMaxPeopleInRaft">Получает максимальное количество человек на плоту. Ожидается от 1 до <see cref="int.MaxValue"/>.</param>
        public RaftState(
            Action<string> setLabel_A,
            Action<string> setLabel_B,
            Action<string> setLabel_Raft,
            Action<double> setProgressBar,
            Func<double> getChanceAppearancePerson,
            Func<double> getRaftSpeedPercentPerSecond,
            Func<int> getMaxPeopleInRaft)
        {
            a = new Coast(setLabel_A);
            b = new Coast(setLabel_B);

            SetProgressBar = setProgressBar;
            SetLabel_Raft = setLabel_Raft;
            GetChanceAppearancePerson = getChanceAppearancePerson;
            GetRaftSpeedPercentPerSecond = getRaftSpeedPercentPerSecond;
            GetMaxPeopleInRaft = getMaxPeopleInRaft;

            timerVisual.Start();
        }

        /// <summary>
        /// Обновляет поведение плота и берегов.
        /// </summary>
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

        /// <summary>
        /// Отправляет состояние плота во внешнюю среду.
        /// </summary>
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

        /// <summary>
        /// Получение состояния плота в текстовом представлении.
        /// </summary>
        public override string ToString()
            => $"Позиция: {raftPosition:N2}, Количество: {countPeopleOnRaft},\nСостояние: {StateStringer.GetDescription(currentState)}";

        
    }

    /// <summary>
    /// Состояния плота.
    /// </summary>
    enum State
    {
        /// <summary>
        /// Загрузка в А.
        /// </summary>
        [Description("Загрузка в А")]
        LEFT_INTO,
        /// <summary>
        /// Отправление слева направо.
        /// </summary>
        [Description("Отправление слева направо")]
        LEFT_TO_RIGHT,
        /// <summary>
        /// Разгрузка в Б.
        /// </summary>
        [Description("Разгрузка в Б")]
        RIGHT_OUT,
        /// <summary>
        /// Загрузка в Б.
        /// </summary>
        [Description("Загрузка в Б")]
        RIGHT_INTO,
        /// <summary>
        /// Отправление справа налево.
        /// </summary>
        [Description("Отправление справа налево")]
        RIGHT_TO_LEFT,
        /// <summary>
        /// Разгрузка в А.
        /// </summary>
        [Description("Разгрузка в А")]
        LEFT_OUT
    }

    static class StateStringer
    {
        /// <summary>
        /// Приведение значения перечисления в удобочитаемый формат.
        /// </summary>
        /// <remarks>
        /// Для корректной работы необходимо использовать атрибут [Description("Name")] для каждого элемента перечисления.
        /// </remarks>
        /// <param name="enumElement">Элемент перечисления</param>
        /// <returns>Название элемента</returns>
        public static string GetDescription(Enum enumElement)
        {
            Type type = enumElement.GetType();

            MemberInfo[] memInfo = type.GetMember(enumElement.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumElement.ToString();
        }
    }

    /// <summary>
    /// Структура представляет собой информацию о береге.
    /// </summary>
    struct Coast
    {
        /// <summary>
        /// Инициализация информации о береге.
        /// </summary>
        /// <param name="setLabel">Функция, с помощью которой отправялется информация о плоте во внешнюю среду.</param>
        public Coast(Action<string> setLabel)
        {
            CountPeople = 0;
            CountPeopleFinish = 0;
            SetLabel = setLabel;
        }

        /// <summary>
        /// Количество людей, готовых отплыть в другой берег.
        /// </summary>
        public int CountPeople;
        /// <summary>
        /// Статистика показывает, сколько людей прибыло на берег за весь период.
        /// </summary>
        public long CountPeopleFinish;
        /// <summary>
        /// Функция, с помощью которой отправялется информация о плоте во внешнюю среду.
        /// </summary>
        private readonly Action<string> SetLabel;

        /// <summary>
        /// Отправляет текущее состояние берега во внешнюю систему.
        /// </summary>
        public void UpdateLabel()
            => SetLabel(ToString());

        /// <summary>
        /// Текстовое представление о береге.
        /// </summary>
        public override string ToString()
            => $"Ждут:\n{CountPeople}\nПриехало:\n{CountPeopleFinish}";
    }
}
