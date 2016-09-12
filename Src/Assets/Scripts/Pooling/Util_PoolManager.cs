using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(Util_PoolObject))]
using System;

[System.Serializable]
public class PoolingDefinition {

	[HideInInspector]
	[TooltipAttribute("TAG Pooling")]
	private string _tag;

	[TooltipAttribute("Object pooled")]
	public GameObject _object;

	[TooltipAttribute("Object sereseved in pooling")]
	public int _amount;
}

[System.Serializable]
public class Util_PoolManager {

	/// <summary>
	/// The _prefabs object.
	/// </summary>
	public GameObject _prefabsObject;

	/// <summary>
	/// The _loaded.
	/// </summary>
	public bool _loaded = false;

	/// <summary>
	/// The _tag.
	/// </summary>
	public string _tag = "POOL";

	/// <summary>
	/// The pooled amount.
	/// </summary>
	public int _pooledAmount = 5;

	public bool _noPooling = false;

	/// <summary>
	/// The _pooled list.
	/// </summary>
	// private GameObject [] _pooledList = new GameObject[0];
	public class PoolElement {

		//
		public GameObject _element = null; // element to insert in the pooling system

		//
		public bool _reserved = false; // reserved element
	}

	private PoolElement [] _pooledList = new PoolElement[0];

	/// <summary>
	/// The index of the pooled list.
	/// </summary>
	// private int _pooledListIndex = 0;

	/// <summary>
	/// The _parent.
	/// </summary>
	private Transform _parent;

	private GameObject _poolingContainer;

	/// <summary>
	/// Sets the parent.
	/// </summary>
	/// <value>The parent.</value>
	public Transform Parent {

		set {

			_parent = value;
		}
	}
		
	/// <summary>
	/// Ares the available.
	/// </summary>
	/// <returns><c>true</c>, if available was ared, <c>false</c> otherwise.</returns>
	/// <param name="count">Count.</param>
	public bool IsAvailable(int count = 1) {

		foreach(PoolElement o in _pooledList) {

			if(o._element.activeInHierarchy || o._reserved)
				continue;

			count--;
			if(count == 0)
				return true;
		}

		if(count == 0)
			return true;

		return false;
	}

	public void Reset() {

		foreach(PoolElement o in _pooledList) {

			try {

				o._element.transform.parent = _poolingContainer.transform; // restore parent
				o._element.SetActive(false);
				o._reserved = false;
			}
			catch(MissingReferenceException e) {

				Debug.LogWarning(e.Message);
			}
		}
	}

	public void DestroyObject(Util_PoolObject o1) {

		if(o1.gameObject.activeSelf) {
			o1.gameObject.SetActive(false);
		}

		foreach(PoolElement o2 in _pooledList) {

			if(o1.gameObject == o2._element) {

				o2._reserved = false;
				break;
			}
		}
	}

	/// <summary>
	/// Get this instance.
	/// </summary>
	public GameObject GetObject(bool resize = true, bool forceUseFirst = false) {

		if(_poolingContainer == null) {
			
			_poolingContainer = GameObject.FindWithTag("PoolingContainer");
		}

		if(_noPooling) {

			GameObject o = GameObject.Instantiate(_prefabsObject) as GameObject;
			o.GetComponent<Util_PoolObject>()._poolManager = this;
			o.SetActive(true);
			//o.transform.parent = transform;
			return o;
		}
		else {

			if(forceUseFirst) {

				_pooledList[0]._reserved = true;

				return _pooledList[0]._element;
			}
				
			try {
				foreach(PoolElement o in _pooledList) {

					if(o._element.activeInHierarchy || o._reserved)
						continue;

					// riservare?
					o._reserved = true;

					// salva manager
					o._element.GetComponent<Util_PoolObject>()._poolManager = this;

					// found a object
					// o.gameObject.SetActive(true);
					return o._element.gameObject;
				}
			}
			catch(NullReferenceException e) {

				Debug.LogWarning("Errore, elemndo nullo nel pool " + _tag + ": " + e.Message);
				return null;
			}

			if(resize) {

				// error increment pool
				Debug.LogWarning("Error increment pool for " + _tag);
				Resize(_pooledList.Length + (int)(_pooledAmount*0.2f) + 1); // increment 20%
			}
		}
		return null;
	}

