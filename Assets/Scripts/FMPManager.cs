using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public class FMPManager : MonoBehaviour
{
	private static RxFMPWork _work = null;

	private static string _timeFormat =
		string.Format("HH:mm:ss{0}ff", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

	public UnityEngine.UI.Text _musicTitle;
	public UnityEngine.UI.Text _musicCreator;
	public UnityEngine.UI.Button _nextMusicButton;
	public UnityEngine.UI.Button _playOrPauseButton;
	public UnityEngine.UI.Slider _playProgress;
	public UnityEngine.UI.Text _playTime;

	public FMPManager()
	{
	}

	public void Awake()
	{
		this.UpdateAsObservable().Subscribe(_ => FMPWork.Update()).AddTo(this);

		FMPWork.MusicTitle.SubscribeToText(_musicTitle).AddTo(this);
		FMPWork.MusicCreator.SubscribeToText(_musicCreator).AddTo(this);
		FMPWork.Status
			.Select(value => (value != FMPStat.None && (value & FMPStat.Play) != 0) ? "Pause" : "Play")
			.SubscribeToText(_playOrPauseButton.GetComponentInChildren<UnityEngine.UI.Text>())
			.AddTo(this);

		var musics = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.owi");
		int index = 0;
		_nextMusicButton.OnClickAsObservable().Select(_ => musics[(index++) % musics.Length])
			.Subscribe(value =>
			{
				FMPControl.MusicLoadAndPlay(value);
			}).AddTo(this);

		_playOrPauseButton.OnClickAsObservable().Subscribe(_ =>
		{
			if ((FMPWork.Status.Value & (FMPStat.Pause | FMPStat.Play)) != 0)
			{
				FMPControl.MusicPause();
			}
			else
			{
				FMPControl.MusicPlay();
			}
		}).AddTo(this);

		FMPWork.Progress.Subscribe(value => _playProgress.value = value).AddTo(this);
		FMPWork.PlayTime
			.Select(value => (new System.DateTime(value.Ticks)).ToString(_timeFormat))
			.SubscribeToText(_playTime).AddTo(this);
	}

	void OnDestroy()
	{
		if (_work != null)
		{
			_work.Dispose();
			_work = null;
		}
	}

	static public RxFMPWork FMPWork
	{
		get
		{
			if (_work == null)
			{
				_work = new RxFMPWork();
			}
			return _work;
		}
	}
}
