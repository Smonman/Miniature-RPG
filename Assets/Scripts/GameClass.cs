using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniRPG
{
	[System.Serializable]
	public enum Stats { Strength, Intellect, Discipline, Stamina, Tempo, Speed };

	[System.Serializable]
	public class GameClass : MonoBehaviour
	{
		#region Variables
		[SerializeField]
		private string className;
		[SerializeField]
		private string classDesc;
		[SerializeField]
		private Color classColor;
		[SerializeField]
		private Stats[] primaryStats;
		[SerializeField]
		private Dictionary<Stats, int> statValues;
		[SerializeField]
		private SpellResource resource;
		[SerializeField]
		private Spell[] spells;

		public string ClassName
		{
			get
			{
				return className;
			}

			set
			{
				if (value.Length <= 0)
				{
					className = "Unnamed Class";
				}
				else
				{
					className = value;
				}
			}
		}

		public string ClassDesc
		{
			get
			{
				return classDesc;
			}

			set
			{
				classDesc = value;
			}
		}

		public Color ClassColor
		{
			get
			{
				return classColor;
			}

			set
			{
				classColor = value;
			}
		}

		public Stats[] PrimaryStats
		{
			get
			{
				return primaryStats;
			}

			set
			{
				primaryStats = value;
			}
		}

		public SpellResource Resource
		{
			get
			{
				return resource;
			}

			set
			{
				resource = value;
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

		}
	}
}
