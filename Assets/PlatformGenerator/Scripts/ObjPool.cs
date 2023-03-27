using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class ObjPool : MonoBehaviour
{
    [SerializeField] bool canGrow = false;

    [SerializeField] private List<_Pool> _poolsX;

    [SerializeField] private Dictionary<string, List<GameObject>> _poolDictionary = new Dictionary<string, List<GameObject>>();

    GameObject objParent;
    protected  void Awake()
    {
        InitPool();
    }
    private void InitPool()
    {
        objParent = new GameObject("All_PRIFABS");
        objParent.transform.SetParent(this.transform);
        foreach (_Pool pol in _poolsX)
        {
            List<GameObject> poolsList = new List<GameObject>();

            for (int i = 0; i < pol.size; i++)
            {
                GameObject pooledPrifab = Instantiate(pol.prifab);
                pooledPrifab.transform.SetParent(objParent.transform);
                pooledPrifab.SetActive(false);
                poolsList.Add(pooledPrifab);
            }
            _poolDictionary.Add(pol.tag, poolsList);
        }
    }
    public GameObject GetAvailablePrifabFromDictionary(string tag)
    {
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("plool with tag " + tag + "not found");

            return null;
        }
        for (int i = 0; i < _poolDictionary[tag].Count; i++)
        {
            if (_poolDictionary[tag][i].activeInHierarchy == false) return _poolDictionary[tag][i];
        }

        if (canGrow)
        {
                List<GameObject> poolsList = new List<GameObject>();

                GameObject pooledPrifab = Instantiate(_poolsX[0].prifab);
                pooledPrifab.transform.SetParent(objParent.transform);
                pooledPrifab.SetActive(false);
                poolsList.Add(pooledPrifab);

                _poolDictionary.Add(tag, poolsList);

                return pooledPrifab;
        }
        //else if (!canGrow)
        //{
        //    for (int i = 0; i < _poolDictionary[tag].Count; i++)
        //    {
        //        if (!_poolDictionary[tag][i].GetComponent<Platform>().isLast && !_poolDictionary[tag][i].GetComponent<Platform>().isActive && !_poolDictionary[tag][i].GetComponent<Platform>().isFirst)
        //        {
        //            _poolDictionary[tag][i].gameObject.SetActive(false);
        //            _poolDictionary[tag][i].gameObject.transform.position = Vector3.zero;
        //            Debug.Log("clinULp");
        //            return _poolDictionary[tag][i];
        //        }

        //    }
            
        //    return GetAvailablePrifabFromDictionary(tag);
        //}




        else
        {
            return null;
        }
    }
    public void ResetAll(string tag)
    {
        for (int i = 0; i < _poolDictionary[tag].Count; i++)
        {
            _poolDictionary[tag][i].gameObject.SetActive(false);
            _poolDictionary[tag][i].gameObject.transform.position = Vector3.zero;
        }
    }
}
[System.Serializable]
public class _Pool
{
    public string tag;
    //public AssetReferenceGameObject prifab;
    public GameObject prifab;
    public int size;

}