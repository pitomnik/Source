using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace SurfedAndFound.Shared.Tools
{
	#region WorkItem Class

	public sealed class WorkItem
	{
		#region Status Enum

		public enum Status
		{
			Queued,
			Executing,
			Completed,
			Aborted,
		}

		#endregion

		#region Private Members

		private readonly ExecutionContext context;
		private readonly WaitCallback callback;
		private readonly object state;

		#endregion

		#region Constructors

		public WorkItem(ExecutionContext context, WaitCallback callback, object state)
        {
			this.context = context;
			this.callback = callback;
			this.state = state;
		}

		#endregion

		#region Public Members

		public ExecutionContext Context
		{
			[DebuggerNonUserCode]
			get { return context; }
		}
		
		public WaitCallback Callback
		{
			[DebuggerNonUserCode]
			get { return callback; }
		}

		public object State
		{
			[DebuggerNonUserCode]
			get { return state; }
		}

		#endregion
	}

	#endregion

	#region ThreadPoolEx Class

	public static class ThreadPoolEx
	{
		#region Private Members

		private static LinkedList<WorkItem> items = new LinkedList<WorkItem>();
        private static Dictionary<WorkItem, Thread> threads = new Dictionary<WorkItem, Thread>();

		#endregion

		#region Public Methods

		public static WorkItem QueueUserWorkItem(WaitCallback callback)
        {
            return QueueUserWorkItem(callback, null);
        }

        public static WorkItem QueueUserWorkItem(WaitCallback callback, object state)
        {
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

            WorkItem item = new WorkItem(ExecutionContext.Capture(), callback, state);

			lock (items)
			{
				items.AddLast(item);
			}
            
			ThreadPool.QueueUserWorkItem(new WaitCallback(HandleItem));
            
			return item;
        }

        public static WorkItem.Status Cancel(WorkItem item, bool abort)
        {
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			WorkItem.Status status;

            lock (items)
            {
                LinkedListNode<WorkItem> node = items.Find(item);
                
				if (node != null)
                {
					status = WorkItem.Status.Queued;
					
					items.Remove(node);
                }
				else if (threads.ContainsKey(item))
				{
					if (abort)
					{
						status = WorkItem.Status.Aborted;

						threads[item].Abort();
						threads.Remove(item);
					}
					else
					{
						status = WorkItem.Status.Executing;
					}
				}
				else
				{
					status = WorkItem.Status.Completed;
				}
            }

			return status;
        }

		public static void CancelAll(bool abort)
		{
			lock (items)
			{
				WorkItem[] itemsArray = new WorkItem[items.Count];

				items.CopyTo(itemsArray, 0);

				foreach (WorkItem item in itemsArray)
				{
					Cancel(item, abort);
				}
			}
		}

		#endregion

		#region Private Methods

		private static void HandleItem(object reserved)
        {
            WorkItem item = null;

			try
            {
                lock (items)
                {
                    if (items.Count > 0)
                    {
                        item = items.First.Value;
                        items.RemoveFirst();
                    }

					if (item == null)
					{
						return;
					}

                    threads.Add(item, Thread.CurrentThread);
                }
				
				ExecutionContext.Run(item.Context, delegate { item.Callback(item.State); }, null);
            }
            finally
            {
                lock (items)
                {
					if (item != null)
					{
						threads.Remove(item);
					}
                }
            }
		}

		#endregion
	}

	#endregion
}
