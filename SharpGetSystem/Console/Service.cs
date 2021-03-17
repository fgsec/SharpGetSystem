using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGetSystem.Console {
	public class Service {

		public static bool installService(string name, string path) {

			IntPtr scManager = Pinvoke.OpenSCManager(null, null, 0xF003F);
			if (scManager != IntPtr.Zero) {
				// SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS, SERVICE_DEMAND_START, SERVICE_ERROR_NORMAL
				IntPtr scService = Pinvoke.CreateService(scManager, name, name, 0xF01FF, 0x00000010, 0x00000003, 0x00000001, path, null, null, null, null, null);
				if (scService != IntPtr.Zero) {
					Pinvoke.CloseServiceHandle(scService);
					Pinvoke.CloseServiceHandle(scManager);
					System.Console.WriteLine("[+] Success creating service!");
					return true;
				}
			}
			System.Console.WriteLine("[-] Error creating service (win32-{0})", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
			return false;
		}

		public static bool deleteService(string name) {

			IntPtr scManager = Pinvoke.OpenSCManager(null, null, 0xF003F);
			if (scManager != IntPtr.Zero) {
				IntPtr scService = Pinvoke.OpenService(scManager, name, Pinvoke.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
				if (scService != IntPtr.Zero) {
					if (Pinvoke.DeleteService(scService)) {
						Pinvoke.CloseServiceHandle(scService);
						Pinvoke.CloseServiceHandle(scManager);
						System.Console.WriteLine("[+] Success deleting service!");
						return true;
					}

				}
			}
			System.Console.WriteLine("[-] Error deleting service (win32-{0})", System.Runtime.InteropServices.Marshal.GetLastWin32Error());
			return false;
		}

		public static bool manageService(string name, string action) {

			IntPtr scManager = Pinvoke.OpenSCManager(null, null, 0xF003F);
			if (scManager != IntPtr.Zero) {
				IntPtr scService = Pinvoke.OpenService(scManager, name, Pinvoke.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
				if (scService != IntPtr.Zero) {

					switch (action) {
						case "stop":
							Pinvoke.SERVICE_STATUS stsService = new Pinvoke.SERVICE_STATUS() { };
							if (Pinvoke.ControlService(scService, Pinvoke.SERVICE_CONTROL.STOP, ref stsService)) {
								Pinvoke.CloseServiceHandle(scService);
								Pinvoke.CloseServiceHandle(scManager);
								System.Console.WriteLine("[+] Success stopping service!");
								return true;
							}
							break;
						case "start":
							if (Pinvoke.StartService(scService, 0, null)) {
								Pinvoke.CloseServiceHandle(scService);
								Pinvoke.CloseServiceHandle(scManager);
								System.Console.WriteLine("[+] Success starting service!");
								return true;
							}
							break;
					}

				}
			}
			// Ignore 109, since our service ends itself after connecting to pipe
			if (System.Runtime.InteropServices.Marshal.GetLastWin32Error() != 109)
				System.Console.WriteLine(String.Format("[-] Error doing {1} service (win32-{0})", System.Runtime.InteropServices.Marshal.GetLastWin32Error(), action));
			return false;
		}

	}
}
