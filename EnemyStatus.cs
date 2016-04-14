using UnityEngine;
using System.Collections;

public class EnemyStatus : MonoBehaviour{

    public string _enemyName;//怪物名称
    public int _enemyID;//怪物ID

    [System.Serializable]

    public class EnemyAttribute {
        public int Lv, Hp, Mp, Atk, Def, Spd;//等级，生命，法力，攻击，防御，速度（影响攻击速度和移动速度）
        public float HitRate, CriticalRate, MissRate, AtkSpd, AtkRange, Movespd;//命中率，暴击率，闪避率，攻击速度，攻击范围，移动速度
    
    }

    public EnemyAttribute _enemyStatus;
    public float _expGive;


	
}
