using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace VarjoCompanion;

public class MainApp
{
    static void Main(string[] args)
    {
        IntPtr session = VarjoEyeTracking.Init();
        
        if (!VarjoEyeTracking.IsGazeAllowed())
        {
            Console.WriteLine("Gaze tracking is not allowed! Please enable it in the Varjo Base!");
            return;
        }

        VarjoEyeTracking.GazeInit();
        VarjoEyeTracking.SyncProperties();

        VarjoEyeTracking.VarjoData varjoData = new VarjoEyeTracking.VarjoData();

        using (var memMapFile = MemoryMappedFile.CreateNew("VarjoEyeTracking", Marshal.SizeOf(varjoData)))
        {
            using (var accessor = memMapFile.CreateViewAccessor())
            {
                Console.WriteLine("Eye tracking session has started!");
                while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter))
                {
                    varjoData = VarjoEyeTracking.GetGazeData();
                    accessor.Write(0, ref varjoData);
                }
            }
        }

        VarjoEyeTracking.varjo_SessionShutDown(session);
    }
}
