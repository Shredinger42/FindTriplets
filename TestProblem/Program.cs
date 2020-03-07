using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace TestProblem
{
    class Program
    {
        static void GetTriplets(string content, Dictionary<char, int> result)
        {
            var words = content.Split(' ');
            foreach (var word in words)
            {
                var wordArray = word.ToLower().ToCharArray();
                for (int i = 0; i < wordArray.Length - 2; i++)
                {
                    if (Char.IsLetter(wordArray[i]) 
                        && wordArray[i] == wordArray[i + 1] 
                        && wordArray[i] == wordArray[i + 2])
                    {
                        result[wordArray[i]] = result.ContainsKey(wordArray[i]) ? ++result[wordArray[i]] : 1;
                        i += 2;
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var isWrongPath = true;
            string content = null;
            while (isWrongPath)
            {
                try
                {
                    Console.WriteLine("Введите путь к файлу:");
                    var filePath = Console.ReadLine();
                    content = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                    isWrongPath = false;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Указан неверный путь к файлу.");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Указанный файл не найден.");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("Указанная директория не найдена");
                }
            }
            var result = new Dictionary<char, int>();
            var searchingThread = new Thread(() => GetTriplets(content, result));
            searchingThread.Start();
            var startTime = DateTime.UtcNow;
            while (true)
            {
                if (searchingThread.IsAlive)
                {
                    Console.Write("\r{0} мс", (DateTime.UtcNow - startTime).TotalMilliseconds);
                    if (Console.KeyAvailable)
                    {
                        searchingThread.Abort();
                    }
                }
                else
                {
                    break;
                }
            }
            Console.WriteLine();
            if(result.Count == 0)
            {
                Console.WriteLine("В тексте нет триплетов.");
                return;
            }
            Console.WriteLine("Часто встречаемые триплеты в тексте:");
            foreach (var pair in result.OrderByDescending(pair => pair.Value).Take(10))
            {
                Console.Write("{0} ", pair.Key);
            }
            Console.WriteLine();
        }
    }
}
