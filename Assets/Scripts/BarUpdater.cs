using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;

namespace MiniRPG
{
	public class BarUpdater : MonoBehaviour
	{
		#region Variables
		[SerializeField]
		private Image image;
		[SerializeField]
		private TextMeshProUGUI curValueText;
		[SerializeField]
		private TextMeshProUGUI percentageText;

		[SerializeField]
		private float maxValue;
		[SerializeField]
		private float curValue;

		[SerializeField]
		private GameObject player;
		private Player p;

		private enum BarType
		{
			Heal, Shield, Mana
		}
		[SerializeField]
		private BarType type;

		#endregion

		void OnEnable()
		{
			p = player.GetComponent<Player>();

			switch (type)
			{
				case BarType.Heal:
					p.OnMaxHealthPointsChanged += MaxValueChanged;
					p.OnCurHealthPointsChanged += CurValueChanged;
					break;
				case BarType.Shield:
					p.OnMaxShieldPointsChanged += MaxValueChanged;
					p.OnCurShieldPointsChanged += CurValueChanged;
					break;
			}
		}

		void OnDisable()
		{
			switch (type)
			{
				case BarType.Heal:
					p.OnMaxHealthPointsChanged -= MaxValueChanged;
					p.OnCurHealthPointsChanged -= CurValueChanged;
					break;
				case BarType.Shield:
					p.OnMaxShieldPointsChanged -= MaxValueChanged;
					p.OnCurShieldPointsChanged -= CurValueChanged;
					break;
			}
		}

		void Awake()
		{

		}

		void Start()
		{
			switch (type)
			{
				case BarType.Heal:
					maxValue = p.MaxHealthPoints;
					curValue = p.CurHealthPoints;
					break;
				case BarType.Shield:
					maxValue = p.MaxShieldPoints;
					curValue = p.CurShieldPoints;
					break;
			}

			UpdateAll();
		}

		void Update()
		{

		}

		void FixedUpdate()
		{

		}

		void LateUpdate()
		{

		}

		void MaxValueChanged(Creature c, int newAmount)
		{
			maxValue = newAmount;
			Debug.Log("BarUpdater::MaxBalueChanged newAmount: " + newAmount);
			UpdateFillAmount();
			UpdateText(percentageText, (PercentageString(Percentage(maxValue, curValue))), null, "%");
		}

		void CurValueChanged(Creature c, int newAmount)
		{
			curValue = newAmount;
			Debug.Log("BarUpdater::CurValueChanged newAmount: " + newAmount);
			UpdateFillAmount();
			UpdateText(curValueText, curValue.ToString(), null, null);
			UpdateText(percentageText, (PercentageString(Percentage(maxValue, curValue))), null, "%");
		}

		void UpdateAll()
		{
			UpdateFillAmount();
			UpdateText(curValueText, curValue.ToString(), null, null);
			UpdateText(percentageText, (PercentageString(Percentage(maxValue, curValue))), null, "%");
		}

		void UpdateFillAmount()
		{
			Debug.Log("BarUpdater::UpdateFillAmount");
			image.fillAmount = Percentage(maxValue, curValue);
		}

		void UpdateText(TextMeshProUGUI txt, string text, string prefix, string suffix)
		{
			txt.text = prefix + " " + text + " " + suffix;
		}

		private float Percentage(float max, float cur)
		{
			return curValue / maxValue;
		}

		private string PercentageString(float percentage)
		{
			float p = percentage * 100;
			int rounded = Mathf.RoundToInt(p);
			return rounded.ToString();
		}
	}
}
