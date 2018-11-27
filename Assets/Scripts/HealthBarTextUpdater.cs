using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniRPG
{

	public class HealthBarTextUpdater : MonoBehaviour
	{
		#region Variables
		private Creature player;
		private int maxHealthPoints;
		private int curHealthPoints;
		private float curHealthPercentage;

		[SerializeField]
		private Image image;
		#endregion

		void OnEnable()
		{
			player.OnMaxHealthPointsChanged += MaxHealthPointsChanged;
			player.OnCurHealthPointsChanged += CurHealthPointsChanged;
		}

		void OnDisable()
		{
			player.OnMaxHealthPointsChanged -= MaxHealthPointsChanged;
			player.OnCurHealthPointsChanged -= CurHealthPointsChanged;
		}

		void Awake()
		{
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		}

		void Start()
		{
			maxHealthPoints = player.MaxHealthPoints;
			curHealthPoints = player.CurHealthPoints;
		}

		void Update()
		{

		}

		void MaxHealthPointsChanged(Creature c, int newAmount)
		{
			maxHealthPoints = newAmount;
			CalculatePercentage();
			UpdateFillAmount();
		}

		void CurHealthPointsChanged(Creature c, int newAmount)
		{
			curHealthPoints = newAmount;
			CalculatePercentage();
			UpdateFillAmount();
		}

		void CalculatePercentage()
		{
			curHealthPercentage = curHealthPoints / maxHealthPoints;
			Debug.Log(curHealthPercentage);
		}

		void UpdateFillAmount()
		{
			image.fillAmount = curHealthPercentage;
		}
	}
}
