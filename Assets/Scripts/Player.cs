using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniRPG
{
	[System.Serializable]
	public class Player : Creature
	{
		#region Variables
		private GameClass gameClass;

		public GameClass GameClass
		{
			get
			{
				return gameClass;
			}

			set
			{
				gameClass = value;
			}
		}
		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
		}
	}
}
