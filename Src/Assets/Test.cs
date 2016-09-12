using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	[Range(0.0f, 1.0f)]
	public float min = 0.5f;

	[Range(0.0f, 1.0f)]
	public float max = 1.0f;

	// Use this for initialization
	void Start () {

		StartCoroutine(__update());
	}


	IEnumerator __update() {

		while(true) {

			float r = Random.Range(min, max);

			yield return new WaitForSeconds(r);

			GameObject o = Util_PoolManagerDatabase.GetObject("MYDATABASE/MyObject");

			if(o != null) {

				o.SetActive(true);

				o.transform.position = new Vector3(Random.Range(-2.0f, 2.0f),
					Random.Range(-2.0f, 2.0f),
					Random.Range(-2.0f, 2.0f));

				o.GetComponent<Util_PoolObject>().DestroyObject(time:2.0f);
			}
		}
	}
}
