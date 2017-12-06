using System;
using System.IO;

namespace SystemsMonitor
{
	public enum FilesCountIndex
	{
		FilesAdded = 0,
		FilesRemoved = 1,
		FilesCurrent = 2
	};

	/// <summary>
	/// Summary description for DirectoryWatcher.
	/// </summary>
	public class DirectoryWatcher : IDisposable
	{
		private int m_FilesAdded, m_FilesRemoved = 0;
		private string m_DisplayName = "";
		private string m_DirectoryPath = "";
		private FileSystemWatcher m_Watcher = null;
		private DateTime m_StartTime;
		private string m_ErrorMessage = "";
		private string DoesNotExistMessage = "";

		internal DirectoryWatcher(string DisplayName, string DirectoryPath)
		{
			m_DisplayName = DisplayName;
			m_DirectoryPath = DirectoryPath;
			DoesNotExistMessage = @m_DirectoryPath + " does not exist.";

			// Create a new FileSystemWatcher and set its properties.
			m_Watcher = new FileSystemWatcher();
			// Watch for changes in LastAccess and LastWrite times.
			m_Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
				| NotifyFilters.FileName | NotifyFilters.DirectoryName;

			// Add event handlers.
			m_Watcher.Created += new FileSystemEventHandler(OnChanged);
			m_Watcher.Deleted += new FileSystemEventHandler(OnChanged);
			
			CanConnect();
		}

		public void Dispose()
		{
			if ( CanConnect() ) 
			{
				m_Watcher.EnableRaisingEvents = false;
			}
		}

		protected void OnChanged(object source, FileSystemEventArgs e)
		{
			// Increment the appropriate counter when a file is created or deleted.
			switch ( e.ChangeType )
			{
				case WatcherChangeTypes.Created:
					m_FilesAdded++;
					break;
				case WatcherChangeTypes.Deleted:
					m_FilesRemoved++;
					break;
			}
		}

		public bool CanConnect()
		{
			bool retVal;

			try 
			{
				if ( Directory.Exists(@m_DirectoryPath) ) 
				{
					// let's make sure we have permission to query the directory
					this.GetFilesCurrent();

					if ( !m_Watcher.EnableRaisingEvents )
					{
						m_Watcher.Path = @m_DirectoryPath;
						m_Watcher.Filter = "*.*";
						m_Watcher.IncludeSubdirectories = false;
						m_Watcher.EnableRaisingEvents = true;
						m_StartTime = DateTime.Now;
					}
					retVal = true;
					if ( m_ErrorMessage.Length > 0 ) m_ErrorMessage = "";
				}
				else
				{
					if ( m_Watcher.EnableRaisingEvents ) 
					{
						m_Watcher.EnableRaisingEvents = false;
					}
					retVal = false;
					if ( m_ErrorMessage != DoesNotExistMessage ) m_ErrorMessage = DoesNotExistMessage;
				}
			}
			catch ( Exception e )
			{
				retVal = false;
				if ( m_ErrorMessage != e.Message ) m_ErrorMessage = e.Message;
			}

			return retVal;
		}

		public string ErrorMessage
		{
			get
			{
				return m_ErrorMessage;
			}
		}

		public int[] FilesCount
		{
			get
			{
				int[] retVal = {-1, -1, -1};

				if ( CanConnect() )
				{
					retVal[(int)FilesCountIndex.FilesAdded] = GetFilesAdded();
					retVal[(int)FilesCountIndex.FilesRemoved] = GetFilesRemoved();
					retVal[(int)FilesCountIndex.FilesCurrent] = GetFilesCurrent();
				}

				return retVal;
			}
		}

		public string DirectoryPath
		{
			get
			{
				string retVal = m_DirectoryPath;

				if ( !CanConnect() ) retVal += ": Cannot connect to path or path not found.";

				return retVal;
			}
		}

		public string DisplayName
		{
			get
			{
				return m_DisplayName;
			}
		}

		public string StartTime
		{
			get
			{
				return m_StartTime.ToLongDateString() + " " + m_StartTime.ToLongTimeString();
			}
		}

		#region Private Methods
		private int GetFilesAdded()
		{
			return m_FilesAdded;
		}

		private int GetFilesRemoved()
		{
			return m_FilesRemoved;
		}

		private int GetFilesCurrent()
		{
			// get a reference to each file in that directory
			string[] files = Directory.GetFiles(@m_DirectoryPath, "*.*");

			return files.Length;
		}
		#endregion Private Methods
	}
}
