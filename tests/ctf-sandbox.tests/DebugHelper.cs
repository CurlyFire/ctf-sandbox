using System.Diagnostics;

namespace ctf_sandbox.tests;

public static class DebugHelper
{
    public static void WaitForDebugger()
    {
        Console.WriteLine($"Attach debugger to PID: {Environment.ProcessId}");
        if (!Debugger.IsAttached)
        {
            Console.WriteLine("Waiting for debugger to attach...");
            while (!Debugger.IsAttached)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Debugger attached.");
        }
    }
}
