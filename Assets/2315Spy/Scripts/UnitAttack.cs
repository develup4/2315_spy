using UnityEngine;
using System.Collections;

public class UnitAttack : MonoBehaviour
{
    private string targetType;
    private string type;
    private GameObject targetUnit;
    private float smooth = 0.1f;
    private float currentTime = 0.0f;

    public BulletFire m_bulletPrefeb; // 원거리 공격 총알

    void Start()
    {
    }
    void Update()
    {

        string state = gameObject.GetComponent<Unit>().getState();
        string attackstate = gameObject.GetComponent<Unit>().getAttackState();

        if (state.Equals("ATTACK"))
        {

            targetUnit = gameObject.GetComponent<UnitMove>().getTargetUnit();

            if (targetUnit != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetUnit.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.time * smooth);
                currentTime += Time.deltaTime;

                targetType = targetUnit.GetComponent<Unit>().m_type;
                type = gameObject.GetComponent<Unit>().m_type;

                // 근거리 공격일때
                if (attackstate.Equals("MELEE_ATTACK"))
                {
                    // meleeAttack 공격속도에 따른 데미지 적용
                    if (GetComponent<Unit>().m_meleeAttackSpeed < currentTime)
                    {
                        targetUnit.GetComponent<Unit>().m_health -=
                            (int)((gameObject.GetComponent<Unit>().m_meleePower - targetUnit.GetComponent<Unit>().m_armor) * meleeDamageRevise());

                        currentTime = 0.0f;
                    }
                }

                // 원거리 공격일때
                if (attackstate.Equals("RANGE_ATTACK"))
                {
                    // rangeAttack 공격속도에 따른 데미지 적용	
                    if (GetComponent<Unit>().m_rangeAttackSpeed < currentTime)
                    {
                        targetUnit.GetComponent<Unit>().m_health -=
                            (int)((gameObject.GetComponent<Unit>().m_rangePower - targetUnit.GetComponent<Unit>().m_armor) * rangeDamageRevise());

                        currentTime = 0.0f;
                    }

                    // 애니메이션 한 싸이클 마다 총알 발사
                    if (GetComponent<UnitMove>().getUnitAniamition().GetComponent<UnitAnimation>().skeleton.state.Time >
                        GetComponent<UnitMove>().getUnitAniamition().GetComponent<UnitAnimation>().skeleton.state.Animation.Duration)
                    {

                        BulletFire newBullet;

                        newBullet = Instantiate(m_bulletPrefeb, transform.position, transform.rotation) as BulletFire;
                        newBullet.m_shooter = gameObject;
                        newBullet.m_target = targetUnit;

                        gameObject.GetComponent<UnitMove>().getUnitAniamition().GetComponent<UnitAnimation>().skeleton.state.Time = 0.0f;
                    }

                }

                // 타겟의 체력이 다되면 타겟 사망
                if (targetUnit.GetComponent<Unit>().m_health <= 0)
                {
                    targetUnit.GetComponent<Unit>().death();
                    GetComponent<UnitMove>().setTargetUnit(null);
                }
            }
        }
    }

    private float meleeDamageRevise()
    {
        float ret = 1.0f;

        if (type.Equals("LightArmor") && targetType.Equals("LightArmor")) { ret = 1.0f; }
        else if (type.Equals("LightArmor") && targetType.Equals("HeavyArmor")) { ret = 1.0f; }
        else if (type.Equals("LightArmor") && targetType.Equals("Gigantic")) { ret = 1.0f; }

        else if (type.Equals("HeavyArmor") && targetType.Equals("LightArmor")) { ret = 1.5f; }
        else if (type.Equals("HeavyArmor") && targetType.Equals("HeavyArmor")) { ret = 1.0f; }
        else if (type.Equals("HeavyArmor") && targetType.Equals("Gigantic")) { ret = 1.0f; }

        else if (type.Equals("Gigantic") && targetType.Equals("LightArmor")) { ret = 1.0f; }
        else if (type.Equals("Gigantic") && targetType.Equals("HeavyArmor")) { ret = 1.0f; }
        else if (type.Equals("Gigantic") && targetType.Equals("Gigantic")) { ret = 1.0f; }

        return ret;
    }

    private float rangeDamageRevise()
    {
        float ret = 1.0f;

        if (type.Equals("LightArmor") && targetType.Equals("LightArmor")) { ret = 1.0f; }
        else if (type.Equals("LightArmor") && targetType.Equals("HeavyArmor")) { ret = 1.0f; }
        else if (type.Equals("LightArmor") && targetType.Equals("Gigantic")) { ret = 1.0f; }

        else if (type.Equals("HeavyArmor") && targetType.Equals("LightArmor")) { ret = 1.0f; }
        else if (type.Equals("HeavyArmor") && targetType.Equals("HeavyArmor")) { ret = 1.0f; }
        else if (type.Equals("HeavyArmor") && targetType.Equals("Gigantic")) { ret = 1.0f; }

        else if (type.Equals("Gigantic") && targetType.Equals("LightArmor")) { ret = 0.5f; }
        else if (type.Equals("Gigantic") && targetType.Equals("HeavyArmor"))
        {
            if (Random.Range(0, 99) < 10) { ret = 3.0f; }
            else { ret = 1.0f; }
        }
        else if (type.Equals("Gigantic") && targetType.Equals("Gigantic")) { ret = 1.0f; }

        return ret;
    }
}
