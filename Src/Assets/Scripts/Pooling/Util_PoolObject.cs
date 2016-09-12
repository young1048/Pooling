using UnityEngine;
using System.Collections;

public class Util_PoolObject : MonoBehaviour {

	[HideInInspector]
	public Transform _parent;

	[HideInInspector]
	public Util_PoolManager _poolManager;

	enum DestroyState {

		kNone,
		kPrepareDestroy,
		kDestroy
	}

	DestroyState _destroystate = DestroyState.kNone;

	void Awake() {

		_parent = transform.parent;
	}

	void OnEnable() {

		_destroystate = DestroyState.kNone;
	}

	protected void _destroy() {

		#if NO_POOLING
		Destroy(gameObject);
		#else
		if(_poolManager != null) {

			if(_poolManager._noPooling) {

				Destroy(gameObject);
			}
			else {
				
				_poolManager.DestroyObject(this);
			}
		}
		else {

			//gameObject.SetActive(false);
			Destroy(gameObject);
		}
		#endif

		_destroystate = DestroyState.kNone;
	}
		
	private IEnumerator __destroyAfter(float t) {

		yield return new WaitForSeconds(t);

		Util_PoolManagerDatabase.GetInstance(0).AddObjectToDestroy(this);

		_destroystate = DestroyState.kPrepareDestroy;
	}

	public virtual void DestroyObject(bool now = false, float time = 0.0f) {

		if(time > 0.0f) {

			StartCoroutine(__destroyAfter(time));

			return;
		}

		if(transform.parent != null) {

			if(transform.parent.tag == "PoolingContainer") {
				_destroy();
				return;
			}
		}

		if(now) {

			_destroy();
			return;
		}

		Util_PoolManagerDatabase.GetInstance(0).AddObjectToDestroy(this);

		_destroystate = DestroyState.kPrepareDestroy;
	}

	public void OnUpdate() {

		switch(_destroystate)
		{
		case DestroyState.kPrepareDestroy:

			transform.parent = _parent;

			_destroystate = DestroyState.kDestroy;

			break;

		case DestroyState.kDestroy:

			_destroy();

			break;
		}
	}
}
