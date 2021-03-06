﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMP.FMP7;
using UniRx;

namespace UnityFMP
{
	public class RxFMPPartWork
	{
		private FMPPartWork _currentWork = new FMPPartWork();

		private ReactiveProperty<FMPSoundUnit> _propSoundUnit = new ReactiveProperty<FMPSoundUnit>(FMPSoundUnit.None);
		private ByteReactiveProperty _propKeyOn = new ByteReactiveProperty();
		private ReactiveProperty<FMPNote> _propNote = new ReactiveProperty<FMPNote>();
		private IntReactiveProperty _propVolume = new IntReactiveProperty();
		private FloatReactiveProperty _propVolumeFloat = new FloatReactiveProperty();
		private IntReactiveProperty _propPan = new IntReactiveProperty();

		public RxFMPPartWork()
		{
		}

		public void Update(FMPPartWork work)
		{
			_currentWork = work;
			_propNote.Value = work.Note;
			_propVolume.Value = work.Volume;
			_propVolumeFloat.Value = (float)work.Volume / 127.0f;
			_propPan.Value = (int)work.Pan - 128;

			//	KeyOn Counter は一番最後に更新しないと他要素が正しく反映されない可能性あり
			_propKeyOn.Value = work.Keyon;
		}

		public void SetSoundUnit(FMPSoundUnit soundUnit)
		{
			_propSoundUnit.Value = soundUnit;
		}

		public IReadOnlyReactiveProperty<FMPSoundUnit> SoundUnit
		{
			get
			{
				return _propSoundUnit;
			}
		}

		public IReadOnlyReactiveProperty<byte> KeyOn
		{
			get
			{
				return _propKeyOn;
			}
		}

		public IReadOnlyReactiveProperty<FMPNote> Note
		{
			get
			{
				return _propNote;
			}
		}

		public IReadOnlyReactiveProperty<int> Volume
		{
			get
			{
				return _propVolume;
			}
		}

		public IReadOnlyReactiveProperty<float> VolumeFloat
		{
			get
			{
				return _propVolumeFloat;
			}
		}

		public IReadOnlyReactiveProperty<int> Pan
		{
			get
			{
				return _propPan;
			}
		}
	}
}
