using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class TestPlayerCrawler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<CrawlerAgent>().brain.InitializeBrain(FindObjectOfType<Academy>(), null);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	private void FixedUpdate()
	{
		#if UNITY_STANDALONE
			GetComponent<CrawlerAgent>().target.z = (transform.Find("Body").transform.localPosition.z == 0 ? 1 :
				                                        transform.Find("Body").transform.localPosition.z) * 
			                                        (Input.GetAxis("Horizontal") != 0 ? Input.GetAxis("Horizontal") * 10 : 1);
			GetComponent<CrawlerAgent>().target.x = (transform.Find("Body").transform.localPosition.x == 0 ? 1 :
				                                        transform.Find("Body").transform.localPosition.x) * 
			                                        (Input.GetAxis("Vertical") != 0 ? Input.GetAxis("Vertical") * 10 : 1);
		#endif


		#if UNITY_ANDROID

		
		//Check if Input has registered more than zero touches
		if (Input.touchCount > 0)
		{
			//Store the first touch detected.
			Touch myTouch = Input.touches[0];
                
			//Check if the phase of that touch equals Began
			if (myTouch.phase == TouchPhase.Began)
			{
				//If so, set touchOrigin to the position of that touch
				var touchOrigin = myTouch.position;
				GetComponent<CrawlerAgent>().target.z = touchOrigin.y;
				GetComponent<CrawlerAgent>().target.x = touchOrigin.x;
			}
                
			

		}
		#endif
	}
}
