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
		FMPManager.FMPWork.MusicTitle.SubscribeToText(_title).AddTo(this);
		FMPManager.FMPWork.MusicCreator.SubscribeToText(_musicCreator).AddTo(this);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
