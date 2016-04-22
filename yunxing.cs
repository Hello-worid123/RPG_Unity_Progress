using UnityEngine;
using System.Collections;

public class yunxing : MonoBehaviour {
    public float gravity;
    public GameObject SkillhitGroundFx;
    public float LifeTimeHitGround;
    private SphereCollider sp;
    private bool ifdestroy;
    private Rigidbody child;

	// Use this for initialization
	void Start () {
        sp = this.GetComponent<SphereCollider>();
        child = transform.Find("Meteo").GetComponent<Rigidbody>();
        child.isKinematic = true;
        ifdestroy = true;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position -= transform.up * gravity*Time.deltaTime;
        
        PhysicCheck();
	}

    void PhysicCheck() {
        
        bool collider = Physics.Raycast(this.transform.position, Vector3.down,sp.radius-4f, LayerMask.GetMask("Ground"));
        if (collider) {
            Debug.Log("Ground");
            
            GameObject fx = (GameObject)GameObject.Instantiate(SkillhitGroundFx, this.transform.position, this.transform.rotation);
            gravity = Mathf.Lerp(gravity, 0, 0.15f);
            if (ifdestroy) {
                child.isKinematic = false;
                ifdestroy = false;
                Destroy(this.gameObject, LifeTimeHitGround);
            }
        }
    
    }
}
