using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleExample
{
    public class FileGenerator
    {
        private string CurrentDirectory { get; }
        private int FilesToGenerate { get; }
        private int NumbersToGenerate { get; }

        public FileGenerator(int filesToGenerate, int numbersToGenerate)
        {
            FilesToGenerate = filesToGenerate;
            NumbersToGenerate = numbersToGenerate;
            CurrentDirectory = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(CurrentDirectory, "asyncTest")))
            {
                Directory.CreateDirectory(Path.Combine(CurrentDirectory, "asyncTest"));
            }

            if (!Directory.Exists(Path.Combine(CurrentDirectory, "syncTest")))
            {
                Directory.CreateDirectory(Path.Combine(CurrentDirectory, "syncTest"));
            }
        }

        public async Task GenerateFilesAsync()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < FilesToGenerate; i++)
            {
                tasks.Add(GenerateFileAsync($"{i}.txt"));
            }

            await Task.WhenAll(tasks);
        }

        private async Task GenerateFileAsync(string file)
        {
            await using var stream = new StreamWriter(Path.Combine(CurrentDirectory, $"asyncTest/{file}"));
            var text = new StringBuilder();
            for (var i = 0; i < NumbersToGenerate; i++)
            {
                text.Append(i * i);
                text.Append("\n");
            }

            await stream.WriteAsync(text);
        }

        public void GenerateFiles()
        {
            for (var i = 0; i < FilesToGenerate; i++)
            {
                GenerateFile($"{i}.txt");
            }
        }

        private void GenerateFile(string file)
        {
            using var stream = new StreamWriter(Path.Combine(CurrentDirectory, $"syncTest/{file}"));
            var text = "";
            for (var i = 0; i < NumbersToGenerate; i++)
            {
                text += i * i;
                text += "\n";
            }

            stream.Write(text);
        }
    }

    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var watch = new Stopwatch();
                var generator = new FileGenerator(int.Parse(args[0]), int.Parse(args[1]));
                watch.Start();
                Task.Run(async () => await generator.GenerateFilesAsync()).Wait();
                Console.WriteLine(
                    $"на асинхронное выполнение задачи потрачено {watch.ElapsedMilliseconds} миллисекунд");
                watch.Restart();
                generator.GenerateFiles();
                Console.WriteLine($"на синхронное выполнение задачи потрачено {watch.ElapsedMilliseconds} миллисекунд");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}