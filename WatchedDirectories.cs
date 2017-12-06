using System;
using System.Collections;

namespace SystemsMonitor
{
	/// <summary>
	/// Summary description for WatchedDirectories.
	/// </summary>
	public class WatchedDirectories : IDisposable
	{
		private SortedList m_WatchedDirs = null;
		private bool m_InternalUse = false;
		private string m_ConfigFilename = "";

		public WatchedDirectories()
		{
			InitProps("");
		}

		public WatchedDirectories(string ConfigFilename)
		{
			InitProps(ConfigFilename);
		}

		private void InitProps(string ConfigFilename)
		{
			m_InternalUse = true;
			
			WatchSettings settings = null;
			m_ConfigFilename = ConfigFilename;
			m_WatchedDirs = new SortedList();
			
			if ( m_ConfigFilename.Length == 0 )
			{
				settings = new WatchSettings("WatchedDirectories");
			}
			else
			{
				settings = new WatchSettings(m_ConfigFilename, "WatchedDirectories");
			}

			foreach ( string key in settings.Keys )
			{
				this.Add(key, settings[key]);
			}

			m_InternalUse = false;
		}

		public void Dispose()
		{
			m_InternalUse = true;
			
			foreach ( string dirName in m_WatchedDirs.Keys )
			{
				this.Remove(dirName);
			}
			m_WatchedDirs.Clear();

			m_InternalUse = false;

			return;
		}

		public void Add(string DisplayName, string DirectoryPath)
		{
			DirectoryWatcher dir = new DirectoryWatcher(DisplayName, DirectoryPath);
			m_WatchedDirs.Add(DisplayName, dir);

			if ( !m_InternalUse )
			{
				WatchSettings settings = null;

				if ( m_ConfigFilename.Length == 0 )
				{
					settings = new WatchSettings("WatchedDirectories");
					settings.Add(DisplayName, DirectoryPath);
				}
				else
				{
					settings = new WatchSettings(m_ConfigFilename, "WatchedDirectories");
					settings.Add(DisplayName, DirectoryPath);
				}
			}

			return;
		}

		public void Remove(string DisplayName)
		{
			DirectoryWatcher dir = (DirectoryWatcher)m_WatchedDirs[DisplayName];
			dir.Dispose();

			if ( !m_InternalUse )
			{
				WatchSettings settings = null;
				m_WatchedDirs.Remove(DisplayName);

				if ( m_ConfigFilename.Length == 0 )
				{
					settings = new WatchSettings("WatchedDirectories");
					settings.Remove(DisplayName);
				}
				else
				{
					settings = new WatchSettings(m_ConfigFilename, "WatchedDirectories");
					settings.Remove(DisplayName);
				}
			}

			return;
		}

		public int Count
		{
			get 
			{
				return m_WatchedDirs.Count;
			}
		}

		public DirectoryWatcher this[int Index]
		{
			get
			{
				return (DirectoryWatcher)m_WatchedDirs.GetByIndex(Index);
			}
		}

		public ICollection DisplayNames
		{
			get
			{
				return m_WatchedDirs.Keys;
			}
		}

		public DirectoryWatcher this[string DisplayName]
		{
			get
			{
				return (DirectoryWatcher)m_WatchedDirs[DisplayName];
			}
		}
	}
}
