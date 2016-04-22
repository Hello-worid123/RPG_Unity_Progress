using UnityEngine;
using System.Collections;

public class DetectArea : MonoBehaviour {
    [HideInInspector]
    public EnemyAIController enemyAIcontroller;
    public float Radius;
    private CharacterController enemycharactercontroller;

    public bool drawGizmos;

    // Use this for initialization
    void Start()
    {

        enemyAIcontroller = transform.parent.GetComponent<EnemyAIController>();
        enemycharactercontroller = transform.parent.GetComponent<CharacterController>();
        SphereCollider collider = this.GetComponent<SphereCollider>();
        collider.center = enemycharactercontroller.center;
        collider.radius = Radius;
        collider.isTrigger = true;
    }



    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PlayerN")
        {
            enemyAIcontroller.chaseTarget = true;
            enemyAIcontroller.targetPlayer = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "PlayerN")
        {
        
            enemyAIcontroller.chaseTarget = false;
            enemyAIcontroller.targetPlayer = null;
        }
    }

    //Draw Gizmoz
    [ExecuteInEditMode]
    void OnDrawGizmosSelected()
    {
        if (drawGizmos)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, this.GetComponent<SphereCollider>().radius);
        }
    }





}
