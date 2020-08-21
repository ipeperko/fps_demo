using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorTrigger : MonoBehaviour
{
    private bool inTrigger;
    private GameObject player;
    private Text playerTextMsg;
    private CarManager carManager;

    // Start is called before the first frame update
    void Start()
    {
        playerTextMsg = GameObject.Find("Player/Canvas/Text").GetComponent<Text>();
        carManager = GameObject.Find("Humvee").GetComponent<CarManager>();
    }

    void Update()
    {
        if(inTrigger == true)
        {
            //Debug.Log("DoorTrigger : inTrigger");
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Debug.Log("DoorTrigger : Key E down");
                carManager.vehicleControl(player);
                inTrigger = false;
            }
        }
    }
 
    void OnTriggerEnter (Collider col) 
    {
        //Debug.Log("Colider object : " + col);

        // if (col.name == "Player") {
            inTrigger = true;
            player = col.gameObject;
            playerTextMsg.enabled = true;
            playerTextMsg.text = "Press E to enter/exit the vehicle\nPress Q to switch driver/shooter mode";
        // }
    }
    void OnTriggerExit()
    {
        inTrigger = false;
        player = null;
        playerTextMsg.enabled = false;
    }
}
