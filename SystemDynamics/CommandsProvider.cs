using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics
{
    class CommandsProvider : IEnumerable<AbstractCommand>
    {
        private readonly Drawer drawer;
        private readonly History<string> history
            = new History<string>();

        public CommandsProvider(Drawer drawer = null)
        {
            if (drawer == null)
                drawer = new Drawer();
            this.drawer = drawer;
            commands = new HashSet<AbstractCommand>()
            {
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за температуру плавления льда.", nameof(drawer.state.ТемператураПлавленияЛьда), (state, v) => state.ТемператураПлавленияЛьда = v, (state) => state.ТемператураПлавленияЛьда, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за температуру кипения воды.", nameof(drawer.state.ТемператураКипенияВоды), (state, v) => state.ТемператураКипенияВоды = v, (state) => state.ТемператураКипенияВоды, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за удельную теплоту плавления.", nameof(drawer.state.УдельнаяТеплотаПлавления), (state, v) => state.УдельнаяТеплотаПлавления = v, (state) => state.УдельнаяТеплотаПлавления, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за удельную теплоту парообразования.", nameof(drawer.state.УдельнаяТеплотаПарообразования), (state, v) => state.УдельнаяТеплотаПарообразования = v, (state) => state.УдельнаяТеплотаПарообразования, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за теплоёмкость льда.", nameof(drawer.state.ТеплоёмкостьЛьда), (state, v) => state.ТеплоёмкостьЛьда = v, (state) => state.ТеплоёмкостьЛьда, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за теплоёмкость жидкости.", nameof(drawer.state.ТеплоёмкостьЖидкости), (state, v) => state.ТеплоёмкостьЖидкости = v, (state) => state.ТеплоёмкостьЖидкости, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за теплоёмкость пара.", nameof(drawer.state.ТеплоёмкостьПара), (state, v) => state.ТеплоёмкостьПара = v, (state) => state.ТеплоёмкостьПара, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за мощноть нагревателя.", nameof(drawer.state.МощностьНагревателя), (state, v) => state.МощностьНагревателя = v, (state) => state.МощностьНагревателя, double.TryParse, (d) => true),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Отвечает за текущее количество джоулей в системе. Может быть только неотрицательное число.", nameof(drawer.state.КоличествоДжоулей), (state, v) => state.КоличествоДжоулей = v, (state) => state.КоличествоДжоулей, double.TryParse, (d) => d >= 0),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Суммарная масса вещества. Не может быть равна нулю. Можно попробовать указать отрицательную массу.", nameof(drawer.state.ИзначальнаяМасса), (state, v) => state.ИзначальнаяМасса = v, (state) => state.ИзначальнаяМасса, double.TryParse, (d) => d != 0),
                new commands.Edit<WaterIceSteamState, double>(drawer.state, "Используется за ускорение, замедление, поворот времени.", nameof(drawer.state.MultiplicationTime), (state, v) => state.MultiplicationTime = v, (state) => state.MultiplicationTime, double.TryParse, (d) => double.IsNormal(d)),
                new commands.Edit<Drawer, int>(drawer, "Задаёт миллисекунды, с какой переодичностью обновлять систему. Диапазон значений: от 0 до 1000. Допустимы только целые числа.", nameof(drawer.IntervalUpdate), (d, i) => d.IntervalUpdate = i, (d) => d.IntervalUpdate, int.TryParse, (d) => 0 <= d && d <= 1000),
                new commands.Edit<Drawer, TimeSpan>(drawer, "Период добавления информации в таблицу.", nameof(drawer.TableSpan), (dr, sp) => dr.TableSpan = sp, (dr) => dr.TableSpan, TimeSpan.TryParse, (ts) => true),

                new commands.Help(this),
                new commands.Exit(this),
                new commands.TableAdd(drawer),
                new commands.TableShow(drawer.Table),
                new commands.TableCls(drawer.Table),
                new commands.Show(drawer.state)
            };
        }

        public void Start()
        {
            IsNeedStop = false;
            System.Threading.Tasks.Task.Run((Action)drawer.Run);
            while (!IsNeedStop)
            {
                history.Add(GetterText());
                InvokeText(history.Last.Value);
            }
            drawer.Stop();
        }

        private string GetterText()
        {
            StringBuilder sb = new StringBuilder();
            ConsoleKeyInfo info;
            var posStart = new { X = Console.CursorLeft, Y = Console.CursorTop };
            StringBuilder buffer = null;
            do
            {
                info = Console.ReadKey(true);
                if (ConsoleKey.Backspace == info.Key)
                {
                    if (sb.Length > 0)
                        sb.Length--;
                }
                else if (ConsoleKey.Enter == info.Key)
                    break;
                else if (info.Key == ConsoleKey.UpArrow || info.Key == ConsoleKey.DownArrow || info.Key == ConsoleKey.LeftArrow || info.Key == ConsoleKey.RightArrow)
                    sb = new StringBuilder(history.Move(info).Value);
                else
                    sb.Append(info.KeyChar);
                buffer = ShowUserCommandText(sb, posStart, buffer);
            } while (info.Key != ConsoleKey.Enter);
            if (GetCommandAndArgsFromText(sb.ToString(), out string commandName, out string[] args))
            {
                List<AbstractCommand> recommendCommands = SearchRecommendations.Search(commandName, (t) => t.Name, commands);
                if (recommendCommands.Count == 1)
                    return recommendCommands[0].Name + " " + string.Join(" ", args);
            }
            return sb.ToString();
        }

        private StringBuilder ShowUserCommandText(StringBuilder sb, dynamic posStart, StringBuilder stringOld)
        {
            string sbString = sb.ToString();
            StringBuilder output = new StringBuilder("> ");
            output.Append(sbString);
            output.AppendLine();
            if (GetCommandAndArgsFromText(sbString, out string commandName, out string[] args))
            {
                output.Append(GetRecommendedInfo(commandName));
                output.AppendLine();
            }
            StringBuilder stringNew = output;
            if (stringOld != null)
            {
                for (int i = 0; i < stringOld.Length; i++)
                    if (stringOld[i] != '\n')
                        stringOld[i] = ' ';
                Console.SetCursorPosition(posStart.X, posStart.Y);
                Console.Write(stringOld.ToString());
            }
            Console.SetCursorPosition(posStart.X, posStart.Y);
            Console.Write(stringNew.ToString());
            return stringNew;
        }

        private string GetRecommendedInfo(string v)
        {
            if (GetCommandAndArgsFromText(v, out string nameCommand, out string[] args))
            {
                List<AbstractCommand> commands = SearchRecommendations.Search(nameCommand, (t) => t.Name, this.commands);
                if (commands.Count == 0)
                    return "not found.";
                else if (commands.Count == 1)
                    return commands[0].Name + " - " + commands[0].Info;
                else
                    return string.Join(", ", commands);
            }
            else return "(?)";
        }
        
        /// <summary>
        /// Преобразовывает входной текст в команду и вызывает этку команду.
        /// </summary>
        /// <param name="text">Текст, который надо преобразовать в команду.</param>
        private void InvokeText(string text)
        {
            if(GetCommandAndArgsFromText(text, out string nameCommand, out string[] args))
                InvokeCommands(nameCommand, args);
        }

        private bool GetCommandAndArgsFromText(string text, out string commandName, out string[] args)
        {
            commandName = null;
            args = null;
            if (text == null)
                return false;
            string[] textWithoutSpaces = GetNameAndArgs(text);
            if (textWithoutSpaces.Length < 1)
                return false;
            args = new string[textWithoutSpaces.Length - 1];
            new List<string>(textWithoutSpaces).CopyTo(1, args, 0, textWithoutSpaces.Length - 1);
            commandName = textWithoutSpaces[0].ToLower();
            return true;
        }

        private void InvokeCommands(string nameCommand, string[] args)
        {
            bool found = false;
            foreach (AbstractCommand command in this)
                found |= command.Invoke(nameCommand, args);
            if (!found)
                Console.WriteLine($"Команда \"{nameCommand}\" не найдена.");
        }

        private string[] GetNameAndArgs(string text)
        {
            return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        public bool IsNeedStop { get; set; } = true;

        private readonly HashSet<AbstractCommand> commands;

        public IEnumerator<AbstractCommand> GetEnumerator()
            => commands.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => commands.GetEnumerator();
    }
}
