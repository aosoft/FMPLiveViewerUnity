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
		private FMPPartWork[] _tmpPartworks = new FMPPartWork[FMPPartWork.MaxChannelCount];
		private uint? _musicStartCounter = new uint?();

		private ReactiveProperty<TimeSpan> _propPlayTime = new ReactiveProperty<TimeSpan>();
		private FloatReactiveProperty _propProgress = new FloatReactiveProperty();
		private StringReactiveProperty _propMusicTitle = new StringReactiveProperty();
		private StringReactiveProperty _propMusicCreator = new StringReactiveProperty();
		private IntReactiveProperty _propActiveChannelCount = new IntReactiveProperty();
		private IntReactiveProperty _propFMChannelCount = new IntReactiveProperty();
		private IntReactiveProperty _propSSGChannelCount = new IntReactiveProperty();
		private IntReactiveProperty _propPCMChannelCount = new IntReactiveProperty();
		private ReactiveCollection<RxFMPPartWork> _propParts = new ReactiveCollection<RxFMPPartWork>();

		private Subject<Unit> _observeMusicStart = new Subject<Unit>();

		public RxFMPWork()
		{
			for (int i = 0; i < FMPPartWork.MaxChannelCount; i++)
			{
				_propParts.Add(new RxFMPPartWork());
			}
		}

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
			_work.CopyPartWork(0, _tmpPartworks, 0, _tmpPartworks.Length);

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

			for (int i = 0; i < _tmpPartworks.Length; i++)
			{
				_propParts[i].Update(_tmpPartworks[i]);
			}

			if (_musicStartCounter.HasValue == false ||
				_musicStartCounter.Value != gwork.StartCounter)
			{
				//	再生開始時に行うプロパティ更新

				int fm = 0;
				int ssg = 0;
				int pcm = 0;
				int pc = 0;
				for (int i = 0; i < FMPPartWork.MaxChannelCount; i++)
				{
					var unit = gwork.Mode[i];
					switch (unit)
					{
						case FMPSoundUnit.FM:
							{
								fm++;
								pc++;
							}
							break;

						case FMPSoundUnit.SSG:
							{
								ssg++;
								pc++;
							}
							break;

						case FMPSoundUnit.PCM:
							{
								pcm++;
								pc++;
							}
							break;
					}

					_propParts[i].SetSoundUnit(unit);
				}

				_propActiveChannelCount.Value = pc;
				_propFMChannelCount.Value = fm;
				_propSSGChannelCount.Value = ssg;
				_propPCMChannelCount.Value = pcm;


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

		public IReadOnlyReactiveProperty<int> ActiveChannelCount
		{
			get
			{
				return _propActiveChannelCount;
			}
		}

		public IReadOnlyReactiveProperty<int> FMChannelCount
		{
			get
			{
				return _propFMChannelCount;
			}
		}

		public IReadOnlyReactiveProperty<int> SSGChannelCount
		{
			get
			{
				return _propSSGChannelCount;
			}
		}

		public IReadOnlyReactiveProperty<int> PCMChannelCount
		{
			get
			{
				return _propPCMChannelCount;
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
