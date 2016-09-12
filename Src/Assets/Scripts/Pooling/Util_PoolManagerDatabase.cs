using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
[System.Serializable]
#endif
public class Util_PoolManagerDatabase : MonoBehaviour {

	[Header("Async Creation")]
	public bool _async = false;	

	[Header("Tag")]
	public string _tag = "";

	[Tooltip("Key loading")]
	[HideInInspector]
	public int _key = 0;

	[Tooltip("Loading objects at start")]
	[Space(5)]
	public bool _loadAtStart = false;

	[Tooltip("Non pooling")]
	[Space(5)]
	public bool _noPooling = false;

	// pooling database
	[Header("Pooling definitions")]
	[Space(1)]
	public PoolingDefinition [] _poolingDefinitions = new PoolingDefinition[0];

	/// <summary>
	/// The _pooling data base.
	/// </summary>
	public Util_PoolManager [] _poolingDataBase;

	/// <summary>
	/// The _instance.
	/// </summary>
	private static Dictionary<string, Util_PoolManagerDatabase> _instance = new Dictionary<string, Util_PoolManagerDatabase>();

	private static  Util_PoolManagerDatabase __BULLETS;
	public static Util_PoolManagerDatabase BULLETS {

		get {
			return __BULLETS;
		}
	}

	private static  Util_PoolManagerDatabase __ENEMY;
	public static Util_PoolManagerDatabase ENEMY {

		get {
			return __ENEMY;
		}
	}

	private static  Util_PoolManagerDatabase __FX;
	public static Util_PoolManagerDatabase FX {

		get {
			return __FX;
		}
	}

	private Util_PoolObject [] _objectToDestroy = new Util_PoolObject[256];
	public void AddObjectToDestroy(Util_PoolObject o) {

		for(int i = 0; i < 256; ++i) {
			if(_objectToDestroy[i] == null) {

				_objectToDestroy[i] = o;
				return;
			}
		}
	}

	public static GameObject GetObject(string tag, bool autoresize = true, bool forceUseFirst = false) {

		char [] splitter = new char[]{'/'};
		string [] args = tag.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
		if(args.Length != 2) {

			Debug.LogWarning(string.Format("*** Util_PoolManagerDatabase: Error tags count (es: <database>/<object_name>: {0}", tag));

			return null;
		}

		Util_PoolManagerDatabase db = GetInstance(args[0]);
		Util_PoolManager m = db.GetManager(args[1]);

		if(db == null) {

			Debug.LogWarning(string.Format("*** Util_PoolManagerDatabase: Database not found: {0}", args[0]));

			return null;
		}

		if(m == null) {

			Debug.LogWarning(string.Format("*** Util_PoolManager: Manager not found: {0}", args[1]));

			return null;
		}

		GameObject o = GetInstance(args[0]).GetManager(args[1]).GetObject(autoresize, forceUseFirst);
//		if(autoresize) {
//			o = GetInstance(args[0]).GetManager(args[1]).GetObject(false, forceUseFirst);
//			if(o == null) {
//
//				Debug.LogWarning(string.Format("*** Util_PoolManager: Error to create object, waiting for resize: {0}", args[1]));
//				return null;
//			}
//		}
//
		return o;
	}

	private void UpdateObjectToDestroy() {

		for(int i = 0; i < 256; ++i) {
			if(_objectToDestroy[i] != null) {

				_objectToDestroy[i].OnUpdate();
				if(_objectToDestroy[i].gameObject.activeSelf == false)
					_objectToDestroy[i] = null;
			}
		}
	}
		
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static Util_PoolManagerDatabase GetInstance(string tag) {

		if(_instance.ContainsKey(tag)) {

			Util_PoolManagerDatabase ret = _instance[tag];

			return ret;
		}
		else {

			Debug.LogWarning(string.Format("Cannot found {0} Key in Util_PoolManagerDatabase", tag));
		}

		return null;
	}

	public static Util_PoolManagerDatabase GetInstance(int index) {

		foreach(KeyValuePair<string, Util_PoolManagerDatabase> entry in _instance)
		{
			// do something with entry.Value or entry.Key
			return entry.Value;
		}

		return null;
	}

