using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace SystemsMonitorService
{
	/// <summary>
	/// Summary description for ProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
		private System.ServiceProcess.ServiceInstaller SystemsMonitorServiceInstaller;
		private System.ServiceProcess.ServiceProcessInstaller SystemsMonitorServiceProcessInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.Container components = null;

		public ProjectInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SystemsMonitorServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.SystemsMonitorServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// SystemsMonitorServiceProcessInstaller
			// 
			this.SystemsMonitorServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.SystemsMonitorServiceProcessInstaller.Password = null;
			this.SystemsMonitorServiceProcessInstaller.Username = null;
			// 
			// SystemsMonitorServiceInstaller
			// 
			this.SystemsMonitorServiceInstaller.ServiceName = "BHCS Systems Monitor Service";
			this.SystemsMonitorServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.SystemsMonitorServiceProcessInstaller,
																					  this.SystemsMonitorServiceInstaller});

		}
		#endregion
	}
}
