using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to update stats of the agent
/// </summary>
public class LivingBeingController : MonoBehaviour {


    LivingBeing livingBeing;

	// Use this for initialization
	void Start () {
        livingBeing = GetComponent<HerbivorousAgent>().getLivingBeing();
        InvokeRepeating("UpdateStats", 0, 1);
	}
	
	// Update is called once per frame
	void Update () {
        livingBeing.Life += livingBeing.Satiety >= 50 ? 1f : -1f;
        livingBeing.Satiety -= 10f;

        livingBeing.Life = livingBeing.Life > 100 ? 100 : livingBeing.Life < 0 ? 0 : livingBeing.Life;
        livingBeing.Satiety = livingBeing.Satiety > 100 ? 100 : livingBeing.Satiety < 0 ? 0 : livingBeing.Satiety;
    }

    void UpdateStats()
    {
        print(livingBeing.ToString());
    }
}
