using System;
using System.Data;
using System.Messaging;

namespace SystemsMonitor
{
	/// <summary>
	/// Summary description for MessageTableWatcher.
	/// </summary>
	public class MessageQueueWatcher
	{
		private string m_DisplayName = "";
		private string m_QueuePath = "";
		private DateTime m_StartTime;
		private string m_ErrorMessage = "";
		private string DoesNotExistMessage = "";

		internal MessageQueueWatcher(string DisplayName, string MessageQueuePath)
		{
			m_DisplayName = DisplayName;
			m_QueuePath = MessageQueuePath;
			m_StartTime = DateTime.Now;
			DoesNotExistMessage = @m_QueuePath + " does not exist.";
		}

		public bool CanConnect()
		{
			bool retVal;

			if ( MessageQueue.Exists(@m_QueuePath) )
			{
				retVal = true;
				if ( m_ErrorMessage.Length > 0 ) m_ErrorMessage = "";
			}
			else
			{
				retVal = false;
				if ( m_ErrorMessage != DoesNotExistMessage ) m_ErrorMessage = DoesNotExistMessage;
			}

			return retVal;
		}

		public int GetMessageCount()
		{
			int retVal = -1;

			if ( CanConnect() )
			{
				try 
				{
					MessageQueue myQueue = new MessageQueue();
					myQueue.Path = this.MessageQueuePath;
					Message[] myMessages = myQueue.GetAllMessages();
					retVal = myMessages.Length;
					if ( m_ErrorMessage.Length > 0 ) m_ErrorMessage = "";
				}
				catch ( Exception e )
				{
					retVal = -1;
					if ( m_ErrorMessage != e.Message ) m_ErrorMessage = e.Message;
				}
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

		public string MessageQueuePath
		{
			get
			{
				string retVal = m_QueuePath;

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
	}
}
