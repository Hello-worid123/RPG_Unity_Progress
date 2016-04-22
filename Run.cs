using UnityEngine;
using System.Collections;

public class Run : MonoBehaviour
{
    private AdvanceMovement _advancemovement;
    private CharacterController boss1characterController;
    private CollisionFlags _collisonflags;
    private SkillParameters thisskillparameters;
    

    public float SkillTime;
    private float SkillTimer;
    public float Speed;
    private Vector3 movedir;

    private float _speed;

    // Use this for initialization
    void Awake()
    {
        boss1characterController = GameObject.FindGameObjectWithTag("Boss1").GetComponent<CharacterController>();
        thisskillparameters = this.GetComponent<SkillParameters>();

        _speed = Speed;

    }



    // Update is called once per frame
    void Update()
    {
        if (boss1characterController.isGrounded)
        {
            movedir = Vector3.zero;
            movedir = transform.TransformDirection(_speed * Vector3.forward * Time.deltaTime);

        }
        else
        {
            if ((_collisonflags & CollisionFlags.CollidedBelow) == 0)
            {
                movedir = Vector3.Lerp(movedir,Vector3.zero,20*Time.deltaTime);
              
            }
          

        }

        movedir.y -= 20 * Time.deltaTime;
        SkillTimer += Time.deltaTime;
       _collisonflags= boss1characterController.Move(movedir);

        if (SkillTimer > SkillTime)
            Destroy(this.gameObject);




    }

   

    void OnTriggerEnter(Collider other)
    {

       if(other.tag=="PlayerN"){
           
         _advancemovement= other.gameObject.GetComponent<AdvanceMovement>();

         _advancemovement.GetDamage(thisskillparameters._multiDamage,thisskillparameters._targetHitRate,thisskillparameters._flichRate,thisskillparameters._skillOwner);
       
       }
           
    }
 
}
