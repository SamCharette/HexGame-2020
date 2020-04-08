using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;

namespace WindowsGame
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("Config.json", true, true)
                .Build();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Game(config));
        }
    }
}