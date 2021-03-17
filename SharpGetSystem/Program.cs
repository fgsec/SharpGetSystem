using SharpGetSystem.Service;
using SharpGetSystem.Console;
using System;
using System.ServiceProcess;

namespace SharpGetSystem {
	class Program {
		static void Main(string[] args) {
			if (!Environment.UserInteractive) {
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] {
					new Service1()
				};
				ServiceBase.Run(ServicesToRun);
			} else {
				Console.Program.Start();
				System.Console.Write("Hit any key to continue...");
				System.Console.ReadKey();
			}
		}
	}
}
