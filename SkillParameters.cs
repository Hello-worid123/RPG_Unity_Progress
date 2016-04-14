using UnityEngine;
using System.Collections;

public class SkillParameters : MonoBehaviour {
    [HideInInspector]
    public float _multiDamage;
    [HideInInspector]
    public float _targetHitRate;
    [HideInInspector]
    public float _flichRate;
    [HideInInspector]
    public GameObject _skillOwner;

    public void RecieveSkillParameters(float multidamage,float targetHitRate,float flichRate,GameObject skillowner) {
        _multiDamage = multidamage;
        _targetHitRate = targetHitRate;
        _flichRate = flichRate;
        _skillOwner = skillowner;
    }
}
