using UnityEngine;
using System.Collections;

public class HpBarManager : MonoBehaviour {
    public static HpBarManager _instance;
    public GameObject HpBar;

    void Awake() {
        _instance = this;
    }

    

    public GameObject GetHpBar(GameObject target) {
        GameObject go = NGUITools.AddChild(transform.gameObject,HpBar);
        go.GetComponent<HpBar>().Target = target.transform;
        return go;  
    }

	
}
