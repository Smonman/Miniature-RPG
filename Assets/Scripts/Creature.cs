using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniRPG
{
	public class Creature : MonoBehaviour
	{
		// TODO:
		// call events on variable changes
		// for example in the setter of curHealth call OnHealthChange
		//
		#region Variables
		[SerializeField]
		private string creatureName;
		[SerializeField]
		private int maxLifePoints;
		[SerializeField]
		private int curLifePoints;
		[SerializeField]
		private int maxHealthPoints;
		[SerializeField]
		private int curHealthPoints;
		[SerializeField]
		private int curShieldPoints;
		[SerializeField]
		private int maxShieldPoints;
		[SerializeField]
		private bool isDead;
		[SerializeField]
		private bool canBeRessurected;
		[SerializeField]
		private SortedDictionary<Creature, int> aggroDict;
		[SerializeField]
		private Creature target;
		//private GameClass gameClass;
		[SerializeField]
		private bool canAttack;
		[SerializeField]
		private bool inFight;

		public delegate void OnCreatureNameChangedDelegate(Creature creature, string newName);
		public event OnCreatureNameChangedDelegate OnCreatureNameChanged;

		public delegate void OnLifePointsChangedDelegate(Creature creature, int newAmount);

		public event OnLifePointsChangedDelegate OnMaxLifePointsChanged;
		public event OnLifePointsChangedDelegate OnCurLifePointsChanged;
		public event OnLifePointsChangedDelegate OnMaxHealthPointsChanged;
		public event OnLifePointsChangedDelegate OnCurHealthPointsChanged;
		public event OnLifePointsChangedDelegate OnMaxShieldPointsChanged;
		public event OnLifePointsChangedDelegate OnCurShieldPointsChanged;

		public delegate void OnDeathDelegate(Creature creature);
		public event OnDeathDelegate OnDeath;

		public delegate void OnTargetChangedDelegate(Creature creature, Creature target);
		public event OnTargetChangedDelegate OnTargetChanged;

		public delegate void OnInFightChangedDelgate(Creature creature, bool curInFight);
		public event OnInFightChangedDelgate OnInFightChanged;

		public string CreatureName
		{
			get
			{
				return creatureName;
			}

			set
			{
				if (value.Length <= 0)
				{
					creatureName = "Unnamed Creature";
				}
				else
				{
					creatureName = value;
				}
				if (OnCreatureNameChanged != null)
				{
					OnCreatureNameChanged(this, CreatureName);
				}
			}
		}

		public int MaxLifePoints
		{
			get
			{
				return MaxHealthPoints + MaxShieldPoints;
			}

			protected set
			{
				maxLifePoints = Mathf.Abs(value);
				if (OnMaxLifePointsChanged != null)
				{
					OnMaxLifePointsChanged(this, MaxHealthPoints);
				}
			}
		}

		public int CurLifePoints
		{
			get
			{
				int amount = CurHealthPoints + CurShieldPoints;
				return Mathf.Clamp(amount, 0, MaxHealthPoints);
			}

			private set
			{
				curLifePoints = Mathf.Clamp(value, 0, MaxLifePoints);
				if (OnCurLifePointsChanged != null)
				{
					OnCurLifePointsChanged(this, CurHealthPoints);
				}
			}
		}

		public int MaxHealthPoints
		{
			get
			{
				return maxHealthPoints;
			}

			set
			{
				maxHealthPoints = Mathf.Abs(value);
				if (OnMaxHealthPointsChanged != null)
				{
					OnMaxHealthPointsChanged(this, MaxHealthPoints);
				}
			}
		}

		public int CurHealthPoints
		{
			get
			{
				return curHealthPoints;
			}

			set
			{
				curHealthPoints = Mathf.Clamp(value, 0, MaxHealthPoints);
				Debug.Log("Creature::CurHealthPoints::Set");
				if (OnCurHealthPointsChanged != null)
				{
					OnCurHealthPointsChanged(this, CurHealthPoints);
					Debug.Log("Creature::CurHealthPoints::Set Called OnCurHealthPointsChanged");
				}
			}
		}

		public int MaxShieldPoints
		{
			get
			{
				return maxShieldPoints;
			}

			set
			{
				maxShieldPoints = value;
				if (OnMaxShieldPointsChanged != null)
				{
					OnMaxShieldPointsChanged(this, MaxShieldPoints);
				}
			}
		}

		public int CurShieldPoints
		{
			get
			{
				return curShieldPoints;
			}

			set
			{
				curShieldPoints = Mathf.Clamp(Mathf.Abs(value), 0, MaxShieldPoints);
				if (OnCurShieldPointsChanged != null)
				{
					OnCurShieldPointsChanged(this, CurShieldPoints);
				}
			}
		}

		public bool IsDead
		{
			get
			{
				return isDead;
			}

			private set
			{
				isDead = value;
				if (isDead == true && curHealthPoints > 0)
				{
					Debug.LogWarning("You set isDead to true even though the current healthpoints are above 0.");
				}
			}
		}

		public bool CanBeRessurected
		{
			get
			{
				return canBeRessurected;
			}

			set
			{
				canBeRessurected = value;
			}
		}

		public Creature Target
		{
			get
			{
				return target;
			}

			private set
			{
				target = value;
				if (OnTargetChanged != null)
				{
					OnTargetChanged(this, Target);
				}
			}
		}

		public bool CanAttack
		{
			get
			{
				return canAttack;
			}

			set
			{
				canAttack = value;
			}
		}

		public bool InFight
		{
			get
			{
				return inFight;
			}

			private set
			{
				inFight = value;
				if (OnInFightChanged != null)
				{
					OnInFightChanged(this, InFight);
				}
			}
		}
		#endregion

		protected virtual void OnEnable()
		{
			// register events
			OnDeath += Die;
		}

		protected virtual void OnDisable()
		{
			// unregister events
			OnDeath -= Die;
		}

		protected virtual void Awake()
		{
			aggroDict = new SortedDictionary<Creature, int>();
		}

		protected virtual void Start()
		{

		}

		protected virtual void Update()
		{
			//CheckAggroList();

			// _____________________________________
			// TEST
			if (Input.GetKeyDown(KeyCode.G))
			{
				Debug.Log("Creature::Update Take Damage Test");
				TakeDamage(this, 10);
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				Debug.Log("Creature::Update Heal Test");
				RecieveHeal(this, 20);
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				Debug.Log("Creature::Update Change Maxumus Health Points Test");
				MaxHealthPoints += 20;
			}
			if (Input.GetKeyDown(KeyCode.J))
			{
				Debug.Log("Creature::Update Recieve Shield Test");
				RecieveShield(this, 10);
			}
		}

		public void TakeDamage(Creature sender, int amount)
		{
			// Go through all of the buffs and debuffs

			if (CurShieldPoints > 0)
			{
				int overflow = CurShieldPoints - amount;
				if (overflow <= 0)
				{
					CurShieldPoints = 0;
					CurHealthPoints -= Mathf.Abs(overflow);
				}
				else
				{
					CurShieldPoints -= amount;
				}
			}
			else
			{
				CurHealthPoints -= amount;
				Debug.Log("Creature::TakeDamage amount: " + amount + " CurHealthPoints: " + CurHealthPoints);
			}
			if (CurHealthPoints <= 0)
			{
				if (OnDeath != null)
				{
					OnDeath(this);
				}
			}
		}

		public void RecieveHeal(Creature sender, int amount)
		{
			// Go through all of the buffs and debuffs

			int tempHealthPoints = CurHealthPoints + amount;
			CurHealthPoints = Mathf.Clamp(tempHealthPoints, CurHealthPoints, MaxHealthPoints);
		}

		public void RecieveShield(Creature sender, int amount)
		{
			// Go through all of the buffs and debuffs

			CurShieldPoints += amount;
		}

		public void AddToAggoList(Creature aggroCreature, int aggroAmount)
		{
			// Maybe go through all of the buffs and debuffs

			if (aggroDict.ContainsKey(aggroCreature))
			{
				aggroDict[aggroCreature] = aggroAmount;
			}
			else
			{
				aggroDict.Add(aggroCreature, aggroAmount);
			}
		}

		public void RemoveFromAggroList(Creature aggroCreature)
		{
			if (aggroDict.ContainsKey(aggroCreature))
			{
				aggroDict.Remove(aggroCreature);
			}
		}

		void CheckAggroList()
		{
			foreach (Creature creature in aggroDict.Keys)
			{
				if (aggroDict[creature] <= 0)
				{
					aggroDict.Remove(creature);
				}
			}
		}

		void Die(Creature c)
		{
			// Go through all of the buffs and debuffs

			IsDead = true;
			Debug.Log("Creature " + CreatureName + " died!");
		}
	}
}
