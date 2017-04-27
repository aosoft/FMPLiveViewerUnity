using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 背景をスクリーンサイズに合わせてリサイズする。
/// アスペクト比を無視して全面リサイズする。
/// </summary>
public class BGSizeCorrector : MonoBehaviour
{
	public Camera _camera;

	static private float _far = 100.0f;

	void Awake()
	{
		if (_camera != null && _camera.orthographic == false)
		{
			this.transform.localPosition = new Vector3(0.0f, 0.0f, _far);

			_camera.ObserveEveryValueChanged(x => new Tuple<int, int>(x.pixelWidth, x.pixelHeight))
				.Subscribe(value =>
				{
					var a = _camera.transform.InverseTransformPoint(_camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, _far)));
					var b = _camera.transform.InverseTransformPoint(_camera.ScreenToWorldPoint(new Vector3(value.Item1, value.Item2, _far)));
					this.transform.localScale = new Vector3(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y), 1.0f);
				}).AddTo(this);
		}
	}
}
