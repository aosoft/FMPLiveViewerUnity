using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public class FMPManager : MonoBehaviour
{
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
		FMPWork = new RxFMPWork();

		this.UpdateAsObservable().Subscribe(_ => FMPWork.Update());

		FMPWork.PlayTime.Select(value => value.ToString()).Subscribe(value => _playTime.Value = value);
		FMPWork.MusicTitle.Subscribe(value => _musicTitle.Value = value);
		FMPWork.MusicCreator.Subscribe(value => _musicCreator.Value = value);

		var musics = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.owi");
		int index = 0;
		_playButton.OnClickAsObservable().Select(_ => musics[(index++) % musics.Length])
			.Subscribe(value =>
			{
				FMPControl.MusicLoadAndPlay(value);
			}).AddTo(this);

		_pauseButton.OnClickAsObservable().Subscribe(_ =>
		{
			FMPControl.MusicPause();
		}).AddTo(this);
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
		if (FMPWork != null)
		{
			FMPWork.Dispose();
			FMPWork = null;
		}
	}

	static public RxFMPWork FMPWork
	{
		get;
		private set;
	}
}
