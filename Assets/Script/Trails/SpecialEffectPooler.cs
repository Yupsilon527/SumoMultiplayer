using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectPooler : MonoBehaviour
{
    public static SpecialEffectPooler main;
    public GameObject TrailPrefab;
    private void Awake()
    {
        main = this;
    }
    public GameObject PoolEffect(string effectName)
    {
        GameObject LoadPrefab = Resources.Load<GameObject>("Prefabs/Special Effects/" + effectName);
        if (LoadPrefab == null)
            return null;
        return PoolEffect(LoadPrefab);
    }
        public GameObject PoolEffect(GameObject prefab)
        {
            for (int child = 0; child < transform.childCount; child++)
        {
            Transform childTransform = transform.GetChild(child);
            if (!childTransform.gameObject.activeSelf && childTransform.name == prefab.name)
            {
                return childTransform.gameObject;
            }
        }
        GameObject FX = GameObject.Instantiate(prefab);
        FX.name = prefab.name;
        FX.transform.SetParent(transform);
        FX.SetActive(false);
        return FX;
    }
    public void CreateSpecialEffect(string name, Vector3 pos)
    {
        GameObject newBug = PoolEffect(name);
        newBug.SetActive(true);
        newBug.transform.localPosition = pos;
    }
    public void CreateSpecialEffect(GameObject prefab, Vector3 pos)
    {
        if (prefab == null)
            return;
        GameObject newBug = PoolEffect(prefab);
        newBug.transform.localPosition = pos;
        newBug.SetActive(true);
    }
    public void CreateTrailOnGameObject(GameObject target, Color c,float comeintime, float trailduration)
    {
        GameObject trail = PoolEffect(TrailPrefab);
        if (trail.TryGetComponent<TrailController>(out TrailController trC))
        {
            trail.SetActive(true);
            trC.CopyEntity(target);
            trC.Expire(comeintime, trailduration);
            trC.ChangeColor(c);
        }
    }

}
