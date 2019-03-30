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
        private readonly VideoInterface gameObjectSpeed = new VideoInterface(0, 0);
        private readonly VideoInterface gameObjectAdsForce = new VideoInterface(0, 1);

        public Game1()
        {
            GoTick += Game1_GoTick;
            GoDraw += Game1_GoDraw;
            IsKeyPress += Game1_IsKeyPress;
            people.Add(new Person(new Point(4, 5), GetAdsForce, GetTalkSpan));
            people.Add(new Person(new Point(4, 7), GetAdsForce, GetTalkSpan));
            gameObjectSpeed.texture = new DynamicTexture("Speed:\0\0\0\0\0\0\0\0\0\0\0", ConsoleColor.Blue, ConsoleColor.Black);
            gameObjectAdsForce.texture = new DynamicTexture("AdsForce:   %", ConsoleColor.Blue, ConsoleColor.Black);
            TimeSpeed = 60f * 60f * 16f;
            AdsForce = 0.2f;
        }

        private void Game1_IsKeyPress(ConsoleKeyInfo KeyPress)
        {
            switch(KeyPress.Key)
            {
                case ConsoleKey.LeftArrow: camera.position.Add(-1, 0); break;
                case ConsoleKey.RightArrow: camera.position.Add(+1, 0); break;
                case ConsoleKey.UpArrow: camera.position.Add(0, -1); break;
                case ConsoleKey.DownArrow: camera.position.Add(0, +1); break;
                case ConsoleKey.F1: TimeSpeed += 1 * (KeyPress.Modifiers == ConsoleModifiers.Shift ? -1 : 1); break;
                case ConsoleKey.F2: AdsForce += 0.002f * (KeyPress.Modifiers == ConsoleModifiers.Shift ? -1.0f : 1.0f); break;
            }
        }

        private TimeSpan talkSpan = new TimeSpan(0, 16, 0, 0, 0);
        private TimeSpan GetTalkSpan() => TimeSpan.FromTicks((long)(talkSpan.Ticks / TimeSpeed));
        private Camera camera = new Camera();


        private float timeSpeed;
        public float TimeSpeed
        {
            get => timeSpeed; set
            {
                if (float.IsNaN(value) || value < 0)
                    return;
                timeSpeed = value;
                gameObjectSpeed.texture.SetInteger((int)timeSpeed, 6, 0);
            }
        }

        private float adsForce;
        private float GetAdsForce() => AdsForce;
        public float AdsForce
        {
            get => adsForce; set
            {
                if (float.IsNaN(value) || value < 0)
                    return;
                adsForce = value;
                gameObjectAdsForce.texture.Pixel[0, 9].Pixel = gameObjectAdsForce.texture.Pixel[0, 10].Pixel = gameObjectAdsForce.texture.Pixel[0, 10].Pixel = '\0';
                gameObjectAdsForce.texture.SetInteger((int)(adsForce*100), 9, 0);
            }
        }

        private void Game1_GoDraw(TimeSpan TimeOldDraw, GraphicState GState)
        {
            GState.camera.Set(camera.position.X, camera.position.Y);
            foreach (Person person in people)
                person.Draw(GState);
            gameObjectAdsForce.Draw(GState);
            gameObjectSpeed.Draw(GState);

        }

        private void Game1_GoTick(TimeSpan TimeOldTick)
        {
            foreach (Person person in people)
                person.Talk(people);
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
            public Person(Point position, Func<float> getAdsForce, Func<TimeSpan> getTalkSpan)
            {
                this.position = position;
                this.getAdsForce = getAdsForce;
                this.State = STATE.not_want;
                this.getTalkSpan = getTalkSpan;
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
                foreach(Person person in people)
                {
                    if (Equals(person)) continue;
                    switch(State)
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
        

    }
}
