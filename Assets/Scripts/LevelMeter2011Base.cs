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
			//
			//	有効なチャンネル数が変動したらレベルメーターの表示本数、レイアウトを調整する。
			//
			float width = 1.0f;
			if (count > 12)
			{
				width = Mathf.Sin(2.0f * Mathf.PI / count) / Mathf.Sin(Mathf.PI / 6);
				width = Mathf.Max(width, 0.1f);
			}

			for (int i = 0; i < _levelMeters.Length; i++)
			{
				if (i < count)
				{
					float angleR = i * 2.0f * Mathf.PI / count;
					float angle = i * 360.0f / count + 90.0f;

					_levelMeters[i].transform.localPosition =
						new Vector3(4.0f * Mathf.Sin(angleR), 0.0f, 4.0f * Mathf.Cos(angleR));
					_levelMeters[i].transform.localEulerAngles = new Vector3(0.0f, angle, 0.0f);
					_levelMeters[i].transform.localScale = new Vector3(1.0f, 1.0f, width);
				}
				_levelMeters[i].SetActive(i < count);
			}
		}).AddTo(this);
	}
}
