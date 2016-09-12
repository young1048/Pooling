using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Util pool loading: is the core of Pooling manager.
/// The cass have a list Util_PoolManagerDatabase objects.
/// </summary>
public class Util_PoolLoading : MonoBehaviour {

	#region Instance
	protected static Util_PoolLoading _instance;
	public static Util_PoolLoading Instance {

		get {

			return _instance;
		}

		set {

			_instance = value;
		}
	}
	#endregion

	public List<string> _keys;

	#if UNITY_EDITOR
	public List<Util_PoolManagerDatabase> _database;
	#endif

	public bool _autoplay = false;

	public GameObject _containeres = null;

	[Space(10)]
	public bool _dontDestroy = false;

	private bool _loaded = false;

	private float _progress = 0.0f;

	private int _totalToLoad = 0;

	private int _currentLoading = 0;

	public int Progress {

		get {

			return (int)(_progress * 100.0f);
		}
	}

	public void DestroyGameObject(GameObject o) {

		DestroyObject(o);
	}

	/// <summary>
	/// Load by specified keys
	/// </summary>
	/// <param name="keys">Keys.</param>
	IEnumerator __load(List<string> keys) {

		#if UNITY_EDITOR
		_database.Clear();

		for(int i = 0; i < transform.childCount; ++i) {

			Util_PoolManagerDatabase o = transform.GetChild(i).GetComponent<Util_PoolManagerDatabase>();
			if(o != null && keys.Contains(_keys[o._key])) {

				_totalToLoad++;
			}

			_database.Add(o);
		}
		#endif

		for(int i = 0; i < transform.childCount; ++i) {

			Util_PoolManagerDatabase o = transform.GetChild(i).GetComponent<Util_PoolManagerDatabase>();
			if(o != null && keys.Contains(_keys[o._key])) {

				Debug.Log(string.Format("*** Util_PoolLoading: - load {0}", o.name));

				o.Load(true);

				yield return o.waitingForCompletition();

				_currentLoading++;

				_progress = (float)_currentLoading / (float)_totalToLoad;
			}

			// yield return new WaitForEndOfFrame();
		}

		_progress = 1.0f;

		_loaded = true;
	}

	public void LoadByKeys(List<string> keys) {

		_loaded = false;
		_progress = 0.0f;
		_totalToLoad = 0;
		_currentLoading = 0;

		StartCoroutine(__load(keys));
	}

	public void Lock() {

		_loaded = false;
	}

	public void UnLock() {

		_loaded = true;
	}

	public bool IsLoaded {

		get {

			return _loaded;
		}
	}

	public void ForceUnloaded() {

		_loaded = false;
	}

	public IEnumerator waitingCompletition() {

		while(!_loaded) yield return new WaitForEndOfFrame();
	}

	public void UnLoad(string key) {

		for(int i = 0; i < transform.childCount; ++i) {

			Util_PoolManagerDatabase o = transform.GetChild(i).GetComponent<Util_PoolManagerDatabase>();
			if(o != null && _keys[o._key] == key) {

				o.UnLoad();
				return;
			}
		}
	}

	IEnumerator __reset() {

		yield return new WaitForEndOfFrame();

		for(int i = 0; i < _containeres.transform.childCount; ++i) {

			_containeres.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void ResetAllObjects() {

		// StartCoroutine(__reset());

//		for(int i = 0; i < _containeres.transform.childCount; ++i) {
//
//			_containeres.transform.GetChild(i).gameObject.SetActive(false);
//		}

		for(int i = 0; i < transform.childCount; ++i) {

			Util_PoolManagerDatabase o = transform.GetChild(i).GetComponent<Util_PoolManagerDatabase>();
			if(o != null) {
				
				o.Reset();
			}
		}
	}

	void Awake() {

		// the system search Containers object. Don't create Util_PoolLooading alone. Use Tools/Pooling/Create
		_containeres = transform.FindChild("Containers").gameObject;

		if(_instance != null) {
			DestroyObject(gameObject);
			return;
		}
			
		if(_dontDestroy) {
			
			DontDestroyOnLoad(gameObject); // preserviamo il pooling
		}

		_instance = this;

		#if UNITY_EDITOR
		if(!Application.isPlaying)
			return;
		#endif
		if(_autoplay) {

			// load pool with kEverytime key.
			// if you want create another key and load at another time another object list,

			// 1) create key

			// 2) create a manager and set you key

			// 3) when you load your scene, use on you OnEnable Util_PoolLoading.Instance.LoadByKeys(new List<string>(new string[]{<you key>}));

			// 4) on your OnDelete run Util_PoolLoading.Instance.UnLoad(<you KeyCode>)

			Util_PoolLoading.Instance.LoadByKeys(new List<string>(new string[]{"kEverytime"}));


		}
	}

	void OnDisable() {

		// _instance = null;
	}
}
