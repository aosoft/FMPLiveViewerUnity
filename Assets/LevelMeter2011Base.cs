using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;
using UniRx;

public class LevelMeter2011Base : MonoBehaviour
{
	public GameObject _levelMeterPrefab;

	private GameObject[] _levelMeters = new GameObject[FMPPartWork.MaxChannelCount];

	void Awake()
	{
		var work = FMPManager.FMPWork;
		for (int i = 0; i < _levelMeters.Length; i++)
		{
			var o = Instantiate(_levelMeterPrefab);
			o.transform.parent = this.transform;
			var l = o.GetComponent<LevelMeter>();
			l.PartWork.Value = work.Parts[i];
			_levelMeters[i] = o;
		}

		work.ActiveChannelCount.Subscribe(count =>
		{
			float width = 1.0f;
			if (count > 12)
			{
				width = Mathf.Sin(2.0f * Mathf.PI / count) / Mathf.Sin(Mathf.PI / 6);
				width = Mathf.Max(width, 0.1f);
			}

			for (int i = 0; i < _levelMeters.Length; i++)
			{
				_levelMeters[i].SetActive(i < count);
			}
		}).AddTo(this);
	}
}
