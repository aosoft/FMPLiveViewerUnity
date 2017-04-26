using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// アスペクト比を無視して全面リサイズする。
/// </summary>
public class BGSizeCorrector : MonoBehaviour
{
	public Camera _camera;

	void Awake()
	{
		if (_camera != null && _camera.orthographic)
		{
			float height = _camera.orthographicSize * 2;

			this.ObserveEveryValueChanged(x => new Tuple<int, int>(Screen.width, Screen.height))
				.Subscribe(value =>
				{
					this.transform.localScale = new Vector3(
						height * value.Item1 / value.Item2,
						height, 1.0f);
				}).AddTo(this);
		}
	}
}
