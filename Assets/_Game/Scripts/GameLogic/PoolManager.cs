using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private Dictionary<string, Stack<GameObject>> dictObject = new Dictionary<string, Stack<GameObject>>();
    private Dictionary<string, List<GameObject>> dictObjectActive = new Dictionary<string, List<GameObject>>();
    public void Register(string objectName)
    {
        if (dictObject.ContainsKey(objectName)) return;

        dictObject[objectName] = new Stack<GameObject>();
    }

    public GameObject GetObject(string objectName)
    {
        if (!dictObject.ContainsKey(objectName)) return null;

        GameObject ob = null;
        //Get new from resource
        if (dictObject[objectName].Count < 1)
        {
            var gObject = GameObject.Instantiate(Resources.Load<GameObject>(objectName));
            if (gObject == null)
            {
                Debug.LogError($"Gameobject {objectName} not found in resource");
                return null;
            }

            ob = gObject;
        }

        if (ob == null)
            ob = dictObject[objectName].Pop();
        ob.SetActive(true);
        ob.transform.parent = null;

        if (!dictObjectActive.ContainsKey(objectName))
            dictObjectActive[objectName] = new List<GameObject>();
        dictObjectActive[objectName].Add(ob);

        return ob;
    }

    public void RetrieveObject(GameObject gameObject)
    {
        if (!dictObjectActive.ContainsKey(gameObject.name)) return;
        if (!dictObjectActive[gameObject.name].Contains(gameObject)) return;

        gameObject.transform.parent = this.transform;
        gameObject.SetActive(false);
        dictObjectActive[gameObject.name].Remove(gameObject);
    }

    public void RetrieveAll(string objectName)
    {
        if (!dictObjectActive.ContainsKey(objectName)) return;

        while (dictObjectActive[objectName].Count > 0)
        {
            var ob = dictObjectActive[objectName][0];
            ob.transform.parent = this.transform;
            ob.SetActive(false);
            dictObject[objectName].Push(ob);
            dictObjectActive[objectName].RemoveAt(0);
        }
    }
}
