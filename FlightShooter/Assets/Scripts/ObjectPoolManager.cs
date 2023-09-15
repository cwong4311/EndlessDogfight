using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolObjectInfo
{
    public string objectName;
    public List<GameObject> availableObjects;
}

public class ObjectPoolManager : MonoBehaviour
{
    private static List<PoolObjectInfo> _objectsPools = new List<PoolObjectInfo>();

    public static GameObject Spawn(GameObject objToSpawn, Vector3 worldPosition, Quaternion worldRotation)
    {
        var pool = _objectsPools.Find(e => e.objectName == objToSpawn.name);
        if (pool == null)
        {
            pool = new PoolObjectInfo() { objectName = objToSpawn.name, availableObjects = new List<GameObject>() };
            _objectsPools.Add(pool);
        }

        GameObject spawned = pool.availableObjects.FirstOrDefault();
        if (spawned == null)
        {
            spawned = Instantiate(objToSpawn, worldPosition, worldRotation);
        }
        else
        {
            spawned.transform.position = worldPosition;
            spawned.transform.rotation = worldRotation;
            pool.availableObjects.Remove(spawned);

            spawned.SetActive(true);
        }

        return spawned;
    }

    public static void ReturnToPool(GameObject objToReturn)
    {
        var objName = objToReturn.name.Replace("(Clone)", "").Trim();
        var pool = _objectsPools.Find(e => e.objectName == objName);
        if (pool != null)
        {
            objToReturn.SetActive(false);
            pool.availableObjects.Add(objToReturn);
        }
    }

    public static bool IsPooledObject(GameObject objToReturn)
    {
        var objName = objToReturn.name.Replace("(Clone)", "").Trim();
        var pool = _objectsPools.Find(e => e.objectName == objName);

        return pool != null;
    }
}
