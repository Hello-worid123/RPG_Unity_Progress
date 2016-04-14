using UnityEngine;
using System.Collections;

public class InventoryWindowUI : MonoBehaviour {

   private UIButton _InventoryWindowCloseBtn;

	// Use this for initialization

   void Awake() {
       _InventoryWindowCloseBtn = transform.Find("Background/Close Button").GetComponent<UIButton>();
   }
	void Start () {

        EventDelegate ed5 = new EventDelegate(this, "OnHideInventoryWindow");
        _InventoryWindowCloseBtn.onClick.Add(ed5);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnHideInventoryWindow()
    {
        this.gameObject.SetActive(false);
    }

}
