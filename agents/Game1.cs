using SidStrikeEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agents
{
    class Game1 : Game
    {
        private readonly List<Person> people = new List<Person>();
        private readonly CharacteristicInterface<float> timeSpeed =
            new CharacteristicInterface<float>(
                16f * 60f * 60f, 0, 0, "speed: ", ConsoleColor.Cyan, ConsoleColor.Black,
                (float f) => f > 0.0f);
        private readonly VideoInterface gameObjectAdsForce = new VideoInterface(0, 1);
        private readonly VideoInterface gameObjectLeft = new VideoInterface(0, 2);
        private readonly Random ran = new Random();

        public Game1()
        {
            GoTick += Game1_GoTick;
            GoDraw += Game1_GoDraw;
            IsKeyPress += Game1_IsKeyPress;
            int w = base.GState.Size.X;
            Point p = new Point(0, 2);
            for (int i = 0; i < 5; i++)
            {
                p.X += ran.Next(1, 3);
                if (p.X > w)
                {
                    p.X -= w;
                    p.Y++;
                }
                people.Add(new Person(p, GetAdsForce, GetTalkSpan, GetShelfLife, () => new TimeSpan(1, 0, 0)));
            }
            gameObjectAdsForce.texture = new DynamicTexture("AdsForce:   %", ConsoleColor.Blue, ConsoleColor.Black);
            gameObjectLeft.texture = new Texture(" \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n \n",
                ConsoleColor.White, ConsoleColor.Black);
            AdsForce = 0.000f;
        }

        private TimeSpan ShelfLife { get; set; }
        private TimeSpan GetShelfLife()
        {
            return TimeSpan.FromTicks((long)(ShelfLife.Ticks / timeSpeed.Value));
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
                case ConsoleKey.F2: AdsForce += 0.002f * (KeyPress.Modifiers == ConsoleModifiers.Shift ? -1.0f : 1.0f); break;
                case ConsoleKey.R: base.GState.Size = new Point(Console.LargestWindowWidth, Console.LargestWindowHeight - 1); break;
            }
        }

        private TimeSpan talkSpan = new TimeSpan(0, 16, 0, 0, 0);
        private TimeSpan GetTalkSpan() => TimeSpan.FromTicks((long)(talkSpan.Ticks / (float)timeSpeed));
        private Camera camera = new Camera();

        private float adsForce;
        private float GetAdsForce() => AdsForce;
        public float AdsForce
        {
            get => adsForce; set
            {
                if (float.IsNaN(value) || value < 0 || value > 1)
                    return;
                adsForce = value;
                gameObjectAdsForce.texture.Pixel[0, 9].Pixel = gameObjectAdsForce.texture.Pixel[0, 10].Pixel = gameObjectAdsForce.texture.Pixel[0, 10].Pixel = '\0';
                gameObjectAdsForce.texture.SetInteger((int)(adsForce*100), 9, 0);
            }
        }

        private void Game1_GoDraw(TimeSpan TimeOldDraw, GraphicState GState)
        {
            System.Threading.Thread.Sleep(1);
            gameObjectLeft.Draw(GState);
            GState.camera.Set(camera.position.X, camera.position.Y);
            foreach (Person person in people)
                person.Draw(GState);
            gameObjectAdsForce.Draw(GState);
            timeSpeed.Draw(GState);
        }

        private void Game1_GoTick(TimeSpan TimeOldTick)
        {
            System.Threading.Thread.Sleep(1);
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
            public Person(Point position, Func<float> getAdsForce, Func<TimeSpan> getTalkSpan, Func<TimeSpan> GetShelfLife, Func<TimeSpan> GetDelivery)
            {
                this.position = position;
                this.getAdsForce = getAdsForce;
                this.State = STATE.not_want;
                this.getTalkSpan = getTalkSpan;
                this.GetDelivery = GetDelivery;
                this.GetShelfLife = GetShelfLife;
            }

            /// <summary>
            /// Всевозможные текстуры для разных состояний человека.
            /// </summary>
            private static readonly Texture[] textureContent = new Texture[] {
                new Texture('@', ConsoleColor.White, ConsoleColor.Black),
                new Texture('$', ConsoleColor.Yellow, ConsoleColor.Black),
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

            /// <summary>
            /// Получение силы рекламной компании от внешней среды.
            /// </summary>
            private readonly Func<float> getAdsForce;
            private readonly Func<TimeSpan> GetShelfLife;
            private readonly Func<TimeSpan> GetDelivery;
            private static readonly Random ran = new Random();
            private STATE state = STATE.not_want;
            private DateTime lastTalk = DateTime.Now;
            /// <summary>
            /// Получение переодичности разговоров.
            /// </summary>
            private readonly Func<TimeSpan> getTalkSpan;

            /// <summary>
            /// Изменение или получение состояния человека.
            /// </summary>
            public STATE State
            {
                set
                {
                    state = value;
                    texture = textureContent[(byte)value];
                }
                get
                {
                    return state;
                }
            }
            public void Tick(ICollection<Person> people)
            {
                GetPackage();
                Use();
                Talk(people);
            }

            /// <summary>
            /// Имитация использования продукта.
            /// </summary>
            private void Use()
            {
                if (DateTime.Now - lastTalk < GetShelfLife())
                    State = STATE.not_want;
            }

            /// <summary>
            /// Симулирует получение поссылки.
            /// </summary>
            private void GetPackage()
            {
                if (DateTime.Now - lastTalk < GetDelivery())
                    State = STATE.use;
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
                if (DateTime.Now - lastTalk < getTalkSpan())
                    // Слишком часто разговаривать нельзя.
                    return;
                int count = 0; // Количество разговоров.
                foreach (Person person in people)
                {
                    if (Equals(person)) continue;
                    if (ran.NextDouble() <= 0.020f / (count)) // Шанс того, что заговорит.
                    {
                        count++;
                        switch (State)
                        {
                            /*
                             Шанс того, что человек заговорит о товаре не 100%.
                             */
                            case STATE.not_want:
                                if (ran.NextDouble() <= (0.001f + getAdsForce()) / 2.0f)
                                    person.TakeMessage(this);
                                break;
                            case STATE.wait:
                                if (ran.NextDouble() <= (0.8f + getAdsForce()) / 2.0f)
                                    person.TakeMessage(this);
                                break;
                            case STATE.use:
                                if (ran.NextDouble() <= (0.4f + getAdsForce()) / 2.0f)
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
                if (from.State == STATE.not_want && ran.NextDouble() <= (0.001f + getAdsForce()) / 2.0f)
                    State = STATE.wait;
                else if (from.State == STATE.wait && ran.NextDouble() <= (0.05f + getAdsForce()) / 2.0f)
                    State = STATE.wait;
                else if (from.State == STATE.use && ran.NextDouble() <= (0.2f + getAdsForce()) / 2.0f)
                    State = STATE.wait;
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
            public CharacteristicInterface(T value, int X, int Y, string legend, ConsoleColor ColorsForeground, ConsoleColor ColorBackground, Func<T, bool> checkSet)
            {
                base.texture = new DynamicTexture(legend, ColorsForeground, ColorBackground);
                this.colorContainer = new ScreenPixel()
                {
                    ForegroundColor = ColorsForeground,
                    BackgroundColor = ColorBackground
                };
                this.checkSet = checkSet;
                this.legend = legend;
                this.Value = value;
                base.position.Set(X, Y);
            }


            private readonly Func<T, bool> checkSet;
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
                        string write = legend + value;
                        if (write.Length > base.texture.Pixel.GetLength(1))
                        {
                            ((DynamicTexture)base.texture).Pixel =
                                new ScreenPixel[base.texture.Pixel.GetLength(0), write.Length];
                            for (int x = 0; x < base.texture.Pixel.GetLength(1); x++)
                                for (int y = 0; y < base.texture.Pixel.GetLength(0); y++)
                                        base.texture.Pixel[y, x] = this.colorContainer;
                        }
                        ((DynamicTexture)(base.texture)).SetCharsFast((legend + value).ToCharArray());
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
