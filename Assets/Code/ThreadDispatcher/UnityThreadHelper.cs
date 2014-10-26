using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ThreadDispatcher {
	public class UnityThreadHelper : MonoBehaviour
	{
		private static UnityThreadHelper instance;
		
		public static void EnsureHelper()
		{
			if (null != (object) instance) return;
			instance = FindObjectOfType(typeof(UnityThreadHelper)) as UnityThreadHelper;
			if (null != (object) instance) return;
			var go = new GameObject("[UnityThreadHelper]")
			{
				hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector
			};
			instance = go.AddComponent<UnityThreadHelper>();
			instance.EnsureHelperInstance();
		}
		
		private static UnityThreadHelper Instance
		{
			get
			{
				EnsureHelper();
				return instance;
			}
		}
		
		/// <summary>
		/// Returns the GUI/Main Dispatcher.
		/// </summary>
		public static Dispatcher Dispatcher
		{
			get
			{
				return Instance.CurrentDispatcher;
			}
		}
		
		/// <summary>
		/// Returns the TaskDistributor.
		/// </summary>
		public static TaskDistributor TaskDistributor
		{
			get
			{
				return Instance.CurrentTaskDistributor;
			}
		}
		
		public Dispatcher CurrentDispatcher { get; private set; }
		
		public TaskDistributor CurrentTaskDistributor { get; private set; }
		
		private void EnsureHelperInstance()
		{
			if (CurrentDispatcher == null)
				CurrentDispatcher = new Dispatcher();
			
			if (CurrentTaskDistributor == null)
				CurrentTaskDistributor = new TaskDistributor();
		}
		
		/// <summary>
		/// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(System.Action<ActionThread> action, bool autoStartThread)
		{
			Instance.EnsureHelperInstance();
			
			System.Action<ActionThread> actionWrapper = currentThread =>
			{
				try
				{
					action(currentThread);
				}
				catch (System.Exception ex)
				{
					Debug.LogError(ex);
				}
			};
			var thread = new ActionThread(actionWrapper, autoStartThread);
			Instance.RegisterThread(thread);
			return thread;
		}
		
		/// <summary>
		/// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(System.Action<ActionThread> action)
		{
			return CreateThread(action, true);
		}
		
		/// <summary>
		/// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(System.Action action, bool autoStartThread)
		{
			return CreateThread((thread) => action(), autoStartThread);
		}
		
		/// <summary>
		/// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(System.Action action)
		{
			return CreateThread((thread) => action(), true);
		}
		
		#region Enumeratable
		
		/// <summary>
		/// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The enumeratable action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ThreadBase CreateThread(System.Func<ThreadBase, IEnumerator> action, bool autoStartThread)
		{
			Instance.EnsureHelperInstance();
			
			var thread = new EnumeratableActionThread(action, autoStartThread);
			Instance.RegisterThread(thread);
			return thread;
		}
		
		/// <summary>
		/// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The enumeratable action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ThreadBase CreateThread(System.Func<ThreadBase, IEnumerator> action)
		{
			return CreateThread(action, true);
		}
		
		/// <summary>
		/// Creates new thread which runs the given action. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The enumeratable action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ThreadBase CreateThread(System.Func<IEnumerator> action, bool autoStartThread)
		{
			System.Func<ThreadBase, IEnumerator> wrappedAction = (thread) => { return action(); };
			return CreateThread(wrappedAction, autoStartThread);
		}
		
		/// <summary>
		/// Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so that any exception will be catched and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ThreadBase CreateThread(System.Func<IEnumerator> action)
		{
			System.Func<ThreadBase, IEnumerator> wrappedAction = (thread) => action();
			return CreateThread(wrappedAction, true);
		}
		
		#endregion
		
		readonly List<ThreadBase> registeredThreads = new List<ThreadBase>();
		public void RegisterThread(ThreadBase thread)
		{
			if (registeredThreads.Contains(thread))
			{
				return;
			}
			
			registeredThreads.Add(thread);
		}
		
		void OnDestroy()
		{
			foreach (var thread in registeredThreads)
				thread.Dispose();
			
			if (CurrentDispatcher != null)
				CurrentDispatcher.Dispose();
			CurrentDispatcher = null;
			
			if (CurrentTaskDistributor != null)
				CurrentTaskDistributor.Dispose();
			CurrentTaskDistributor = null;
		}
		
		void Update()
		{
			if (CurrentDispatcher != null)
				CurrentDispatcher.ProcessTasks();
			
			var finishedThreads = registeredThreads.Where(thread => !thread.IsAlive).ToArray();
			foreach (var finishedThread in finishedThreads)
			{
				finishedThread.Dispose();
				registeredThreads.Remove(finishedThread);
			}
		}
	}
}
