using SidStrikeEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace agents
{
    class Game1 : Game
    {
        private readonly List<Person> people = new List<Person>();
        private readonly VideoInterface background = new VideoInterface(0, 0) {
            texture = new Texture(string.Concat(Enumerable.Repeat(new string(' ', 500) + "\n", 500)), ConsoleColor.White, ConsoleColor.Black) };
        private readonly CharacteristicInterface<float> timeSpeed =
            new CharacteristicInterface<float>(
                16f * 60f * 60f, 0, 0, "speed: ", ConsoleColor.Cyan, ConsoleColor.Black,
                (float f) => f > 0.0f);
        private readonly CharacteristicInterface<float> adsForce = new CharacteristicInterface<float>(
            1.000f, 0, 1, "Сила рекламы: ", ConsoleColor.Cyan, ConsoleColor.Black,
            (float f) => f >= 0.0f && f <= 1.0f, (float f) => f.ToString("G3"));
        private readonly CharacteristicInterface<TimeSpan> deleveryTime =
            new CharacteristicInterface<TimeSpan>(TimeSpan.FromHours(24+12), 0, 2, "Время доставки: ",
                ConsoleColor.Cyan, ConsoleColor.Black);
        private readonly CharacteristicInterface<DateTime> timeInGame =
            new CharacteristicInterface<DateTime>(DateTime.MinValue, 0, 3, "Время: ",
                ConsoleColor.Black, ConsoleColor.White);
        private readonly CharacteristicInterface<TimeSpan> shelfLife =
            new CharacteristicInterface<TimeSpan>(TimeSpan.FromDays(5), 0, 4, "Срок годности: ",
                ConsoleColor.Cyan, ConsoleColor.Black);
        private readonly CharacteristicInterface<TimeSpan> talkSpan =
            new CharacteristicInterface<TimeSpan>(TimeSpan.FromHours(11), 0, 5, "Talk span: ",
                ConsoleColor.Cyan, ConsoleColor.Black);
        private readonly VideoInterface gameObjectLeft = new VideoInterface(0, 2);
        private readonly Random ran = new Random();

        public Game1()
        {
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize((int)(Console.LargestWindowWidth - 1), (int)(Console.LargestWindowHeight - 1));
            Console.WindowHeight = Console.BufferHeight;
            Console.WindowWidth = Console.BufferWidth;
            base.GState.Size = new Point(Console.WindowWidth, Console.WindowHeight);

            GoTick += Game1_GoTick;
            GoDraw += Game1_GoDraw;
            IsKeyPress += Game1_IsKeyPress;
            int w = base.GState.Size.X;
            Point p = new Point(0, 6);
            for (int i = 0; i < (base.GState.Size.Y - 6) * base.GState.Size.X; i++)
            {
                p.X += ran.Next(1, 3);
                if (p.X > w)
                {
                    p.X -= w;
                    p.Y++;
                }
                people.Add(new Person(p, this));
            }
            gameObjectLeft.texture = new Texture(" \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n",
                ConsoleColor.White, ConsoleColor.Black);
        }

        private void Game1_IsKeyPress(ConsoleKeyInfo KeyPress)
        {
            switch(KeyPress.Key)
            {
                case ConsoleKey.LeftArrow: camera.position.Add(-1, 0); break;
                case ConsoleKey.RightArrow: camera.position.Add(+1, 0); break;
                case ConsoleKey.UpArrow: camera.position.Add(0, -1); break;
                case ConsoleKey.DownArrow: camera.position.Add(0, +1); break;
                case ConsoleKey.F1: timeSpeed.Value *= (KeyPress.Modifiers == ConsoleModifiers.Shift ? 0.95f : 1.05f); break;
                case ConsoleKey.F2: adsForce.Value += 0.001f * (KeyPress.Modifiers == ConsoleModifiers.Shift ? -1.0f : 1.0f); break;
                case ConsoleKey.F3: deleveryTime.Value = new TimeSpan(1 + (long)(deleveryTime.Value.Ticks * (KeyPress.Modifiers == ConsoleModifiers.Shift ? 0.95 : 1.05))); break;
                case ConsoleKey.F4: shelfLife.Value = new TimeSpan(1 + (long)(shelfLife.Value.Ticks * (KeyPress.Modifiers == ConsoleModifiers.Shift ? 0.95 : 1.05))); break;
                case ConsoleKey.F5: talkSpan.Value = new TimeSpan(1 + (long)(talkSpan.Value.Ticks * (KeyPress.Modifiers == ConsoleModifiers.Shift ? 0.95 : 1.05))); break;
                case ConsoleKey.R:
                    {
                        Point must = new Point(Console.LargestWindowWidth, Console.LargestWindowHeight - 1);
                        while (!base.GState.Size.Equals(must))
                        {
                            base.GState.Size = new Point(Console.WindowWidth, Console.WindowHeight - 1);
                            System.Threading.Thread.Sleep(1);
                        }
                        Console.BufferWidth = Console.WindowWidth;
                    }
                    break;
            }
        }

        private Camera camera = new Camera();

        private void Game1_GoDraw(TimeSpan TimeOldDraw, GraphicState GState)
        {
            System.Threading.Thread.Sleep(1);
            background.Draw(GState);
            gameObjectLeft.Draw(GState);
            GState.camera.Set(camera.position.X, camera.position.Y);
            foreach (Person person in people)
                person.Draw(GState);
            adsForce.Draw(GState);
            timeSpeed.Draw(GState);
            deleveryTime.Draw(GState);
            timeInGame.Draw(GState);
            shelfLife.Draw(GState);
            talkSpan.Draw(GState);
        }

        private void Game1_GoTick(TimeSpan TimeOldTick)
        {
            System.Threading.Thread.Sleep(1);
            if(TimeOldTick.TotalHours < 24)
                timeInGame.Value = timeInGame.Value.AddSeconds(TimeOldTick.TotalSeconds * timeSpeed.Value);
            foreach (Person person in people)
                person.Tick(people);
            return;
        }

        /// <summary>
        /// Представление человека в системе.
        /// </summary>
        class Person : GameObject
        {
            /// <summary>
            /// Создание человека.
            /// </summary>
            /// <param name="position">Местоположение человека.</param>
            /// <param name="getAdsForce">Получение силы рекламной компании.
            /// Ожидается возвращаение от 0 до 1.</param>
            public Person(Point position, Game1 game)
            {
                this.position = position;
                this.State = STATE.not_want;
                this.game = game;
            }

            /// <summary>
            /// Всевозможные текстуры для разных состояний человека.
            /// </summary>
            private static readonly Texture[] textureContent = new Texture[] {
                // Просто человек.
                new Texture('@', ConsoleColor.White, ConsoleColor.Black),
                // Ждёт доставки.
                new Texture('$', ConsoleColor.Yellow, ConsoleColor.Black),
                // Пользуется.
                new Texture('!', ConsoleColor.Green, ConsoleColor.Black)
            };

            /// <summary>
            /// Возможные состояния человека.
            /// </summary>
            public enum STATE
            {
                not_want = 0,
                wait,
                use
            }
            
            private static readonly Random ran = new Random();
            private STATE state = STATE.not_want;
            private DateTime lastTalk = DateTime.MinValue;
            private DateTime timeOfStartDelevery;
            private DateTime timeOfGetPackage;

            /// <summary>
            /// Изменение или получение состояния человека.
            /// </summary>
            public STATE State
            {
                set
                {
                    if (value == STATE.wait)
                        timeOfStartDelevery = (DateTime)game.timeInGame;
                    state = value;
                    texture = textureContent[(byte)value];
                }
                get
                {
                    return state;
                }
            }

            private readonly Game1 game;

            public void Tick(ICollection<Person> people)
            {

                switch (State)
                {
                    case STATE.wait:
                        GetPackage();
                        break;
                    case STATE.use:
                        Use();
                        break;
                }
                Talk(people);
            }

            /// <summary>
            /// Имитация использования продукта.
            /// </summary>
            private void Use()
            {
                if ((DateTime)game.timeInGame - timeOfGetPackage > (TimeSpan)game.shelfLife)
                    State = STATE.not_want;
            }

            /// <summary>
            /// Симулирует получение поссылки.
            /// </summary>
            private void GetPackage()
            {
                if ((DateTime)game.timeInGame - timeOfStartDelevery > (TimeSpan)game.deleveryTime)
                {
                    timeOfGetPackage = game.timeInGame.Value;
                    State = STATE.use;
                }
            }

            /// <summary>
            /// Метод просит человека разговаривать с другими людьми.
            /// Человек сам с собой не разговаривает.
            /// Человек сам решает, начинать разговаривать, или нет.
            /// Человек сам решает, рассказывать о товаре, или нет.
            /// </summary>
            /// <param name="people"></param>
            public void Talk(ICollection<Person> people)
            {
                if ((DateTime)game.timeInGame - lastTalk < (TimeSpan)game.talkSpan)
                    // Слишком часто разговаривать нельзя.
                    return;
                lastTalk = (DateTime)game.timeInGame;
                int count = 0; // Количество разговоров.
                foreach (Person person in people)
                {
                    if (Equals(person)) continue;
                    if (ran.NextDouble() <= 0.0002f / (count)) // Шанс того, что заговорит.
                    {
                        count++;
                        switch (State)
                        {
                            /*
                             Шанс того, что человек заговорит о товаре не 100%.
                             */
                            case STATE.not_want:
                                if (ran.NextDouble() <= (0.001f + (float)game.adsForce) / 2.0f)
                                    person.TakeMessage(this);
                                break;
                            case STATE.wait:
                                if (ran.NextDouble() <= (0.8f + (float)game.adsForce) / 2.0f)
                                    person.TakeMessage(this);
                                break;
                            case STATE.use:
                                if (ran.NextDouble() <= (0.4f + (float)game.adsForce) / 2.0f)
                                    person.TakeMessage(this);
                                break;
                        }
                    }
                }
            }

            /// <summary>
            /// Вызывается, когда человеку приходит сообщение.
            /// </summary>
            /// <param name="from">Источник сообщения.</param>
            private void TakeMessage(Person from)
            {
                if (from.Equals(this) || this.State != STATE.not_want)
                    return;
                /*
                 Шанс того, что другой человек послушает его - не 100%.
                 */
                if (from.State == STATE.not_want && ran.NextDouble() <= (0.001f + (float)game.adsForce) / 2.0f)
                    State = STATE.wait;
                else if (from.State == STATE.wait && ran.NextDouble() <= (0.05f + (float)game.adsForce) / 2.0f)
                    State = STATE.wait;
                else if (from.State == STATE.use && ran.NextDouble() <= (0.2f + (float)game.adsForce) / 2.0f)
                    State = STATE.wait;
            }
        }

        class VisualTable : GameObject
        {
            public ConsoleColor ColorForeground
            {
                get
                {
                    return texture.Pixel[0, 0].ForegroundColor;
                }
                set
                {
                    for (int x = 0; x < texture.Pixel.GetLength(0); x++)
                        for (int y = 0; y < texture.Pixel.GetLength(1); y++)
                        {
                            texture.Pixel[x, y].ForegroundColor = value;
                        }
                }
            }
            private ConsoleColor ColorBackground
            {
                get
                {
                    return texture.Pixel[0, 0].BackgroundColor;
                }
                set
                {
                    for (int x = 0; x < texture.Pixel.GetLength(0); x++)
                        for (int y = 0; y < texture.Pixel.GetLength(1); y++)
                        {
                            texture.Pixel[x, y].BackgroundColor = value;
                        }
                }
            }

            public VisualTable(int x, int y, ConsoleColor ColorForeground = ConsoleColor.Gray, ConsoleColor ColorBackground = ConsoleColor.Black)
                : base(x, y)
            {
                this.ColorForeground = ColorForeground;
                this.ColorBackground = ColorBackground;
            }
        }

        sealed class CharacteristicInterface<T> : VideoInterface
        {
            public readonly string legend;
            private readonly ScreenPixel colorContainer;

            /// <summary>
            /// Создание легенды.
            /// </summary>
            /// <param name="value">Значение, которое надо отображать.</param>
            /// <param name="X">Положение текстуры от левого края.</param>
            /// <param name="X">Положение текстуры от верхнего края.</param>
            /// <param name="legend">Легенда с подписью значения.</param>
            /// <param name="ColorsForeground">Цвет заполнения.</param>
            /// <param name="ColorBackground">Цвет фона.</param>
            /// <param name="checkSet">Проверка записи в легенду.</param>
            /// <param name="toString">Правила преобразования хранимой характеристики.</param>
            public CharacteristicInterface(T value, int X, int Y, string legend, ConsoleColor ColorsForeground, ConsoleColor ColorBackground, Func<T, bool> checkSet = null, Func<T, string> toString = null)
            {
                base.texture = new DynamicTexture(legend, ColorsForeground, ColorBackground);
                this.colorContainer = new ScreenPixel()
                {
                    ForegroundColor = ColorsForeground,
                    BackgroundColor = ColorBackground
                };
                if (checkSet == null)
                    this.checkSet = (T t) => true;
                else
                    this.checkSet = checkSet;
                if (toString == null)
                    this.toString = (T t) => t.ToString();
                else
                    this.toString = toString;
                this.legend = legend;
                this.Value = value;
                base.position.Set(X, Y);
            }


            private readonly Func<T, bool> checkSet;
            private readonly Func<T, string> toString;
            private T _value;
            public T Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    if (checkSet(value))
                    {
                        _value = value;
                        string write = legend + toString(value);
                        if (write.Length > base.texture.Pixel.GetLength(1))
                        {
                            ((DynamicTexture)base.texture).Pixel =
                                new ScreenPixel[base.texture.Pixel.GetLength(0), write.Length];
                        }
                        for (int x = 0; x < base.texture.Pixel.GetLength(1); x++)
                            for (int y = 0; y < base.texture.Pixel.GetLength(0); y++)
                                base.texture.Pixel[y, x] = this.colorContainer;
                        ((DynamicTexture)(base.texture)).SetCharsFast(write.ToCharArray());
                    }
                }
            }

            public static explicit operator T(CharacteristicInterface<T> v)
            {
                return v.Value;
            }

            public T GetValue() => Value;
            public void SetValue(T t) => Value = t;
            
        }
    }
}
