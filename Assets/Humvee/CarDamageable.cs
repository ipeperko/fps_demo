using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDamageable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InflictDamage(float damage, bool isExplosionDamage, GameObject damageSource)
    {
        //Debug.Log("CAR Inflict Damage " + damage + " exlosion " + isExplosionDamage);
        var carMgr = GetComponentInParent<CarManager>();
        carMgr.inflictDamage(damage);
    }
}
