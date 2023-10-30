using System.Xml.Serialization;

namespace ConsoleApp3
{
    internal class ReadnSave
    {
        public static void Read(string path) 
        {
            Console.Clear();
            Console.WriteLine("можно сохранить файл в одном из форматов (txt, json, xml) - F1. закрыть - Escape\r\n-------------------------------------------");

            List<Notes> result = new();

            if (path.Contains(".txt"))
            {
                string[] txt = File.ReadAllLines(path);

                Notes newNotes = new();

                string authorMarkerWord = "автор: ";

                for (int i = 0; i < txt.Length; i++)  //stshk
                {
                    if (txt[i].Contains(authorMarkerWord))
                    {
                        newNotes.author = txt[i].Replace(authorMarkerWord, "") + "\r\n";
                    }
                }

                for (int j = 0; j < txt.Length / 3; j++)
                {
                    txt[2 + (j * 3)] += "\r\n";
                    newNotes.date[j] = txt[2 + (j * 3)];
                }

                for (int k = 0; k < txt.Length / 3; k++)
                {
                    txt[3 + (k * 3)] += "\r\n";
                    newNotes.content[k] = txt[3 + (k * 3)];
                }

                result.Add(newNotes); 

                foreach (Notes notes in result)
                {
                    Console.WriteLine("->автор: " + notes.author);

                    for (int j = 0; j < notes.date.Length; j++)
                    {
                        Console.WriteLine("  " + notes.date[j] + "  " + notes.content[j]);
                    }

                    Console.SetCursorPosition(2, 2);
                }

            }

            else if ((path.Contains(".xml")) || (path.Contains(".json")))
            {
                if (path.Contains(".xml"))
                {
                    XmlSerializer xml = new(typeof(List<Notes>));

                    using FileStream fs = new(path, FileMode.Open);

                    result = (List<Notes>)xml.Deserialize(fs);
                }

                else if (path.Contains(".json"))
                {
                    string json = File.ReadAllText(path);

                    result = JsonConvert.DeserializeObject<List<Notes>>(json);
                }

                foreach (Notes notes in result)
                {
                    Console.WriteLine("->автор: " + notes.author + "\r\n");

                    for (int j = 0; j < notes.date.Length; j++)
                    {
                        Console.WriteLine(notes.date[j] + notes.content[j]);
                    }

                    Console.SetCursorPosition(0, 2);
                }
            }

            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.F1)
                {
                    Save(path, result);
                }

                else if (key.Key == ConsoleKey.Escape)  //stshk
                {
                    break;
                }

                else if ((key.Key == ConsoleKey.UpArrow) || (key.Key == ConsoleKey.DownArrow) || (key.Key == ConsoleKey.Enter))
                {
                    result = CursorMove(key, result);
                }

            } while (true);
        }
        static void LineFindToStringEdit(List<Notes> result, int[] tops, int lineIndex)
        {
            int[] ints = new int[tops.Length];

            for (int i = 0; i <= tops.Length - 1; i++)
            {
                ints[i] = i;
            }

            string additionalString = "  ";

            if (lineIndex == 0)
            {
                additionalString = "  автор: ";

                result[0].author = StringEdit(result[0].author, additionalString, tops[0]);
            }

            else if (lineIndex % 2 != 0)
            {
                result[0].date[lineIndex / 2] = StringEdit(result[0].date[lineIndex / 2], additionalString, tops[lineIndex]);
            }

            else if (lineIndex % 2 == 0)
            {
                result[0].content[lineIndex / 2 - 1] = StringEdit(result[0].content[lineIndex / 2 - 1], additionalString, tops[lineIndex]);
            }
        }
        static List<Notes> CursorMove(ConsoleKeyInfo key, List<Notes> result)
        {
            int[] tops = new int[1 + Program.init * 2];
            tops[0] = 2;
            int[] tops2 = new int[1 + Program.init * 2];
            tops2[0] = 2;

            int[] contentLenght = new int[Program.init + 1];

            for (int i = 0; i < contentLenght.Length - 1; i++)  //stshk
            {
                contentLenght[i + 1] = result[0].content[i].Length / 120;
            }

            for (int i = 0; i < contentLenght.Length - 1; i++)
            {
                contentLenght[i + 1] += contentLenght[i];
            }

            for (int i = 0; i < Program.init; i++)
            {
                tops[1 + i * 2] = 4 + i * 3 + contentLenght[i];
                tops[2 + i * 2] = 5 + i * 3 + contentLenght[i];
            }

            for (int i = 0; i < Program.init; i++)
            {
                tops2[i + 1] = 4 + i * 3 + contentLenght[i];
                tops2[i + 7] = 5 + i * 3 + contentLenght[i];
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                if (!(lineIndex == 0))
                {
                    lineIndex--;
                }

                Console.SetCursorPosition(0, tops[lineIndex + 1]);
                Console.Write("  ");
            }

            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (!(lineIndex == tops.Length - 1))
                {
                    lineIndex++;
                }

                Console.SetCursorPosition(0, tops[lineIndex - 1]);
                Console.Write("  ");
            }

            else if (key.Key == ConsoleKey.Enter)
            {
                LineFindToStringEdit(result, tops, lineIndex);
            }

            Console.SetCursorPosition(0, tops[lineIndex]);
            Console.Write("->");

            return result;
        }
        static void Save(string path, List<Notes> result)
        {
            Console.Clear();
            Console.WriteLine("можно сохранить файл в одном из форматов (txt, json, xml) - F1. закрыть - Escape\r\n-----------------------------------------------------------\r\n");

            path = StringEdit(path, "", 2).Replace("\r\n", "");
            Console.SetCursorPosition(0, 3);

            if (File.Exists(path))
            {
                Console.WriteLine("нет файла с таким названием, для прдолжения нажмите любую клавишу");
                ConsoleKeyInfo key = Console.ReadKey(true);
            }

            if (path.Contains(".txt"))
            {
                string txt = "";

                foreach (Notes notes in result)
                {
                    txt += "автор: " + notes.author + "\r\n";

                    for (int i = 0; i < notes.date.Length; i++)  //stshk
                    {
                        txt += notes.date[i] + notes.content[i] + "\r\n";
                    }
                }

                File.WriteAllText(path, txt);
            }

            else if (path.Contains(".xml"))
            {
                XmlSerializer xml = new(typeof(List<Notes>));
                using FileStream fs = new(path, FileMode.Create);
                xml.Serialize(fs, result);
            }

            else if (path.Contains(".json"))
            {
                string json = JsonConvert.SerializeObject(result);
                File.WriteAllText(path, json);
            }

            Console.WriteLine("все получилось");
        }

        static int lineIndex = 0;

        static string StringEdit(string @string, string additionalString, int cursorTop)
        {
            ConsoleKeyInfo key;

            do
            {
                Console.SetCursorPosition(additionalString.Length, cursorTop);

                for (int i = 0; i < @string.Length + 1; i++)
                {
                    Console.Write(" ");
                }

                Console.SetCursorPosition(0, cursorTop);

                Console.Write(additionalString + @string);


                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace)
                {
                    @string = @string.Remove(@string.Length - 1, 1);
                }

                else if (key.Key == ConsoleKey.Enter)  //stshk
                {

                }

                else
                {
                    @string += key.KeyChar;
                }

            } while (key.Key != ConsoleKey.Enter);

            return @string + "\r\n";
        }
    }
}