using UnityEngine;
using System.Collections;

public class IceWave : MonoBehaviour {

    public GameObject Skill;
    public float Speed;
    public float Spawnrate;
    public float LifeTime = 1;

    private float timeTemp;
    private SkillParameters thisskillparameters;
    private AdvanceMovement _advancemovement;
    

    void Start()
    {
        GameObject.Destroy(this.gameObject, LifeTime);
        thisskillparameters = this.GetComponent<SkillParameters>();

        // Shaking a camera
        //ShakeCamera.Shake(0.2f, 0.7f);
    }

    void Update()
    {
        this.transform.position += this.transform.forward * Speed * Time.deltaTime;
        if (Time.time > timeTemp + Spawnrate)
        {
            if (Skill)
            {
                for (int i = 0; i < 3; i++)
                {
                    var skillSpawned = (GameObject)GameObject.Instantiate(Skill, this.transform.position+Vector3.left*4*(i-1), Quaternion.identity);
                  
                    // var skillbase = skillSpawned.GetComponent<SkillBase>();
                    //if (skillbase)
                    //{
                    //    skillbase.Owner = Owner;
                    //    skillbase.Damage = Damage;
                    //}
                }
            }
            timeTemp = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
    {


        if (other.tag == "PlayerN")
        {
            _advancemovement = other.gameObject.GetComponent<AdvanceMovement>();
            _advancemovement.GetDamage(thisskillparameters._multiDamage, thisskillparameters._targetHitRate, thisskillparameters._flichRate, thisskillparameters._skillOwner);

        }
    }


}
