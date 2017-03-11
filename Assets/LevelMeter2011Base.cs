using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;

public class LevelMeter2011Base : MonoBehaviour
{
	public GameObject _levelMeterPrefab;

	void Start()
	{
		for (int i = 0; i < FMPPartWork.MaxChannelCount; i++)
		{
			var o = Instantiate(_levelMeterPrefab, new Vector3(i - 32, 0, 0), new Quaternion());
			o.transform.parent = this.transform;
			var l = o.GetComponent<LevelMeter>();
			l.PartWork.Value = FMPManager.FMPWork.Parts[i];
		}
	}
}
