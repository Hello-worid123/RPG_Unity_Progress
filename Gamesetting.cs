using UnityEngine;
using System.Collections;
using System;

public class Gamesetting : MonoBehaviour {
    public static Gamesetting _instance;
    public GameObject playerprefab;
    public Transform respawnPos;
    public GameObject castCircleEffect;
    private const string PLAYERPREFABPATH = "PlayerPrefab/";

	// Use this for initialization

    void Awake() {
        _instance = this;
    }
	void Start () {
        GameObject go = GameObject.Instantiate(Resources.Load(PLAYERPREFABPATH + "Magican"))as GameObject;
        go.name = "PLAYER";
   //   GameObject Target = go.transform.Find("HpBarPos").gameObject;
     //  HpBarManager._instance.GetHpBar(Target);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void savaData() { 
        GameObject go=GameObject.Find("PLAYER");
        PlayerCharacter pc=go.GetComponent<PlayerCharacter>();
     PlayerPrefs.SetString("play name",go.name);
     for (int i = 0; i < Enum.GetValues(typeof(VitalName)).Length; i++) { 
         PlayerPrefs.SetInt(((VitalName)i).ToString(),pc.GetPrimaryAttribute(i).AdjustedBaseValue);
     }


         Debug.Log("Saved");
    }
    public void LoadData() { }
}
