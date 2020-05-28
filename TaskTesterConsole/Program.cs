using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskTesterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
                Console.Title = "TASK TESTER BETA";

                Logger("Привiт!", ConsoleColor.Cyan);
                Logger("Версiя: 0.1 BETA", ConsoleColor.Cyan);
                Logger(@"GIT репозиторiй: https://github.com/Andriy22/TaskTesterConsole", ConsoleColor.Cyan);
                Logger("Автор: Andriy Alexandruk", ConsoleColor.Cyan);
                Logger("Всi налаштування знаходяться в файлi [ CFG/config.json ]", ConsoleColor.DarkYellow);
                Console.WriteLine();
                Console.WriteLine();

                try
                {
                    var config = new Config();

                    if (!File.Exists("CFG/config.json"))
                    {
                        Console.WriteLine("[Помилка] - конфiг не знайдено!");
                        return;
                    }

                    var cfgData = File.ReadAllText("CFG/config.json");

                    config = JsonConvert.DeserializeObject<Config>(cfgData);

                    if (!config.SkipWelcome)
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                      
                    Console.Clear();
                    Logger("[IНФО] - Запуск системи тестування... ", ConsoleColor.DarkYellow);


                    Compile(config);

                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Logger("[Помилка] - Полимлка![" +  e.Message +"]", ConsoleColor.Red);
                    return;
                }




        }


        public static void Compile(Config config)
        {
            var outputFile = "main.exe";


            var countOfPassed = 0;








            Logger("[iНФО] - Файл знайдено.", ConsoleColor.Green);

                Process process = new System.Diagnostics.Process();
                ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                if (File.Exists(config.Source + ".cpp"))
                {
                    startInfo.Arguments = config.CPPCompiler + ' ' + config.Source + ".cpp" +
                                          " -static-libstdc++ -static-libgcc -o " + outputFile;
                }
                   
                else if (File.Exists(config.Source + ".pas"))
                {
                    outputFile = config.Source + ".exe";
                    startInfo.Arguments = config.PascalCompiler + ' ' + config.Source + ".pas";
                }
                   


                Logger("[iНФО] - Компiляцiя...", ConsoleColor.DarkYellow);


                process.StartInfo = startInfo;
                process.Start();

                Thread.Sleep(3000);
                if (File.Exists(outputFile))
                {
                    Logger("Файл " + outputFile + " створено!", ConsoleColor.Green);

                    Logger("[INFO] - Будь ласка, зачекайте! Запускаємо тестування...", ConsoleColor.Cyan);
                    
                    Thread.Sleep(1500);
                    
                  
                    
                    for (int i = 1; i <= config.CountOfTests; i++)
                    {
                        Console.WriteLine();
                        Console.WriteLine("==============================");

                        if (TaskExecutor(outputFile,  config.TestOutputName + i + config.TestOutputType, config.TimeLimit, config.TestInputName + i + config.TestInputType, config))
                        {
                            Logger("[Успiх] - Тест " + i + " успiшно пройдено.", ConsoleColor.Green);
                            countOfPassed++;
                        }
                        else
                        { 
                            Logger("[Фейл] - Тест " + i + " провалено.", ConsoleColor.Red);
                        }

                        Console.WriteLine("[Прогрес] - " + Convert.ToDouble(i) / config.CountOfTests * 100 + "%");
                        Console.WriteLine("==============================");
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("======== СТАТИСТИКА ========");

                    Console.WriteLine("Кiлькiсть успiшних тестiв: " + countOfPassed + " з " + config.CountOfTests + " - " +
                                      Convert.ToDouble(countOfPassed) / config.CountOfTests * 100 + "%");
                    Console.WriteLine();
                    File.Delete(outputFile);


                    
                }
                else
                {
                    Console.WriteLine("[Помилка] - файл ("+ config.Source +".exe) не знайдено!!!");
                }

        }

    


        public static bool  TaskExecutor (string programName, string testName, int timeLimit, string inputTestFile, Config config)
        {

            var resultData = "";
            var testData = "";

            if (!File.Exists("INPUT//" + inputTestFile))
            {
                Console.WriteLine("[Помилка] - тестовий файл (" + inputTestFile + ") не знайдено!");
                return false;
            }

            using (var stream = File.Open(config.InputFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                stream.Write(Encoding.UTF8.GetBytes(File.ReadAllText("INPUT//" + inputTestFile)));
            }




            Process program = new System.Diagnostics.Process();
            var programInfo = new System.Diagnostics.ProcessStartInfo();
            programInfo.FileName = "cmd.exe";
            programInfo.Arguments = "/c start " + programName;


          





            program.StartInfo = programInfo;
            program.Start();

            Thread.Sleep(timeLimit);


            program.Kill(true);

            if (!File.Exists("OUTPUT//" + testName))
            {
                Console.WriteLine("[Помилка] - тестовий файл (" + testName + ") не знайдено!");
                return false;
            }

            else
                testData = File.ReadAllText("OUTPUT//" + testName);






            if (!File.Exists(config.OutputFile))
                return false;
            else
                resultData = File.ReadAllText(config.OutputFile);

            if (resultData.Trim() == testData.Trim())
            {
                File.Delete(config.OutputFile);
                File.Delete(config.InputFile);
                return true;
            }
            else
            {
                File.Delete(config.OutputFile);
                File.Delete(config.InputFile);
                return false;
            }
              

        }



        private static void Logger(object text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text.ToString());
            Console.ResetColor();
        }
    }
}