	public IEnumerator waitingForCompletition() {

		while(!_loaded) yield return new WaitForEndOfFrame();
	}

	IEnumerator __resize(int size) {

		int oldSize = _pooledList.Length;
		//float t = Time.time;

		if(_pooledList.Length != 0) {

			Debug.Log("Increment pool " + _tag + " to " + size.ToString());
		}

		PoolElement[] newArr = new PoolElement[size];
		System.Array.Copy(_pooledList, newArr, _pooledList.Length);
		_pooledList = newArr;

		int index = oldSize;

		#if ENABLE_RESIZE_ASYNC
		for(int i = oldSize; i < size; i+=j) {

			for(int k = 0; k < j; k++, index++) {

				if(index < size) {

					GameObject o = GameObject.Instantiate(_prefabsObject) as GameObject;
					o.transform.parent = _parent;
					if(_parent == null) {
						
						if(_poolingContainer == null) {
							_poolingContainer = GameObject.FindWithTag("PoolingContainer");
						}
						
						if(_poolingContainer != null) {
							o.transform.parent = _poolingContainer.transform;
						}
					}
					_pooledList[index] = o;
				}
			}

			yield return new WaitForEndOfFrame();
		}
		#else
		for(int i = oldSize; i < size; i++) {
			_prefabsObject.SetActive(false);
			GameObject o = GameObject.Instantiate(_prefabsObject) as GameObject;
			o.transform.parent = _parent;
			if(_parent == null) {

				if(_poolingContainer == null) {
					_poolingContainer = GameObject.FindWithTag("PoolingContainer");
				}

				if(_poolingContainer != null) {
					o.transform.parent = _poolingContainer.transform;
				}
			}
			_pooledList[index] = new PoolElement();
			_pooledList[index]._reserved = false;
			_pooledList[index++]._element = o;
		}
		yield return new WaitForEndOfFrame();

		#endif
			


		//float dt = Time.time - t;

		//DebugConsole.Log("Resize pool manager " + _tag + " from " + oldSize.ToString() + " to " + newSize.ToString() + " in " + dt.ToString() + "s");

		_loaded = true;
	}

	public void Free() {

		foreach(PoolElement o in _pooledList) {

			Util_PoolLoading.Instance.DestroyGameObject(o._element);
		}

		_pooledList = new PoolElement[0];
	}

	public void Resize(int size) {

		int oldSize = _pooledList.Length;
		int newSize = size;

		PoolElement[] newArr = new PoolElement[size];

		try {

			System.Array.Copy(_pooledList, newArr, _pooledList.Length);
		}
		catch(System.ArgumentException e) {

			Debug.Log(e.Message);

			for(int i = 0; i < _pooledList.Length; ++i) {

				newArr[i] = _pooledList[i];
			}
		}

		_pooledAmount = newSize;

		_pooledList = newArr;

		for(int i = oldSize; i < size; i++) {

			GameObject o = GameObject.Instantiate(_prefabsObject) as GameObject;
			o.transform.parent = _parent;
			if(_parent == null) {

				if(_poolingContainer == null) {
					_poolingContainer = GameObject.FindWithTag("PoolingContainer");
				}

				if(_poolingContainer != null) {
					o.transform.parent = _poolingContainer.transform;
				}
			}
			_pooledList[i] = new PoolElement();
			_pooledList[i]._element = o;
			_pooledList[i]._reserved = false;
		}

		Debug.Log("Resize pool manager " + _tag + " from " + oldSize.ToString() + " to " + newSize.ToString());

		_loaded = true;
	}

	/// <summary>
	/// Create the specified definition and _async.
	/// </summary>
	/// <param name="definition">Definition.</param>
	/// <param name="_async">Object to attach async function</param>
	public void Create(PoolingDefinition definition, GameObject _async = null) {

		if(_pooledList.Length >= definition._amount)
			return; // ne ho già abbastanza qui dentro

		if(_noPooling) {

			_tag = definition._object.name;
			_prefabsObject = definition._object;
			_pooledAmount = definition._amount;

			_loaded = true;
		}
		else {

			_tag = definition._object.name;
			_prefabsObject = definition._object;
			_pooledAmount = definition._amount;

			if(_async) {

				_async.GetComponent<MonoBehaviour>().StartCoroutine(__resize(_pooledAmount));
			}
			else {
				
				Resize(_pooledAmount);
			}
		}
	}
}
