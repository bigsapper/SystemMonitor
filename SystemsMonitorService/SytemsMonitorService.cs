using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using SystemsMonitor;
using System.Reflection;

namespace SystemsMonitor
{
	public class SystemsMonitorService : System.ServiceProcess.ServiceBase
	{
		private WatchedDirectories m_WatchedDirs = null;
		private WatchedMessageQueues m_WatchedQueues = null;
		private PerformanceCounter[] m_DirectoryCounters;
		private PerformanceCounter[] m_MessageQueueCounters;
		private Timer m_Timer = null;
		private DateTime m_Midnight;
		private const string PERF_MON_CAT_DIR_WATCHER = "My Directory Watcher";
		private const string PERF_MON_CAT_MSQ_WATCHER = "My Message Queue Watcher";
		private const string DIR_ADD_FILES = " - Files Added";
		private const string DIR_REM_FILES = " - Files Removed";
		private const string DIR_CUR_FILES = " - Current Files";


		public SystemsMonitorService()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new SystemsMonitorService() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SystemsMonitorService
			// 
			this.CanPauseAndContinue = true;
			this.CanShutdown = true;
			this.ServiceName = "BIS Systems Monitor Service";

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				// m_Timer
				m_Timer.Close();
				m_Timer.Dispose();
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			StartService();
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			StopService();
		}

		/// <summary>
		/// Continue this service.
		/// </summary>
		protected override void OnContinue()
		{
			m_Timer.Start();
		}  
		
		/// <summary>
		/// Pause this service.
		/// </summary>
		protected override void OnPause()
		{
			m_Timer.Stop();
		}  

		/// <summary>
		/// Shutdown this service.
		/// </summary>
		protected override void OnShutdown()
		{
			this.OnStop();
		}  

		#region Private Methods
		private void ConfigureTimer()
		{
			// 1000 ticks is approximately 1 second
			WatchSettings settings = new WatchSettings(GetConfigFilename(), "appSettings");
			int refreshInterval = Convert.ToInt32(settings["RefreshInterval"]);
			// ensure we have no less than a 5 second interval
			refreshInterval = refreshInterval >= 5 ? refreshInterval : 5;

			// 
			// m_Timer
			// 
			m_Timer = new Timer(refreshInterval * 1000);
			m_Timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnElapsed);
			m_Timer.AutoReset = true;
			m_Timer.Enabled = false;

