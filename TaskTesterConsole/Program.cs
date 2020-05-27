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

            Parallel.Invoke((() =>
            {
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


                    Console.WriteLine("[IНФО] - Запуск системи тестування... ");


                    Compile(config, ".cpp");
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Помилка] - Полимлка! [" +  e.Message +"]");
                    return;
                }



            }));

        

            Console.ReadLine();

        }


        public static void Compile(Config config, string type)
        {
            var outputFile = "main.exe";


            var countOfPassed = 0;






            if (File.Exists(config.Source + type))


                Console.WriteLine("[iНФО] - Файл знайдено.");

                Process process = new System.Diagnostics.Process();
                ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = config.CPPCompiler + ' ' + config.Source + ".cpp" +
                                      " -static-libstdc++ -static-libgcc -o " + outputFile;


                Console.WriteLine("[IНФО] - Копiляцiя...");


                process.StartInfo = startInfo;
                process.Start();

                Thread.Sleep(2000);
                if (File.Exists(outputFile))
                {
                    Console.WriteLine("Файл " + outputFile + " створено!");


                    for (int i = 1; i <= config.CountOfTests; i++)
                    {
                        Console.WriteLine();
                        Console.WriteLine("==============================");

                        if (TaskExecutor(outputFile, "output.rez", "test" + i + ".test", config.TimeLimit, "test" + i + ".dat"))
                        {
                            Console.WriteLine("[Успiх] - Тест " + i + " успiшно пройдено.");
                            countOfPassed++;
                        }
                        else
                        {
                            Console.WriteLine("[Фейл] - Тест " + i + " провалено.");
                        }

                        Console.WriteLine("[Прогрес] - " + Convert.ToDouble(i) / config.CountOfTests * 100 + "%");
                        Console.WriteLine("==============================");
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("======== СТАТИСТИКА ========");

                    Console.WriteLine("Кiлькiсть успiшних тестiв: " + countOfPassed + " з " + config.CountOfTests + " - " +
                                      Convert.ToDouble(countOfPassed) / config.CountOfTests * 100 + "%");

                    File.Delete(outputFile);
                }
                else
                {
                    Console.WriteLine("[Помилка] - файл ("+ config.Source + type +") не знайдено!!!");
                }

        }

    


        public static bool  TaskExecutor (string programName, string resultFile, string testName, int timeLimit, string inputTestFile)
        {

            var resultData = "";
            var testData = "";

            if (!File.Exists("INPUT//" + inputTestFile))
            {
                Console.WriteLine("[Помилка] - тестовий файл (" + inputTestFile + ") не знайдено!");
                return false;
            }

            using (var stream = File.Open("input.dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
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






            if (!File.Exists(resultFile))
                return false;
            else
                resultData = File.ReadAllText(resultFile);

            if (resultData == testData)
            {
                File.Delete(resultFile);
                File.Delete("input.dat");
                return true;
            }
            else
            {
                File.Delete(resultFile);
                File.Delete("input.dat");
                return false;
            }
              

        }
    }
}
