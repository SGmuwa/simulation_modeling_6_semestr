using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics.commands
{
    class Edit : AbstractCommand
    {
        public Edit(WaterIceSteamState state) : base("edit") => State = state;

        private readonly Dictionary<string, GetSet<double>> Properties = new Dictionary<string, GetSet<double>>()
        {
            { "ТемператураПлавленияЛьда".ToLower(), new GetSet<double>((state, v) => state.ТемператураПлавленияЛьда = v, (state) => state.ТемператураПлавленияЛьда) },
            { "ТемператураКипенияВоды".ToLower(), new GetSet<double>((state, v) => state.ТемператураКипенияВоды = v, (state) => state.ТемператураКипенияВоды) },
            { "УдельнаяТеплотаПлавления".ToLower(), new GetSet<double>((state, v) => state.УдельнаяТеплотаПлавления = v, (state) => state.УдельнаяТеплотаПлавления) },
            { "УдельнаяТеплотаПарообразования".ToLower(), new GetSet<double>((state, v) => state.УдельнаяТеплотаПарообразования = v, (state) => state.УдельнаяТеплотаПарообразования) },
            { "ТеплоёмкостьЛьда".ToLower(), new GetSet<double>((state, v) => state.ТеплоёмкостьЛьда = v, (state) => state.ТеплоёмкостьЛьда) },
            { "ТеплоёмкостьЖидкости".ToLower(), new GetSet<double>((state, v) => state.ТеплоёмкостьЖидкости = v, (state) => state.ТеплоёмкостьЖидкости) },
            { "ТеплоёмкостьПара".ToLower(), new GetSet<double>((state, v) => state.ТеплоёмкостьПара = v, (state) => state.ТеплоёмкостьПара) },
            { "МощностьНагревателя".ToLower(), new GetSet<double>((state, v) => state.МощностьНагревателя = v, (state) => state.МощностьНагревателя) },
            { "КоличествоДжоулей".ToLower(), new GetSet<double>((state, v) => state.КоличествоДжоулей = v, (state) => state.КоличествоДжоулей) },
            { "ИзначальнаяМасса".ToLower(), new GetSet<double>((state, v) => state.ИзначальнаяМасса = v, (state) => state.ИзначальнаяМасса) }
        };

        private readonly WaterIceSteamState State;

        protected override void Action(string[] args)
        {
            if (args.LongLength < 2)
            {
                Console.WriteLine("Слишком мало аргументов. Используйте /help.");
                return;
            }
            if (args[0] == null || args[1] == null)
            {
                Console.WriteLine("Аргументы переданы с указателем null.");
                throw new ArgumentNullException();
            }
            Result<GetSet<double>> property = SearchProperty(args[0]);
            if(!property.Ok)
                Console.WriteLine(property.Problem);
            else if(double.TryParse(args[1], out double value))
                try
                {
                    property.Value.Set(State, value);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            else
                Console.WriteLine("Вторым аргументом ожидалась цифра double. Например: " + Math.PI);
        }

        /// <summary>
        /// Ищет значение определения по части ключа среди ключей в <see cref="Properties"/>.
        /// </summary>
        /// <param name="str">Часть ключа.</param>
        /// <returns>В случае удачи - параметры изменения.</returns>
        private Result<GetSet<double>> SearchProperty(string str)
        {
            str = str.ToLower();
            foreach(string key in Properties.Keys)
            {
                if(key.StartsWith(str))
                {
                    return new Result<GetSet<double>>(Properties[key]);
                }
            }
            return new Result<GetSet<double>>(new KeyNotFoundException("Не удалось найти ключ, который начинается на: " + str));
        }

        public override string Info
            => $"Изменяет параметры системы. " +
            $"Использование: \"{Name} <имя_параметра> <значение>. " +
            $"Имя параметра можно задать первыми уникальными символами. " +
            $"Доступные для изменения:\n{string.Join("\n", Properties.Keys)}";

        private struct GetSet<T>
        {
            public GetSet(Action<WaterIceSteamState, T> set, Func<WaterIceSteamState, T> get)
            {
                Set = set;
                Get = get;
            }

            public Action<WaterIceSteamState, T> Set;
            public Func<WaterIceSteamState, T> Get;
        }
    }
}
