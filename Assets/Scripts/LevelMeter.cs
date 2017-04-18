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
		var bodyRenderer = _body.GetComponentInChildren<MeshRenderer>();
		var bodyAnimator = _body.GetComponent<Animator>();
		var bodyColorBlend = _body.GetComponentInChildren<MaterialColorBlend>();
		var peakRenderer = _peak.GetComponentInChildren<MeshRenderer>();
		var peakAnimator = _peak.GetComponent<Animator>();
		var peakColorBlend = _body.GetComponentInChildren<MaterialColorBlend>();

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

					if (mat != null)
					{
						var colorA = mat.color * 0.8f;
						var colorB = mat.color * 1.8f;
						bodyColorBlend._colorA = colorA;
						bodyColorBlend._colorB = colorB;
						peakColorBlend._colorA = colorA;
						peakColorBlend._colorB = colorB;
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
