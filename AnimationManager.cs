using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {
    public static AnimationManager _instance;
    //public delegate void AnimationHandle();
   // public AnimationHandle animationState;

    public AnimationSkill animationskillsetup;
    [HideInInspector]
    public bool checkAttack;
    private SkillManager _skillmanager;
    private AdvanceMovement controller;
    private Animation _anim;

    public class AnimationSkill
    {
        public string skillType;
        public int skillIndex;
        public AnimationClip animationSkill;
        public float speedAnimation;
        public float activeTimer;
        public bool speedTuning;

    }
    void Awake() {
        _instance = this;
        
    }

   
    void Start() {
        animationskillsetup = new AnimationSkill();
        _anim = this.GetComponent<Animation>();
        _skillmanager = this.GetComponent<SkillManager>();
        controller = this.GetComponent<AdvanceMovement>();
    }

    public void CastSkill() {
        _anim.Play(_skillmanager.CastAnimation.name);

    }
    public void ActiveSkill()
    {

       _anim.Play(animationskillsetup.animationSkill.name);

        //if (skillSetup.speedTuning)  //Enable Speed Tuning
        //{
        //    GetComponent<Animation>()[skillSetup.animationSkill.name].speed = (playerStatus.statusCal.atkSpd / 100f) / skillSetup.speedAnimation;
        //}
        //else
        //{
        //    GetComponent<Animation>()[skillSetup.animationSkill.name].speed = skillSetup.speedAnimation;
        //}

        //Calculate Attack
       if (_anim[animationskillsetup.animationSkill.name].normalizedTime > animationskillsetup.activeTimer && !checkAttack)
        {
            _skillmanager.Activeskill(animationskillsetup.skillType, animationskillsetup.skillIndex);
            checkAttack = true;
        }

       if (_anim[animationskillsetup.animationSkill.name].normalizedTime > 0.9f)
        {
         
            controller._state = AdvanceMovement._State.Run;           
            controller.isUseSkill = false;
            _skillmanager.deployingSkill = false;
            
        }
    }


}
