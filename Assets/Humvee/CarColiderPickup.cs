using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarColiderPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter (Collider col) 
    {
        var healthPickup = col.GetComponent<HealthPickup>();
        //Debug.Log("CarColiderPickup OnTriggerEnter " + col.GetType() + " is health : " + healthPickup != null);

        if (healthPickup) {
            CarManager carMgr = GetComponentInParent<CarManager>();
            healthPickup.onPicked(carMgr);
        }

    }
}
