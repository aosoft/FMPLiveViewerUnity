using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityFMP;

public class LevelMeter : MonoBehaviour
{
	public GameObject _body;
	public GameObject _peak;

	public LevelMeter()
	{
		this.PartWork = new ReactiveProperty<RxFMPPartWork>();
	}

	private void Awake()
	{
		var bodyAnimator = _body.GetComponent<Animator>();
		var peakAnimator = _peak.GetComponent<Animator>();

		PartWork.Subscribe(part =>
		{
			part.KeyOn.Subscribe(value =>
			{
				var tmp = transform.localScale;
				tmp.y = 5.0f * part.VolumeFloat.Value;
				transform.localScale = tmp;
				bodyAnimator.SetTrigger("Start");
				peakAnimator.SetTrigger("Start");
			}).AddTo(this);
		}).AddTo(this);
	}


	public IReactiveProperty<RxFMPPartWork> PartWork
	{
		get;
		private set;
	}
}
