using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityFMP;
using FMP.FMP7;

public class LevelMeter : MonoBehaviour
{
	public GameObject _body;
	public GameObject _peak;

	private Vector3 _defaultLocalScale;

	public LevelMeter()
	{
		this.PartWork = new ReactiveProperty<RxFMPPartWork>();
	}

	private void Awake()
	{
		var bodyRenderer = _body.GetComponent<MeshRenderer>();
		var bodyAnimator = _body.GetComponent<Animator>();
		var peakRenderer = _peak.GetComponent<MeshRenderer>();
		var peakAnimator = _peak.GetComponent<Animator>();

		//	Scale の初期値をひかえておく
		//	レベルメーターの KeyOn 時に設定する Scale の最小値はこれを参照する。
		//	(レベルメーターの値を 0 にすると表示がおかしくなるため)
		_defaultLocalScale = transform.localScale;

		PartWork.Subscribe(part =>
		{
			if (part != null)
			{
				part.SoundUnit.Subscribe(value =>
				{
					Material mat = null;
					switch (value)
					{
						case FMPSoundUnit.FM:
							{
								mat = Resources.Load("LevelMeterFM") as Material;
							}
							break;
						case FMPSoundUnit.SSG:
							{
								mat = Resources.Load("LevelMeterSSG") as Material;
							}
							break;
						case FMPSoundUnit.PCM:
							{
								mat = Resources.Load("LevelMeterPCM") as Material;
							}
							break;
						default:
							{
								mat = null;
							}
							break;
					}

					bodyRenderer.material = mat;
					peakRenderer.material = mat;
				}).AddTo(this);

				part.KeyOn.Subscribe(value =>
				{
					var tmp = transform.localScale;
					tmp.y = Mathf.Max(5.0f * part.VolumeFloat.Value, _defaultLocalScale.y);
					transform.localScale = tmp;
					bodyAnimator.SetTrigger("Start");
					peakAnimator.SetTrigger("Start");
				}).AddTo(this);
			}
		}).AddTo(this);
	}


	public IReactiveProperty<RxFMPPartWork> PartWork
	{
		get;
		private set;
	}
}
