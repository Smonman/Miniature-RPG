using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniRPG
{
	public enum SpellResource { Mana, Concentration, Rage };
	public enum SpellType { CD, DefCD, DMG, Heal, Disspell, Interrupt, Antistun, Stun, CC };

	public class Spell : MonoBehaviour
	{
		#region Variables
		private string spellName;
		private string spellDesc;
		private Image icon;
		private int cost;
		private float critChance;
		private float critMultiplier;
		private float castTime;
		private float curCastTime;
		private float coolDownTime;
		private float curCoolDownTime;
		private int range;
		private Action func;
		private SpellType type;
		private bool isAffectedByGCD;
		private CurState curState;
		private bool canBeCasted;

		private enum CurState { Casting, OnCooldown, Waiting };

		public delegate void OnTimeChangedDelegate(Spell spell, float curTime);
		public event OnTimeChangedDelegate OnCoolDownTimeChanged;
		public event OnTimeChangedDelegate OnCastTimeChanged;

		public string SpellName
		{
			get
			{
				return spellName;
			}

			set
			{
				if (value.Length <= 0)
				{
					spellName = "Unnamed Spell";
				}
				else
				{
					spellName = value;
				}
			}
		}

		public string SpellDesc
		{
			get
			{
				return spellDesc;
			}

			set
			{
				spellDesc = value;
			}
		}

		public int Cost
		{
			get
			{
				return cost;
			}

			set
			{
				cost = Mathf.Abs(value);
			}
		}

		public float CritChance
		{
			get
			{
				return critChance;
			}

			set
			{
				critChance = Mathf.Clamp01(value);
			}
		}

		public float CritMultiplier
		{
			get
			{
				return critMultiplier;
			}

			set
			{
				if (value == 0)
				{
					Debug.LogWarning("The CritMultiplier is set to 0! No damage is expected!");
				}
				critMultiplier = Mathf.Abs(value);
			}
		}

		public float CastTime
		{
			get
			{
				return castTime;
			}

			set
			{
				castTime = value;
			}
		}

		public float CurCastTime
		{
			get
			{
				return curCastTime;
			}

			set
			{
				curCastTime = Mathf.Clamp(value, 0, value);
				OnCastTimeChanged(this, CurCastTime);
			}
		}

		public float CoolDownTime
		{
			get
			{
				return coolDownTime;
			}

			set
			{
				coolDownTime = Mathf.Clamp(value, 0, value);
			}
		}

		public float CurCoolDownTime
		{
			get
			{
				return curCoolDownTime;
			}

			private set
			{
				curCoolDownTime = Mathf.Clamp(value, 0, value);
				if (OnCoolDownTimeChanged != null)
				{
					OnCoolDownTimeChanged(this, CurCoolDownTime);
				}
			}
		}

		public int Range
		{
			get
			{
				return range;
			}

			set
			{
				range = value;
			}
		}

		public Action Func
		{
			get
			{
				return func;
			}

			set
			{
				func = value;
			}
		}

		public SpellType Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
			}
		}

		public bool IsAffectedByGCD
		{
			get
			{
				return isAffectedByGCD;
			}

			set
			{
				isAffectedByGCD = value;
			}
		}

		public bool CanBeCasted
		{
			get
			{
				return canBeCasted;
			}

			set
			{
				canBeCasted = value;
			}
		}
		#endregion

		void Awake()
		{

		}

		void Start()
		{

		}

		void Update()
		{
			if (CurCoolDownTime <= 0)
			{
				curState = CurState.Waiting;
				CanBeCasted = true;
			}
			else
			{
				curState = CurState.OnCooldown;
				CurCoolDownTime -= Time.deltaTime;
				CanBeCasted = false;
			}
		}

		public void Cast()
		{
			StartCoroutine(WaitCastTime());
			if (CanBeCasted)
			{
				CurCoolDownTime = CoolDownTime;

				// check range
				// check cur cast time
				// check if target is possible line of sight
				// ...

				func();
			}
		}

		private IEnumerator WaitCastTime()
		{
			yield return new WaitForSeconds(CastTime);
		}

		public void Interrupt()
		{
			StopAllCoroutines();
		}
	}
}