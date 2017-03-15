using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFMP;
using UniRx;
using UniRx.Triggers;

public class TitlePresenter : MonoBehaviour
{
	public GameObject _header;
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
			_header.GetComponent<UnityEngine.CanvasGroup>().alpha = 0.0f;
			animator.SetTrigger("Start");
		}).AddTo(this);
		animator.OnAnimatorMoveAsObservable().Subscribe(_ =>
		{
			_header.GetComponent<UnityEngine.CanvasGroup>().alpha = 1.0f;
		}).AddTo(this);
	}
}
