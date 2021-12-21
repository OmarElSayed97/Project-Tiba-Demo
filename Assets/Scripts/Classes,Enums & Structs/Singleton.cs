using UnityEngine;

public interface ISingelton
{
	bool IsSingletonLogsEnabled { get; set; }
}

public class Singleton<T> : SingletonBase<Singleton<T>>, ISingelton where T : MonoBehaviour, ISingelton
{
	private static T s_Instance;

	/*[PropertyOrder(-20)]*/ public bool DontDestroyLoad;

	// [PropertyOrder(-19), ShowInInspector]
	public bool IsSingletonLogsEnabled
	{
		get => m_IsSingletonLogsEnabled;
		set => m_IsSingletonLogsEnabled = value;
	}

	[SerializeField, HideInInspector] private bool m_IsSingletonLogsEnabled = false;

	// [Title("Derived Fields Below:"), PropertyOrder(-18), ShowInInspector, HideLabel, DisplayAsString,
	//  VerticalGroup("1", PaddingBottom = -20)]
	private string emptyTitle = string.Empty;

	protected bool m_IsDestroyed = false;

	private static object _lock = new object();

	protected static bool applicationIsQuittingFlag = false;
	protected static bool applicationIsQuitting = false;

	public static bool IsInstanceNull
	{
		get { return s_Instance == null; }
	}

	public static T Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = (T)FindObjectOfType(typeof(T));


				//if (FindObjectsOfType(typeof(T)).Length > 1)
				//{
				//    Debug.LogError("[Singleton] multiple instances of '" + typeof(T) + "' found!");
				//}

				if (s_Instance == null)
				{
					if (applicationIsQuitting && Application.isPlaying)
					{
						Debug.LogWarning("[Singleton] IsQuitting Instance '" + typeof(T) + "' is null, returning.");
						return s_Instance;
					}
					else
					{
						//create a new gameObject, if Instance isn't found
						GameObject singleton = new GameObject();
						s_Instance = singleton.AddComponent<T>();
						singleton.name = "[Singleton] " + typeof(T).ToString();
						Debug.Log("[Singleton] An instance of '" + typeof(T) + "' was created: " + singleton);
					}
				}
				else
				{
					if (s_Instance.IsSingletonLogsEnabled)
					{
						Debug.Log("[Singleton] Found instance of '" + typeof(T) + "': " + s_Instance.gameObject.name);
					}
				}
			}

			return s_Instance;
		}
	}

	protected sealed override void Awake()
	{
		if (s_Instance == null)
		{
			s_Instance = gameObject.GetComponent<T>();
			if (DontDestroyLoad)
			{
				setDontDestroyOnLoad();
			}

			OnAwakeEvent();
		}
		else
		{
			if (this == s_Instance)
			{
				if (DontDestroyLoad)
				{
					setDontDestroyOnLoad();
				}

				OnAwakeEvent();
			}
			else
			{
				m_IsDestroyed = true;
				Destroy(this.gameObject);
			}
		}
	}

	protected virtual void OnAwakeEvent()
	{
	}

	// This is added to indicate for derived class that they should override
	// Since implementing a Start() function on any of the derived will hide base Start() functions - dangerous
	public virtual void Start()
	{
	}

	public virtual void OnDisable()
	{
		//Debug.LogError("SINGLETON ON DISABLE " + this.name);
#if UNITY_EDITOR
		applicationIsQuitting = applicationIsQuittingFlag;
#endif
	}

	public virtual void OnDestroy()
	{
	}

	protected void setDontDestroyOnLoad()
	{
		DontDestroyLoad = true;
		if (DontDestroyLoad)
		{
			if (transform.parent != null)
			{
				transform.parent = null;
			}

			DontDestroyOnLoad(gameObject);
		}
	}

	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed,
	/// it will create a buggy ghost object that will stay on the Editor scene
	/// even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
#if UNITY_EDITOR
	public virtual void OnApplicationQuit()
	{
		//Debug.LogError("SINGLETON APP QUIT " + this.name);
		applicationIsQuittingFlag = true;
	}
#endif
}