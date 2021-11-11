using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Vena.Utilities.Imports;

namespace Vena.Utilities;

internal static class Memory
{
    public static async Task<bool> Inject(string dllPath, Process process)
    {
        var injected = false;
        await Task.Run(() =>
        {
            do
            {
                if (process is null)
                    break;

                foreach (ProcessModule pm in process.Modules)
                {
                    if (pm is null || pm.ModuleName is null)
                        continue;

                    if (pm.ModuleName.StartsWith("inject", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }

                if (!process.Responding)
                    break;

                int lenWrite = dllPath.Length + 1;
                UIntPtr allocMem = VirtualAllocEx(process.Handle, (UIntPtr)null, (uint)lenWrite, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

                WriteProcessMemory(process.Handle, allocMem, dllPath, (UIntPtr)lenWrite, out IntPtr bytesout);
                UIntPtr injector = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

                if (injector == UIntPtr.Zero)
                    break;

                IntPtr hThread = CreateRemoteThread(process.Handle, (IntPtr)null, 0, injector, allocMem, 0, out bytesout);
                if (hThread == IntPtr.Zero)
                    break;

                int r = WaitForSingleObject(hThread, 10 * 1000);
                if (r == 0x00000080L || r == 0x00000102L)
                {
                    if (hThread != IntPtr.Zero)
                        CloseHandle(hThread);
                    break;
                }
                VirtualFreeEx(process.Handle, allocMem, (UIntPtr)0, 0x8000);

                if (hThread != IntPtr.Zero)
                    CloseHandle(hThread);

                injected = true;
            } while (false);
        });

        return injected;
    }
}
