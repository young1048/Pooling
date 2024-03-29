﻿using UnityEngine;
using System.Collections;

public class TestPooling : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		StartCoroutine(__update());
	}


	IEnumerator __update() {

		while(true) {

			float r = Random.Range(0.0f, .5f);

			yield return new WaitForSeconds(r);

			// get a object inside databse
			GameObject o = Util_PoolManagerDatabase.GetObject("ENEMY/MyObject");

			if(o != null) {

				o.SetActive(true);

				o.transform.position = new Vector3(Random.Range(-2.0f, 2.0f),
												   Random.Range(2.0f, 4.0f),
												   Random.Range(-2.0f, 2.0f));

				o.GetComponent<Util_PoolObject>().DestroyObject(time:2.0f);
			}
		}
	}
}
