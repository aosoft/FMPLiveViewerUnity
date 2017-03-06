using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMP.FMP7;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public class FMPManager : MonoBehaviour
{
	RxFMPWork _fmpWork = null;

	public StringReactiveProperty _playTime = new StringReactiveProperty();

	public FMPManager()
	{
	}

	public void Awake()
	{
		_fmpWork = new RxFMPWork();

		this.UpdateAsObservable().Subscribe(_ => _fmpWork.Update());

		_fmpWork.PlayTime.Select(value => value.ToString()).Subscribe(value => _playTime.Value = value);
	}

	void OnDestroy()
	{
		if (_fmpWork != null)
		{
			_fmpWork.Dispose();
			_fmpWork = null;
		}
	}



}