	public static string [] GetInstancesName() {

		string [] ret = new string[_instance.Count];

		int index = 0;

		foreach(KeyValuePair<string, Util_PoolManagerDatabase> o in _instance) {

			ret[index++] = o.Key;
		}

		return ret;
	}

	/// <summary>
	/// Gets the manager.
	/// </summary>
	/// <returns>The manager.</returns>
	/// <param name="tag">Tag.</param>
	private Util_PoolManager __getManager(string tag) {

		if(_poolingDataBase == null)
			return null;

		foreach(Util_PoolManager manager in _poolingDataBase) {
			
			if(manager != null && manager._tag == tag)
				return manager;
		}

		return null;
	}

	string _cachedTag = "";
	Util_PoolManager _cachedManager = null;

	/// <summary>
	/// Gets the manager.
	/// </summary>
	/// <returns>The manager.</returns>
	/// <param name="tag">Tag.</param>
	public Util_PoolManager GetManager(string tag) {

		if(_cachedTag == tag)
			return _cachedManager;

		_cachedManager = __getManager(tag);
		_cachedTag = tag;

		return _cachedManager;
	}
		
	#region MonoBehaviour

	bool _loaded = false;
	public bool IsLoaded {

		get {

			return _loaded;
		}
	}

	IEnumerator __awake() {

		int index = 0;
		int count = _poolingDataBase.Length;

		if(count < _poolingDefinitions.Length) {

			_poolingDataBase = new Util_PoolManager[_poolingDefinitions.Length];

			while(count < _poolingDefinitions.Length) {

				Util_PoolManager o = new Util_PoolManager();

				o._noPooling = _noPooling;

				PoolingDefinition p = _poolingDefinitions[count++];

				if(p._amount == 0 && !_noPooling) {
					
					Debug.LogWarning("Create a pooling with 0 element!");
					
					continue;
				}

				o.Create(p, gameObject);

				yield return o.waitingForCompletition();

				_poolingDataBase[index++] = o;
			}
		}

		yield return new WaitForEndOfFrame();

		_loaded = true;

		__ENEMY = Util_PoolManagerDatabase.GetInstance("ENEMY");
		__FX = Util_PoolManagerDatabase.GetInstance("FX");
		__BULLETS = Util_PoolManagerDatabase.GetInstance("BULLETS");

		#if UNITY_EDITOR

		string children = "";

		for(int i = 0; i < transform.childCount; ++i) {

			children += transform.GetChild(i).name;
			children += "\n";
		}
			
		Debug.Log(transform.name);
		Debug.Log(children);

		#endif
	}

	public void UnLoad() {

		foreach(Util_PoolManager o in _poolingDataBase) {

			o.Free();
		}

		_poolingDataBase = new Util_PoolManager[0];
		_cachedTag = "";
		_cachedManager = null;
	}

	public void Reset() {

		if(_poolingDataBase != null) {

			foreach(Util_PoolManager o in _poolingDataBase) {

				if(o != null)
					o.Reset();
			}
		}
	}

	public IEnumerator waitingForCompletition() {

		while(!_loaded) yield return new WaitForEndOfFrame();
	}

	public void Load(bool _async = true) {

		if(_async) {

			StartCoroutine(__awake());
		}
		else {

			int index = 0;
			_poolingDataBase = new Util_PoolManager[_poolingDefinitions.Length];

			foreach(PoolingDefinition pooling in _poolingDefinitions) {

				Util_PoolManager o = new Util_PoolManager();
				if(pooling._amount == 0) {

					Debug.LogWarning("Create a pooling with 0 element!");

					continue;
				}

				o.Create(pooling);

				_poolingDataBase[index++] = o;
			}

			_loaded = true;
		}
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {

		_instance[_tag] = this;

		// DontDestroyOnLoad(gameObject);

		if(_loadAtStart) {

			Load(false);
		}
	}

	void OnDestroy() {

		_instance.Remove(_tag);
	}

	void Update() {

		UpdateObjectToDestroy(); // iteriamo gli oggetti che devono essere distrutti
	}

	#endregion
}
