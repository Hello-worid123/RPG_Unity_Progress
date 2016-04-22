using UnityEngine;
using System.Collections;

public class energyBlast : MonoBehaviour {

    private AdvanceMovement _advancement;
    private SkillParameters _skillParameters;
    public float LifeTime;



	// Use this for initialization
	void Start () {
        _skillParameters = this.GetComponent<SkillParameters>();
        Destroy(this.gameObject, LifeTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "PlayerN") {
            _advancement = other.gameObject.GetComponent<AdvanceMovement>();
            _advancement.GetDamage(_skillParameters._multiDamage, _skillParameters._targetHitRate, _skillParameters._flichRate, _skillParameters._skillOwner) ;

        }
    }
}
