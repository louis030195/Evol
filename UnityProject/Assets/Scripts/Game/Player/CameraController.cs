using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Player
{
	/// <summary>
	/// https://www.youtube.com/watch?v=blO039OzUZc
	/// </summary>
	public class CameraController : MonoBehaviour
	{

		[Header("Parameters")]
		public float MinimumY = -90f;
		public float MaximumY = 90f;

		public float Sensitivity = 3f;

		public float Smoothing = 2f;

		public GameObject Character;


		private Vector2 mouseLook;
		private Vector2 smoothV;
		
		// Use this for initialization
		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			//offset = transform.position - ToFollow.transform.position;
		}

		// Update is called once per frame
		void Update()
		{
			
			var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));


			md = Vector2.Scale(md, new Vector2(Sensitivity * Smoothing, Sensitivity * Smoothing));
			smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / Smoothing);
			smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / Smoothing);
			mouseLook += smoothV;
			mouseLook.y = Mathf.Clamp(mouseLook.y, MinimumY, MaximumY);


			transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
			Character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, Character.transform.up);
			

			if (Input.GetKey(KeyCode.Escape))
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}

			
		}
		
		void LateUpdate ()
		{
			//transform.position = ToFollow.transform.position + offset;
		}
	}
}
