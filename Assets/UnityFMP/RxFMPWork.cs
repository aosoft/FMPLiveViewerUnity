using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMP.FMP7;
using UniRx;

namespace UnityFMP
{
	public class RxFMPWork : IDisposable
	{
		private FMPWork _work = new FMPWork();
		private FMPGlobalWork _gwork = new FMPGlobalWork();
		private FMPPartWork[] _partworks = new FMPPartWork[FMPPartWork.MaxChannelCount];
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
			for (int i = 0; i < _partworks.Length; i++)
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
					_gwork = _work.GetGlobalWork();
					_work.CopyPartWork(0, _partworks, 0, _partworks.Length);
				}
				finally
				{
					_work.Close();
				}

				UpdateEvery();

				if (_musicStartCounter.HasValue == false ||
					_musicStartCounter.Value != _gwork.StartCounter)
				{
					UpdateOnMusicStart();

					_musicStartCounter = _gwork.StartCounter;
					_observeMusicStart.OnNext(Unit.Default);
				}
			}
			catch
			{
			}
		}

		private void UpdateEvery()
		{
			_propPlayTime.Value = TimeSpan.FromMilliseconds(_gwork.PlayTime * 10);
			if (_gwork.Count < 1)
			{
				_propProgress.Value = 0.0f;
			}
			else if (_gwork.CountNow > _gwork.Count)
			{
				_propProgress.Value = 1.0f;
			}
			else
			{
				_propProgress.Value = (float)_gwork.CountNow / (float)_gwork.Count;
			}

			for (int i = 0; i < _partworks.Length; i++)
			{
				_propParts[i].Update(_partworks[i]);
			}

		}

		private void UpdateOnMusicStart()
		{
			int fm = 0;
			int ssg = 0;
			int pcm = 0;
			int pc = 0;
			for (int i = 0; i < FMPPartWork.MaxChannelCount; i++)
			{
				var unit = _gwork.Mode[i];
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
