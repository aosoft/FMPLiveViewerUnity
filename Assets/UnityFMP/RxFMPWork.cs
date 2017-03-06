using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMP.FMP7;
using UniRx;

namespace UnityFMP
{
	class RxFMPWork : IDisposable
	{
		private FMPWork _work = new FMPWork();
		private uint? _musicStartCounter = new uint?();

		private ReactiveProperty<TimeSpan> _propPlayTime = new ReactiveProperty<TimeSpan>();
		private FloatReactiveProperty _propProgress = new FloatReactiveProperty();
		private StringReactiveProperty _propMusicTitle = new StringReactiveProperty();
		private StringReactiveProperty _propMusicCreator = new StringReactiveProperty();
		private ReactiveCollection<RxFMPPartWork> _propParts = new ReactiveCollection<RxFMPPartWork>();

		private Subject<Unit> _observeMusicStart = new Subject<Unit>();

		public void Dispose()
		{
			if (_work != null)
			{
				_work.Dispose();
				_work = null;
			}

			_observeMusicStart.OnCompleted();
		}

		public void Update()
		{
			try
			{
				_work.Open();
				try
				{
					InternalUpdate();
				}
				finally
				{
					_work.Close();
				}
			}
			catch
			{
			}
		}

		private void InternalUpdate()
		{
			var gwork = _work.GetGlobalWork();
			_propPlayTime.Value = TimeSpan.FromMilliseconds(gwork.PlayTime * 10);
			if (gwork.Count < 1)
			{
				_propProgress.Value = 0.0f;
			}
			else if (gwork.CountNow > gwork.Count)
			{
				_propProgress.Value = 1.0f;
			}
			else
			{
				_propProgress.Value = (float)gwork.CountNow / (float)gwork.Count;
			}

			if (_musicStartCounter.HasValue == false ||
				_musicStartCounter.Value != gwork.StartCounter)
			{
				//	再生開始時に行うプロパティ更新

				_propMusicTitle.Value = FMPControl.GetTextData(FMPText.Title);
				_propMusicCreator.Value = FMPControl.GetTextData(FMPText.Creator);

				_musicStartCounter = gwork.StartCounter;
				_observeMusicStart.OnNext(Unit.Default);
			}
		}


		public IReadOnlyReactiveProperty<TimeSpan> PlayTime
		{
			get
			{
				return _propPlayTime;
			}
		}


		public IReadOnlyReactiveProperty<float> Progress
		{
			get
			{
				return _propProgress;
			}
		}

		public IReadOnlyReactiveProperty<string> MusicTitle
		{
			get
			{
				return _propMusicTitle;
			}
		}

		public IReadOnlyReactiveProperty<string> MusicCreator
		{
			get
			{
				return _propMusicCreator;
			}
		}

		public IReadOnlyReactiveCollection<RxFMPPartWork> Parts
		{
			get
			{
				return _propParts;
			}
		}

		public IObservable<Unit> MusicStartEvent
		{
			get
			{
				return _observeMusicStart;
			}
		}
	}
}