			m_Midnight = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
		}

		private PerformanceCounter[] CreateMessageQueueCounter(WatchedMessageQueues myQueues)
		{
			CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
			CounterCreationData ccd;

			for ( int i = 0; i < myQueues.Count; i++ )
			{
				ccd = new CounterCreationData();
				ccd.CounterName = myQueues[i].DisplayName;
				ccd.CounterHelp = "Shows the number of messages in the queue since " + myQueues[i].StartTime;
				ccd.CounterType = PerformanceCounterType.NumberOfItems32;
				ccdc.Add(ccd);
			}
			if ( PerformanceCounterCategory.Exists(PERF_MON_CAT_MSQ_WATCHER) )
			{
				PerformanceCounterCategory.Delete(PERF_MON_CAT_MSQ_WATCHER);
			}

            PerformanceCounterCategory.Create(PERF_MON_CAT_MSQ_WATCHER, "Shows the number of messages in the message queue", PerformanceCounterCategoryType.Unknown, ccdc);

			PerformanceCounter[] myCounters = new PerformanceCounter[myQueues.Count];
			for ( int i = 0; i < myQueues.Count; i++ )
			{
				myCounters[i] = new PerformanceCounter(PERF_MON_CAT_MSQ_WATCHER,
					myQueues[i].DisplayName,
					false);
			}

			return myCounters;
		}

		private PerformanceCounter[] CreateDirectoryCounter(WatchedDirectories myDirs)
		{
			CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
			CounterCreationData ccd;
			
			for ( int i = 0; i < myDirs.Count; i++ )
			{
				ccd = new CounterCreationData();
				ccd.CounterName = myDirs[i].DisplayName + DIR_ADD_FILES;
				ccd.CounterHelp = "Shows the number of files added to the directory since " + myDirs[i].StartTime;
				ccd.CounterType = PerformanceCounterType.NumberOfItems32;
				ccdc.Add(ccd);

				ccd = new CounterCreationData();
				ccd.CounterName = myDirs[i].DisplayName + DIR_REM_FILES;
				ccd.CounterHelp = "Shows the number of files removed from the directory since " + myDirs[i].StartTime;
				ccd.CounterType = PerformanceCounterType.NumberOfItems32;
				ccdc.Add(ccd);

				ccd = new CounterCreationData();
				ccd.CounterName = myDirs[i].DisplayName + DIR_CUR_FILES;
				ccd.CounterHelp = "Shows the number of files currently in the directory";
				ccd.CounterType = PerformanceCounterType.NumberOfItems32;
				ccdc.Add(ccd);
			}

			if ( PerformanceCounterCategory.Exists(PERF_MON_CAT_DIR_WATCHER) )
			{
				PerformanceCounterCategory.Delete(PERF_MON_CAT_DIR_WATCHER);
			}

            PerformanceCounterCategory.Create(PERF_MON_CAT_DIR_WATCHER, "Shows the files moving through the indicated directory",
                PerformanceCounterCategoryType.Unknown, ccdc);

			PerformanceCounter[] myCounters = new PerformanceCounter[myDirs.Count * 3];
			int j = 0;
			for ( int i = 0; i < myDirs.Count; i++ )
			{
				myCounters[j++] = new PerformanceCounter(PERF_MON_CAT_DIR_WATCHER, 
					myDirs[i].DisplayName + DIR_ADD_FILES, 
					false);
				myCounters[j++] = new PerformanceCounter(PERF_MON_CAT_DIR_WATCHER, 
					myDirs[i].DisplayName + DIR_REM_FILES, 
					false);
				myCounters[j++] = new PerformanceCounter(PERF_MON_CAT_DIR_WATCHER, 
					myDirs[i].DisplayName + DIR_CUR_FILES, 
					false);
			}

			return myCounters;
		}

		private void RefreshDirectoryCounters()
		{
			int j = 0;

			for ( int i = 0; i < m_WatchedDirs.Count; i++ )
			{
				if ( m_WatchedDirs[i].CanConnect() )
				{
					int[] FilesCount = m_WatchedDirs[i].FilesCount;
					m_DirectoryCounters[j++].RawValue = FilesCount[(int)FilesCountIndex.FilesAdded];
					m_DirectoryCounters[j++].RawValue = FilesCount[(int)FilesCountIndex.FilesRemoved];
					m_DirectoryCounters[j++].RawValue = FilesCount[(int)FilesCountIndex.FilesCurrent];
				}
				else
				{
					m_DirectoryCounters[j++].RawValue = -1;
					m_DirectoryCounters[j++].RawValue = -1;
					m_DirectoryCounters[j++].RawValue = -1;
				}
			}
		}

		private void RefreshMessageQueueCounters()
		{
			for ( int i = 0; i < m_WatchedQueues.Count; i++ )
			{
				if ( m_WatchedQueues[i].CanConnect() )
				{
					m_MessageQueueCounters[i].RawValue = m_WatchedQueues[i].GetMessageCount();
				}
				else
				{
					m_MessageQueueCounters[i].RawValue = -1;
				}
			}
		}

		private static string GetConfigFilename()
		{
			// Use reflection to find the location of the config file. 
			Assembly asm = Assembly.GetExecutingAssembly();
			string configLoc = asm.Location;

			return configLoc + ".config";
		}

		/// <summary>
		/// Refresh the counter data.
		/// </summary>
		private void OnElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			m_Timer.Stop();

			// if it's after midnight, reset the counter values
			if ( DateTime.Now.CompareTo(m_Midnight) <= 0 )
			{
				RefreshDirectoryCounters();
				RefreshMessageQueueCounters();

			}
			else // reset the counters
			{
				StopService();
				StartService();
			}

			m_Timer.Start();
		}

		private void StartService()
		{
			ConfigureTimer();

			string configFile = GetConfigFilename();
			m_WatchedDirs = new WatchedDirectories(configFile);
			m_WatchedQueues = new WatchedMessageQueues(configFile);
			
			m_DirectoryCounters = CreateDirectoryCounter(m_WatchedDirs);
			m_MessageQueueCounters = CreateMessageQueueCounter(m_WatchedQueues);

			RefreshDirectoryCounters();
			RefreshMessageQueueCounters();

			m_Timer.Start();
		}

		private void StopService()
		{
			m_Timer.Stop();

			for (int i = 0; i < m_MessageQueueCounters.Length; i++ )
			{
				m_MessageQueueCounters[i].Close();
				m_MessageQueueCounters[i].Dispose();
				m_MessageQueueCounters[i] = null;
			}
			PerformanceCounterCategory.Delete(PERF_MON_CAT_MSQ_WATCHER);
			m_WatchedQueues.Dispose();
			m_WatchedQueues = null;

			for ( int i = 0; i < m_DirectoryCounters.Length; i++ )
			{
				m_DirectoryCounters[i].Close();
				m_DirectoryCounters[i].Dispose();
				m_DirectoryCounters[i] = null;
			}
			PerformanceCounterCategory.Delete(PERF_MON_CAT_DIR_WATCHER);
			m_WatchedDirs.Dispose();
			m_WatchedDirs = null;
		}

		#endregion Private Methods
	}
}
