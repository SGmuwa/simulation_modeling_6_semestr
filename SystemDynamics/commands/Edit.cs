using System;

namespace SystemDynamics.commands
{
    class Edit<ClassType, PropertyType> : AbstractCommand where ClassType : class
    {
        /// <summary>
        /// Создание экземпляра редактирования свойства.
        /// </summary>
        /// <param name="state">Ссылка на объект, свойство которого надо менять.</param>
        /// <param name="infoProperty">Справочная информация о меняемом свойстве объекта.</param>
        /// <param name="nameProperty">Имя свойства объекта.</param>
        /// <param name="set">Функция присваивания значения в свойство.</param>
        /// <param name="get">Функция получения значения из свойства.</param>
        /// <param name="tryParse">Функция преобразования текста в тип свойства.</param>
        public Edit(ClassType state, string infoProperty, string nameProperty, Action<ClassType, PropertyType> set, Func<ClassType, PropertyType> get, TryParseType tryParse, Func<PropertyType, bool> isFine)
            : this(state, new GetSet(nameProperty, infoProperty, set, get, tryParse, isFine)) { }

        /// <summary>
        /// Создание экземпляра редактирования свойства.
        /// </summary>
        /// <param name="state">Ссылка на объект, свойство которого надо менять.</param>
        /// <param name="property">Информация о меняемом свойстве.</param>
        public Edit(ClassType state, GetSet property)
            : base($"{nameof(Edit<ClassType, PropertyType>)}:{property.Name}")
        {
            State = state ?? throw new ArgumentNullException($"Поле {nameof(state)} должно быть не равно null.");
            Property = property;
        }

        /// <summary>
        /// Информация о объекте, чьё свойство меняется.
        /// </summary>
        private readonly ClassType State;

        protected override void Action(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("Аргументы переданы с указателем null.");
                throw new ArgumentNullException();
            }
            if (args.Length == 0)
                try
                {
                    Console.WriteLine($"{Property.Name} = {Property.Get(State)}");
                }
                catch
                {
                    Console.WriteLine("Не удалось получить значение свойства.");
                }
            else if (Property.TryParse(args[0], out PropertyType value))
                try
                {
                    if (Property.IsFine(value))
                        Property.Set(State, value);
                    else
                        Console.WriteLine("Не удалось изменить значение свойства.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            else
                Console.WriteLine($"Не удалось преобразовать входной текст в тип {typeof(PropertyType).Name}.");
        }

        /// <summary>
        /// Информация о свойстве.
        /// </summary>
        private readonly GetSet Property;

        /// <summary>
        /// Справочная информация.
        /// </summary>
        public override string Info
            => $"Изменяет параметр \"{Property.Name}\" системы. " +
            $"Использование: {Name} <значение>.\n" +
            $"Справка: {Property.Info}";

        /// <summary>
        /// Класс представляет собой прототип функции TryParse всех типов без форматирования.
        /// </summary>
        /// <param name="input">Входной текст, который надо преобразовать в тип <typeparamref name="PropertyType"/>.</param>
        /// <param name="result">Преобразованные данные из текста.</param>
        /// <returns>True, если преобразование было успешно. Иначе - false.</returns>
        public delegate bool TryParseType(string input, out PropertyType result);

        public struct GetSet
        {
            /// <summary>
            /// Создание информации о свойстве.
            /// </summary>
            /// <param name="set">Функция присваивания свойства.</param>
            /// <param name="get">Функция получения из свойства значения.</param>
            /// <param name="tryParse">Функция преобразования текста в тип данных свойства.</param>
            /// <param name="isFine">True, если можно задать этому свойству значение. Иначе - False.</param>
            public GetSet(Action<ClassType, PropertyType> set, Func<ClassType, PropertyType> get, TryParseType tryParse, Func<PropertyType, bool> isFine)
                : this("", "", set, get, tryParse, isFine) { }

            /// <summary>
            /// Создание информации о свойстве.
            /// </summary>
            /// <param name="name">Имя свойства.</param>
            /// <param name="info">Справочная информация о свойстве.</param>
            /// <param name="set">Функция присваивания свойства.</param>
            /// <param name="get">Функция получения из свойства значения.</param>
            /// <param name="tryParse">Функция преобразования текста в тип данных свойства.</param>
            /// <param name="isFine">True, если можно задать этому свойству значение. Иначе - False.</param>
            public GetSet(string name, string info, Action<ClassType, PropertyType> set, Func<ClassType, PropertyType> get, TryParseType tryParse, Func<PropertyType, bool> isFine)
            {
                Name = name;
                Info = info;
                Set = set;
                Get = get;
                TryParse = tryParse;
                IsFine = isFine;
            }

            /// <summary>
            /// Имя свойства.
            /// </summary>
            public string Name;
            /// <summary>
            /// Справочная информация о свойстве.
            /// </summary>
            public string Info;
            /// <summary>
            /// Функция присваивания свойства.
            /// </summary>
            public Action<ClassType, PropertyType> Set;
            /// <summary>
            /// Функция получения из свойства значения.
            /// </summary>
            public Func<ClassType, PropertyType> Get;
            /// <summary>
            /// Функция преобразования текста в тип данных свойства.
            /// </summary>
            public TryParseType TryParse;
            /// <summary>
            /// True, если можно задать этому свойству значение. Иначе - False.
            /// </summary>
            public Func<PropertyType, bool> IsFine;
        }
    }
}
