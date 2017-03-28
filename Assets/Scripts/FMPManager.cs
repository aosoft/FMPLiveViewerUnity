using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMP.FMP7;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public enum SceneType : int
{
	Unspecified = 0,
	LevelMeter2011,
}

public class FMPManager : MonoBehaviour
{
	private static RxFMPWork _work = null;

	private static string _timeFormat =
		string.Format("HH:mm:ss{0}ff", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

	private SceneType _currentSceneType = SceneType.Unspecified;

	public UnityEngine.UI.Text _musicTitle;
	public UnityEngine.UI.Text _musicCreator;
	public UnityEngine.UI.Button _nextMusicButton;
	public UnityEngine.UI.Button _playOrPauseButton;
	public UnityEngine.UI.Slider _playProgress;
	public UnityEngine.UI.Text _playTime;
	public UnityEngine.UI.Text _channelCountInfo;
	public UnityEngine.UI.Text _calorieInfo;

	public FMPManager()
	{
	}

	public void Awake()
	{
		_currentSceneType = SceneType.Unspecified;
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

		FMPWork.MusicStartEvent.Subscribe(_ =>
		{
			var sb = new System.Text.StringBuilder();
			if (FMPWork.FMChannelCount.Value > 0)
			{
				sb.AppendFormat("FM: {0}ch\n", FMPWork.FMChannelCount.Value);
			}
			if (FMPWork.SSGChannelCount.Value > 0)
			{
				sb.AppendFormat("SSG: {0}ch\n", FMPWork.SSGChannelCount.Value);
			}
			if (FMPWork.PCMChannelCount.Value > 0)
			{
				sb.AppendFormat("PCM: {0}ch\n", FMPWork.PCMChannelCount.Value);
			}
			sb.AppendFormat("Total: {0}ch", FMPWork.ActiveChannelCount);
			_channelCountInfo.text = sb.ToString();
		}).AddTo(this);

		Observable.Create<Tuple<int, int>>(observer =>
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			return FMPWork.AverageCalorie.Zip(
				FMPWork.InstantCalorie,
				(value1, value2) =>
				{
					return new Tuple<int, int>(value1, value2);
				}).Subscribe(value =>
				{
					if (sw.ElapsedMilliseconds > 1000)
					{
						observer.OnNext(value);
						sw.Reset();
						sw.Start();
					}
				});
		})
		.Select(value =>
		{
			return string.Format("Calorie\nAvg {0}\nInstant {1}", value.Item1, value.Item2);
		})
		.SubscribeToText(_calorieInfo)
		.AddTo(this);

		ChangeSubScene(SceneType.LevelMeter2011);
	}

	void OnDestroy()
	{
		if (_work != null)
		{
			_work.Dispose();
			_work = null;
		}
	}


	public void ChangeSubScene(SceneType scene)
	{
		if (_currentSceneType == scene)
		{
			return;
		}

		if (_currentSceneType > SceneType.Unspecified)
		{
			SceneManager.UnloadSceneAsync((int)_currentSceneType).AsAsyncOperationObservable()
				.Subscribe(_ =>
				{
					SceneManager.LoadScene((int)scene, LoadSceneMode.Additive);
					_currentSceneType = scene;
				});
		}
		else
		{
			SceneManager.LoadScene((int)scene, LoadSceneMode.Additive);
			_currentSceneType = scene;
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
