
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace jp.osakana4242.itunes_furikake
{
	public class FormsTimerScheduler : IScheduler, IDisposable
	{
		static FormsTimerScheduler instance;
		
		readonly System.Windows.Forms.Timer timer;
		readonly Subject<DateTimeOffset> subject;

		public static void Init()
		{
			instance = new FormsTimerScheduler();
		}

		public FormsTimerScheduler()
		{
			subject = new Subject<DateTimeOffset>();
			timer = new System.Windows.Forms.Timer();
			timer.Interval = 1;
			timer.Tick += (_a, _b) =>
			{
				subject.OnNext(Now);
			};
			timer.Start();
		}

		public void Dispose()
		{
			subject.Dispose();
			timer.Dispose();
		}

		public static FormsTimerScheduler Instance => instance;

		public DateTimeOffset Now => DateTimeOffset.Now;

		public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
		{
			TimeSpan ts = TimeSpan.FromMilliseconds(0f);
			return Schedule(state, ts, action);
		}

		public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			//var timer = new System.Windows.Forms.Timer();
			//timer.Interval = (int)dueTime.TotalMilliseconds;
			//timer.Tick += (_a, _b) =>
			//{
			//	action(this, state);
			//	timer.Dispose();
			//};
			//timer.Enabled = true;
			//timer.Start();

			//return timer;

			return Schedule(state, Now + dueTime, action);
		}

		public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			return subject.Where(_now => dueTime <= _now).
			Take(1).
			Subscribe(_ =>
			{
				action(this, state);
			});
		}
	}
}
