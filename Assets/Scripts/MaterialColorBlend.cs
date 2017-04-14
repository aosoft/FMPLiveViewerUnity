using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Renderer))]
public class MaterialColorBlend : MonoBehaviour
{
	public string _shaderKeyword = null;
	public string _propertyName = null;

	[ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
	public Color _colorA = new Color(0.0f, 0.0f, 0.0f, 1.0f);

	[ColorUsageAttribute(true, true, 0f, 8f, 0.125f, 3f)]
	public Color _colorB = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	public float _ratioValue = 0.0f;

	private ReactiveProperty<float> _propRatio = null;

	public MaterialColorBlend()
	{
	}

	void Awake()
	{
		var renderer = GetComponent<Renderer>();

		if (string.IsNullOrEmpty(_shaderKeyword) == false)
		{
			renderer.material.EnableKeyword(_shaderKeyword);
		}

		_propRatio = this.ObserveEveryValueChanged(x => x._ratioValue).ToReactiveProperty().AddTo(this);

		_propRatio.Subscribe(value =>
		{
			var rateB = Mathf.Min(1.0f, Mathf.Max(0.0f, value));
			var rateA = 1.0f - rateB;

			renderer.material.SetColor(_propertyName,
				new Color(
					_colorA.r * rateA + _colorB.r * rateB,
					_colorA.g * rateA + _colorB.g * rateB,
					_colorA.b * rateA + _colorB.b * rateB,
					_colorA.a * rateA + _colorB.a * rateB));
		}).AddTo(this);
	}


	public ReactiveProperty<float> Ratio
	{
		get
		{
			return _propRatio;
		}
	}
}

