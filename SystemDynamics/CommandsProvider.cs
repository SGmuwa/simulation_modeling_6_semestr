using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics
{
    class CommandsProvider : IEnumerable<AbstractCommand>
    {
        private readonly Drawer drawer;

        public CommandsProvider(Drawer drawer = null)
        {
            if (drawer == null)
                drawer = new Drawer();
            this.drawer = drawer;
            commands = new HashSet<AbstractCommand>()
            {
                new commands.Edit(drawer.state),
                new commands.Help(this),
                new commands.Exit(this),
                new commands.TableAdd(drawer),
                new commands.TableShow(drawer.Table),
                new commands.Show(drawer.state),
                new commands.UpdateInterval(drawer)
            };
        }

        public void Start()
        {
            IsNeedStop = false;
            System.Threading.Tasks.Task.Run((Action)drawer.Run);
            while (!IsNeedStop)
            {
                InvokeText(GetterText());
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
                    ; // not support
                else
                    sb.Append(info.KeyChar);
                buffer = ShowUserCommandText(sb, posStart, buffer);
            } while (info.Key != ConsoleKey.Enter);
            if (GetCommandAndArgsFromText(sb.ToString(), out string commandName, out string[] args))
            {
                List<AbstractCommand> recommendCommands = SearchCommands(commandName);
                if (recommendCommands.Count == 1)
                    return recommendCommands[0].Name + " " + string.Join(" ", args);
            }
            return sb.ToString();
        }

        private StringBuilder ShowUserCommandText(StringBuilder sb, dynamic posStart, StringBuilder stringOld)
        {
            string sbString = sb.ToString();
            StringBuilder output = new StringBuilder(sbString);
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
                List<AbstractCommand> commands = SearchCommands(v);
                if (commands.Count == 0)
                    return "not found.";
                else if (commands.Count == 1)
                    return commands[0].Name + " - " + commands[0].Info;
                else
                    return string.Join(", ", commands);
            }
            else return "(?)";
        }

        private List<AbstractCommand> SearchCommands(string v)
        {
            Dictionary<AbstractCommand, int> pairs = new Dictionary<AbstractCommand, int>(commands.Count);
            foreach(AbstractCommand command in commands)
            {
                pairs[command] = GetDistance(command.Name, v);
            }
            int minDistance = Minimum(pairs.Values);
            List<AbstractCommand> output = new List<AbstractCommand>();
            foreach(KeyValuePair<AbstractCommand, int> pair in pairs)
            {
                if (pair.Value <= minDistance)
                    output.Add(pair.Key);
            }
            return output;
        }

        private int GetDistance(string ConstantText, string UserText)
        {
            if (ConstantText.Equals(UserText, StringComparison.InvariantCultureIgnoreCase))
                return 0;
            if (ConstantText.Contains(UserText, StringComparison.InvariantCultureIgnoreCase))
                return 1;
            else
                return GetLevenshteinDistance(ConstantText.ToLower(), UserText.ToLower()) + 2;
        }

        int depth = 0;

        private int GetLevenshteinDistance(string first, string second)
        {
            if (++depth > 500)
            {
                depth--;
                return int.MaxValue / 2;
            }

            int cost;

            /* base case: empty strings */
            if (first.Length == 0) return second.Length;
            if (second.Length == 0) return first.Length;

            /* test if last characters of the strings match */
            if (first[first.Length - 1] == second[second.Length - 1])
                cost = 0;
            else
                cost = 1;

            /* return minimum of delete char from s, delete char from t, and delete char from both */
            int output = Minimum(GetDistance(first.Substring(0, first.Length - 1), second) + 1,
                           GetDistance(first, second.Substring(0, second.Length - 1)) + 1,
                           GetDistance(first.Substring(0, first.Length - 1), second.Substring(0, second.Length - 1)) + cost);
            depth--;
            return output;
        }

        private int Minimum(IEnumerable<int> values)
        {
            int min = 0;
            bool first = true;
            foreach (int v in values)
            {
                if(first)
                {
                    min = v;
                    first = false;
                    continue;
                }
                if (v < min)
                    min = v;
            }
            return min;
        }

        private int Minimum(params int[] values)
        {
            if (values.Length <= 0)
                throw new ArgumentException();
            int min = values[0];
            for (int i = 1; i < values.Length; i++)
                if (values[i] < min)
                    min = values[i];
            return min;
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
