using System;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Spell
{
	public class Portal : SpellBase
	{
		public GameObject Portal1;
		public GameObject Portal2;
		
		// Use this for initialization
		protected override void Start()
		{
			if (!gameObject.GetPhotonView().IsMine)
				return;
			base.Start();
			// TODO: wait the good time of animation to throw spell (animation event)
			// Play animation
			// Caster.Item1.GetComponent<Animator>().SetTrigger("Attack1Trigger");
			Portal2.transform.position = Position.AboveGround(Position.RandomPositionAround(Vector3.zero, 50), 5);

			// Destroy the portal after 20 seconds
			Invoke(nameof(DestroyAfter), 20);
		}

		private void DestroyAfter()
		{
			PhotonNetwork.Destroy(gameObject.GetPhotonView());
		}

		public void TeleportPosition(Transform other)
		{
			if (Vector3.Distance(other.position, Portal1.transform.position) < 20)
			{
				other.transform.position = new Vector3(Portal2.transform.position.x+5, Portal2.transform.position.y, Portal2.transform.position.z);
			}
			else
			{
				other.transform.position = new Vector3(Portal1.transform.position.x+5, Portal1.transform.position.y, Portal1.transform.position.z);
			}
		}





		// Update is called once per frame
		/*
		void Update () {
			
		}*/
	}
}
