using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;

namespace TheForestWrapper
{

    class Program
    {
        //\\76561198064553744
        static string saveDir;
        static string saveName;
        static string userName;
        static string gameDir;

        static void Main(string[] args)
        {

            var envName = Environment.GetEnvironmentVariable("production");

            var services = new ServiceCollection();
            Console.WriteLine(Directory.GetCurrentDirectory());
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            ;


            saveDir = config.GetSection("App").GetSection("SaveDirectory").Value;
            saveName = config.GetSection("App").GetSection("SaveName").Value;
            userName = config.GetSection("App").GetSection("UserName").Value;
            gameDir = config.GetSection("App").GetSection("GameDirectory").Value;


            if (!CheckGitExist()) {
                Console.WriteLine("Veuillez installer Git avant de continuer.");
                return;
            }

            if (RepoNotFound())
            {
                Clone();
            }
            else
            {
                Pull();
            }

            
            StartProcess();
        }

        static bool CheckGitExist()
        {
            return Directory.Exists("C:\\Program Files\\Git");
        }

        static bool RepoNotFound()
        {
            return !Directory.Exists(saveDir + "\\" + saveName) || !Directory.Exists(saveDir + "\\" + saveName + "\\" + ".git");
            
        }

        static void Init()
        {

        }

        static void Clone()
        {

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = saveDir,
                    FileName = "CMD.exe",
                    Arguments = "/c git clone https://github.com/thegrondin/TheForest_Save.git",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

            var proc2 = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = saveDir,
                    FileName = "CMD.exe",
                    Arguments = "/c ren TheForest_Save " + saveName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc2.Start();
            while (!proc2.StandardOutput.EndOfStream)
            {
                string line = proc2.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
        }

        static void Pull()
        {
          
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = saveDir + "\\" + saveName,
                    FileName = "CMD.exe",
                    Arguments = "/c git pull",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

        }

        static void Push()
        {

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = saveDir + "\\" + saveName,
                    FileName = "CMD.exe",
                    Arguments = "/c git push -u origin main",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }

        }

        static void Commit()
        {

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = saveDir + "\\" + saveName,
                    FileName = "CMD.exe",
                    Arguments = "/c git add .",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }


            var proc2 = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = saveDir + "\\" + saveName,
                    FileName = "CMD.exe",
                    Arguments = "/c git commit -m \"" + userName + "_SAVE\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc2.Start();
            while (!proc2.StandardOutput.EndOfStream)
            {
                string line = proc2.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }


        }

        static void StartProcess()
        {

            Console.WriteLine(gameDir + "\\TheForest.exe");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = gameDir + "\\TheForest.exe";
            startInfo.WorkingDirectory = Path.GetDirectoryName(gameDir + "\\TheForest.exe");

            try
            {
                Console.WriteLine("Ouverture du jeu ...");
                using (Process exeProcess = Process.Start(startInfo))
                {
                    Console.WriteLine("Le jeu est en cours d'execution");
                    exeProcess.WaitForExit();

                    Commit();
                    Push();
                }
            }
            catch
            {
                Console.WriteLine("Erreur dans L'ouverture de jeu");
            }

        }
    }
}
