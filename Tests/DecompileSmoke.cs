using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Crowbar;

internal static class DecompileSmoke
{
    [STAThread]
    private static int Main(string[] args)
    {
        var app = new App();
        var settings = new AppSettings();
        typeof(App).Assembly.GetType("Crowbar.Main").GetField("TheApp").SetValue(null, app);
        typeof(App).GetField("theSettings", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(app, settings);

        settings.DecompileMdlPathFileName = args[0];
        settings.DecompileMode = Directory.Exists(args[0]) ? AppEnums.InputOptions.Folder : AppEnums.InputOptions.File;
        settings.DecompileOutputFolderOption = AppEnums.DecompileOutputPathOptions.WorkFolder;
        settings.DecompileOutputFullPath = args[1];
        settings.DecompileMeshResourceRoot = args[2];
        settings.DecompileGroupIntoQciFilesIsChecked = args[3] == "group";
        settings.DecompileQcFileIsChecked = true;
        settings.DecompileReferenceMeshSmdFileIsChecked = true;
        settings.DecompileLogFileIsChecked = true;

        using (var lockedFile = args.Length > 4 ? File.Open(args[4], FileMode.Open, FileAccess.Read, FileShare.Read) : null)
        using (var decompiler = new Decompiler())
        using (var completed = new ManualResetEvent(false))
        {
            Exception error = null;
            DecompilerOutputInfo result = null;
            decompiler.RunWorkerCompleted += (sender, eventArgs) =>
            {
                error = eventArgs.Error;
                if (error == null && !eventArgs.Cancelled)
                    result = (DecompilerOutputInfo)eventArgs.Result;
                completed.Set();
            };
            decompiler.Run();
            if (!completed.WaitOne(TimeSpan.FromSeconds(45)))
                throw new TimeoutException("Decompilation did not finish within 45 seconds.");
            if (error != null)
                throw error;
            Console.WriteLine(result.theStatus.ToString());
        }
        return 0;
    }
}

