using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFMP;
using UniRx;

public class TitlePresenter : MonoBehaviour
{
	public UnityEngine.UI.Text _title;
	public UnityEngine.UI.Text _musicCreator;

	// Use this for initialization
	void Start()
	{
		var animator = GetComponent<Animator>();

		FMPManager.FMPWork.MusicTitle.SubscribeToText(_title).AddTo(this);
		FMPManager.FMPWork.MusicCreator.SubscribeToText(_musicCreator).AddTo(this);
		FMPManager.FMPWork.MusicStartEvent.Subscribe(_ =>
		{
			animator.SetTrigger("Start");
		}).AddTo(this);
	}
}
