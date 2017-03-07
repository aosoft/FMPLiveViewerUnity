using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public class FMPManager : MonoBehaviour
{
	private RxFMPWork _fmpWork = null;

	public UnityEngine.UI.Button _playButton;
	public UnityEngine.UI.Button _pauseButton;

	public StringReactiveProperty _playTime = new StringReactiveProperty();
	public StringReactiveProperty _musicTitle = new StringReactiveProperty();
	public StringReactiveProperty _musicCreator = new StringReactiveProperty();

	public FMPManager()
	{
	}

	public void Awake()
	{
		_fmpWork = new RxFMPWork();

		this.UpdateAsObservable().Subscribe(_ => _fmpWork.Update());

		_fmpWork.PlayTime.Select(value => value.ToString()).Subscribe(value => _playTime.Value = value);
		_fmpWork.MusicTitle.Subscribe(value => _musicTitle.Value = value);
		_fmpWork.MusicCreator.Subscribe(value => _musicCreator.Value = value);

		var musics = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.owi");
		int index = 0;
		_playButton.OnClickAsObservable().Select(_ => musics[(index++) % musics.Length])
			.Subscribe(value =>
			{
				FMPControl.MusicLoadAndPlay(value);
			});

		_pauseButton.OnClickAsObservable().Subscribe(_ =>
		{
			FMPControl.MusicPause();
		});
	}

	IEnumerator<string> GetMusics()
	{
		var musics = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.owi");
		while (true)
		{
			foreach (var item in musics)
			{
				yield return item;
			} 
		}
	}

	void OnDestroy()
	{
		if (_fmpWork != null)
		{
			_fmpWork.Dispose();
			_fmpWork = null;
		}
	}

	public RxFMPWork FMPWork
	{
		get
		{
			return _fmpWork;
		}
	}
}
