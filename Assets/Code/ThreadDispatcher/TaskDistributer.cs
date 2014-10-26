﻿using System.Collections;
using System.Linq;
using System.Threading;
using System;


namespace ThreadDispatcher {
	public class TaskDistributor : DispatcherBase
	{
		private TaskWorker[] workerThreads;
		
		internal WaitHandle NewDataWaitHandle { get { return DataEvent; } }
		
		private static TaskDistributor mainTaskDistributor;
		
		/// <summary>
		/// Returns the first created TaskDistributor instance. When no instance has been created an exception will be thrown.
		/// </summary>
		public static TaskDistributor Main
		{
			get
			{
				if (mainTaskDistributor == null)
					throw new InvalidOperationException("No default TaskDistributor found, please create a new TaskDistributor instance before calling this property.");
				
				return mainTaskDistributor;
			}
		}
		
		/// <summary>
		/// Creates a new instance of the TaskDistributor with ProcessorCount x3 worker threads.
		/// The task distributor will auto start his worker threads.
		/// </summary>
		public TaskDistributor()
			: this(0)
		{
		}
		
		/// <summary>
		/// Creates a new instance of the TaskDistributor.
		/// The task distributor will auto start his worker threads.
		/// </summary>
		/// <param name="workerThreadCount">The number of worker threads, a value below one will create ProcessorCount x3 worker threads.</param>
		public TaskDistributor(int workerThreadCount)
			: this(workerThreadCount, true)
		{
		}
		
		/// <summary>
		/// Creates a new instance of the TaskDistributor.
		/// </summary>
		/// <param name="workerThreadCount">The number of worker threads, a value below one will create ProcessorCount x3 worker threads.</param>
		/// <param name="autoStart">Should the instance auto start the worker threads.</param>
		public TaskDistributor(int workerThreadCount, bool autoStart)
		{
			if (workerThreadCount <= 0)
			{
				#if !NO_UNITY
				workerThreadCount = UnityEngine.SystemInfo.processorCount * 3;
				#else
				workerThreadCount = Environment.ProcessorCount * 3;
				#endif
			}
			
			workerThreads = new TaskWorker[workerThreadCount];
			lock (workerThreads)
			{
				for (var i = 0; i < workerThreadCount; ++i)
					workerThreads[i] = new TaskWorker(this);
			}
			
			if (mainTaskDistributor == null)
				mainTaskDistributor = this;
			
			if (autoStart)
				Start();
		}
		
		/// <summary>
		/// Starts the TaskDistributor if its not currently running.
		/// </summary>
		public void Start()
		{
			lock (workerThreads)
			{
				foreach (TaskWorker t in workerThreads.Where(t => !t.IsAlive))
				{
					t.Dispatcher.AddTasks(this.SplitTasks(workerThreads.Length));
					t.Start();
				}
			}
		}
		
		internal void FillTasks(Dispatcher target)
		{
			target.AddTasks(this.IsolateTasks(1));
		}
		
		protected override void CheckAccessLimitation()
		{
			if (ThreadBase.CurrentThread is TaskWorker &&
			    ((TaskWorker)ThreadBase.CurrentThread).TaskDistributor == this)
			{
				throw new InvalidOperationException("Access to TaskDistributor prohibited when called from inside a TaskDistributor thread. Dont dispatch new Tasks through the same TaskDistributor. If you want to distribute new tasks create a new TaskDistributor and use the new created instance. Remember to dispose the new instance to prevent thread spamming.");
			}
		}
		
		#region IDisposable Members
		
		/// <summary>
		/// Disposes all TaskDistributor, worker threads, resources and remaining tasks.
		/// </summary>
		public override void Dispose()
		{
			while (true)
			{
				TaskBase currentTask;
				lock (TaskList)
				{
					if (TaskList.Count != 0)
					{
						currentTask = TaskList[0];
						TaskList.RemoveAt(0);
					}
					else
						break;
				}
				currentTask.Dispose();
			}
			
			lock (workerThreads)
			{
				foreach (TaskWorker t in workerThreads)
					t.Dispose();
				workerThreads = new TaskWorker[0];
			}
			
			DataEvent.Close();
			DataEvent = null;
			
			if (mainTaskDistributor == this)
				mainTaskDistributor = null;
		}
		
		#endregion
	}
	
	internal class TaskWorker : ThreadBase
	{
		public Dispatcher Dispatcher;
		public TaskDistributor TaskDistributor;
		
		public TaskWorker(TaskDistributor taskDistributor)
			: base(false)
		{
			TaskDistributor = taskDistributor;
			Dispatcher = new Dispatcher(false);
		}
		
		protected override IEnumerator Do()
		{
			while (!ExitEvent.WaitOne(0, false))
			{
				if (Dispatcher.ProcessNextTask()) continue;
				TaskDistributor.FillTasks(Dispatcher);
				if (Dispatcher.TaskCount != 0) continue;
				var result = WaitHandle.WaitAny(new[] { ExitEvent, TaskDistributor.NewDataWaitHandle });
				if (result == 0)
					return null;
				TaskDistributor.FillTasks(Dispatcher);
			}
			return null;
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (Dispatcher != null)
				Dispatcher.Dispose();
			Dispatcher = null;
		}
	}
}
