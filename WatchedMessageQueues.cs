using System;
using System.Collections;

namespace SystemsMonitor
{
	/// <summary>
	/// Summary description for WatchedMessageQueues.
	/// </summary>
	public class WatchedMessageQueues : IDisposable
	{
		private SortedList m_WatchedQueues = null;
		private bool m_InternalUse = false;
		private string m_ConfigFilename = "";

		public WatchedMessageQueues()
		{
			InitProps("");
		}

		public WatchedMessageQueues(string ConfigFilename)
		{
			InitProps(ConfigFilename);
		}

		private void InitProps(string ConfigFilename)
		{
			m_InternalUse = true;

			WatchSettings settings = null;
			m_ConfigFilename = ConfigFilename;
			m_WatchedQueues = new SortedList();
			
			if ( m_ConfigFilename.Length == 0 )
			{
				settings = new WatchSettings("WatchedMessageQueues");
			}
			else
			{
				settings = new WatchSettings(m_ConfigFilename, "WatchedMessageQueues");
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
			
			foreach ( string queueName in m_WatchedQueues.Keys )
			{
				this.Remove(queueName);
			}
			m_WatchedQueues.Clear();

			m_InternalUse = false;

			return;
		}

		public void Add(string DisplayName, string MessageQueuePath)
		{
			MessageQueueWatcher queue = new MessageQueueWatcher(DisplayName, MessageQueuePath);
			m_WatchedQueues.Add(DisplayName, queue);

			if ( !m_InternalUse )
			{
				WatchSettings settings = null;

				if ( m_ConfigFilename.Length == 0 )
				{
					settings = new WatchSettings("WatchedMessageQueues");
					settings.Add(DisplayName, MessageQueuePath);
				}
				else
				{
					settings = new WatchSettings(m_ConfigFilename, "WatchedMessageQueues");
					settings.Add(DisplayName, MessageQueuePath);
				}
			}

			return;
		}

		public void Remove(string DisplayName)
		{
			if ( !m_InternalUse )
			{
				WatchSettings settings = null;
				m_WatchedQueues.Remove(DisplayName);

				if ( m_ConfigFilename.Length == 0 )
				{
					settings = new WatchSettings("WatchedMessageQueues");
					settings.Remove(DisplayName);
				}
				else
				{
					settings = new WatchSettings(m_ConfigFilename, "WatchedMessageQueues");
					settings.Remove(DisplayName);
				}
			}

			return;
		}

		public int Count
		{
			get 
			{
				return m_WatchedQueues.Count;
			}
		}

		public MessageQueueWatcher this[int Index]
		{
			get
			{
				return (MessageQueueWatcher)m_WatchedQueues.GetByIndex(Index);
			}
		}

		public ICollection DisplayNames
		{
			get
			{
				return m_WatchedQueues.Keys;
			}
		}

		public MessageQueueWatcher this[string DisplayName]
		{
			get
			{
				return (MessageQueueWatcher)m_WatchedQueues[DisplayName];
			}
		}
	}
}
