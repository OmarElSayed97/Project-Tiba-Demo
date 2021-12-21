using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
	//Making sure no Object can override Awake (Only implementation insde Singleton)
	protected virtual void Awake()
	{
	}
}