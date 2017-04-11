using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Renderer))]
public class MaterialColorBlend : MonoBehaviour
{
	public string _shaderKeyword = null;
	public string _propertyName = null;
	public Color _colorA = new Color(0.0f, 0.0f, 0.0f, 1.0f);
	public Color _colorB = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	private FloatReactiveProperty _propRatio = new FloatReactiveProperty();

	void Awake()
	{
		var renderer = GetComponent<Renderer>();

		if (string.IsNullOrEmpty(_shaderKeyword) == false)
		{
			renderer.material.EnableKeyword(_shaderKeyword);
		}

		_propRatio.Subscribe(value =>
		{
			var rateA = Mathf.Min(1.0f, Mathf.Max(0.0f, value));
			var rateB = 1.0f - rateA;

			renderer.material.SetColor(_propertyName,
				new Color(
					_colorA.r * rateA + _colorB.r * rateB,
					_colorA.g * rateA + _colorB.g * rateB,
					_colorA.b * rateA + _colorB.b * rateB,
					_colorA.a * rateA + _colorB.a * rateB));

		}).AddTo(this);
	}


	public FloatReactiveProperty Ratio
	{
		get
		{
			return _propRatio;
		}
	}
}

