using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace WIS.ClearLog
{
    public class Program
    {
        public static IConfiguration configuration;

        static async Task Main(string[] args)
        {
            try
            {
                ConsoleWriteLine("Inicio");

                configuration = new ConfigurationBuilder()
                  .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .Build();

                var paths = configuration.GetSection("AppSettings:PathLog").Get<List<string>>();
                var extension = configuration.GetSection("AppSettings:Extension").Value;
                var cantidadDias = 0;

                if (int.TryParse(configuration.GetSection("AppSettings:CantidadDias").Value, out int cantidad))
                {
                    cantidadDias = cantidad;
                }

                foreach (var path in paths)
                {
                    var comando = $"ForFiles /p \"{path}\" /s /m *{extension} /d -{cantidadDias}  /c \"cmd /c del @file\"";
                    ExecuteCommand(comando);
                }

                ConsoleWriteLine("Fin");
            }
            catch (Exception ex)
            {
                ConsoleWriteLine("Error: " + ex.ToString());
                Console.ReadLine();
            }
        }

        public static void ConsoleWriteLine(string msg)
        {
            Console.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + " " + msg);
        }

        public static void ExecuteCommand(string _Command)
        {

            using (Process cmd = new Process())
            {
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine(_Command);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());
            }
        }
    }
}
