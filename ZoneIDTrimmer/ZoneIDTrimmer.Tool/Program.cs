using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ZoneIDTrimmer.Shared;
using ZoneIDTrimmer.Tool.Properties;
using ZoneIDTrimmer.View;

namespace ZoneIDTrimmer.Tool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.ThreadException += OnGuiUnhandedException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            if (args.Length == 0)
            {
                var path = Environment.GetCommandLineArgs().FirstOrDefault();
                var text = String.Format(Resources.CommandLineUsage, Path.GetFileName(path));

                MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LogMaster.Instance.InfoFormat("Application started: '{0}'.", String.Join(" ", args));

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var view = new TaskDialogView();
            var tool = new TrimmingTool(view, args);

            Application.Run(tool);

            stopwatch.Stop();

            LogMaster.Instance.InfoFormat("Application finished: {0}.", stopwatch.Elapsed);
        }

        private static void OnGuiUnhandedException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        private static void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject as Exception);
        }

        private static void HandleUnhandledException(Exception exception)
        {
            LogMaster.Instance.Error("Unexpected error occurred.", exception);
        }
    }
}