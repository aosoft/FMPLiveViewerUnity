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
		var animator2 = _header.GetComponent<Animator>();

		FMPManager.FMPWork.MusicTitle.SubscribeToText(_title).AddTo(this);
		FMPManager.FMPWork.MusicCreator.SubscribeToText(_musicCreator).AddTo(this);
		FMPManager.FMPWork.MusicStartEvent.Subscribe(_ =>
		{
			_header.GetComponent<CanvasGroup>().alpha = 0.0f;
			animator.SetTrigger("Start");
		}).AddTo(this);
		FMPManager.FMPWork.MusicStartEvent
			.Delay(System.TimeSpan.FromSeconds(5))
			.Subscribe(_ =>
			{
				animator2.SetTrigger("Start");
			}).AddTo(this);
	}
}
