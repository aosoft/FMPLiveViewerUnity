using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public class FMPManager : MonoBehaviour
{
	private static RxFMPWork _work = null;

	public UnityEngine.UI.Button _nextMusicButton;
	public UnityEngine.UI.Button _playOrPauseButton;

	public StringReactiveProperty _playTime = new StringReactiveProperty();
	public StringReactiveProperty _musicTitle = new StringReactiveProperty();
	public StringReactiveProperty _musicCreator = new StringReactiveProperty();

	public FMPManager()
	{
	}

	public void Awake()
	{
		this.UpdateAsObservable().Subscribe(_ => FMPWork.Update()).AddTo(this);

		FMPWork.PlayTime.Select(value => value.ToString()).Subscribe(value => _playTime.Value = value).AddTo(this);
		FMPWork.MusicTitle.Subscribe(value => _musicTitle.Value = value).AddTo(this);
		FMPWork.MusicCreator.Subscribe(value => _musicCreator.Value = value).AddTo(this);
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
