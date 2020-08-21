using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarManager : MonoBehaviour
{
    public bool enterCarOption;
    public AudioClip destroySFXClip;
    public GameObject deathVFX;
    
    private GameObject gameHUD;
    private GameObject playerStanceIcon;
    private GameObject weaponHUDManager;
    private CrosshairManager crosshairManager;
    private PlayerHealthBar playerHealthBar;
    private GameObject gun;
    private GameObject muzzle;
    private Camera carCam;
    private Camera playerCam;
    private CarUserControl userCtrl;
    private bool inVehicle;
    private WeaponController weaponController;
    private GameObject player;
    private bool fireUp = false;
    public float health {get; private set; }

    
    // Start is called before the first frame update
    void Start()
    {   
        health = 50.0F;

        gameHUD = GameObject.Find("GameHUD");
        playerHealthBar = gameHUD.GetComponent<PlayerHealthBar>();
        playerStanceIcon = GameObject.Find("Stance");
        weaponHUDManager = GameObject.Find("WeaponHUDManager");
        crosshairManager = GameObject.FindObjectOfType<CrosshairManager>();

        gun = transform.Find("_MG_turret_portable/MGMain").gameObject;
        muzzle = gun.transform.Find("Muzzle").gameObject;
        carCam = transform.Find("Camera").gameObject.GetComponent<Camera>();
        userCtrl = gameObject.GetComponent<CarUserControl>();
        weaponController = gameObject.GetComponent<WeaponController>();
        weaponController.owner = this.gameObject;

        playerCam = GameObject.Find("Player/Main Camera").GetComponent<Camera>();

        if (enterCarOption) {
            userCtrl.enabled = false;
            carCam.enabled = false;
            carCam.GetComponent<AudioListener>().enabled = false;
            inVehicle = false;
        }
        else {
            inVehicle = true;        
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(inVehicle == true)
            {
                vehicleControl(null);
            }
        }        

        if (inVehicle) 
        {            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                userCtrl.enabled = !userCtrl.enabled;
                Debug.Log("Enter mode " + (isShotgunControlled() ? " shotgun" : " driver"));
            }

            if (isShotgunControlled()) 
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                Transform t = carCam.transform;
                Vector3 euler = t.eulerAngles;
                Quaternion quat = t.localRotation;
                float x = quat.x;
                float y = quat.y;
                float z = quat.z;

                float pitch = euler.x;
                float yaw = euler.y;
                float roll = euler.z;

                /*
                Debug.Log("Shotgun control | mouse x : " + mouseX + " y : " + mouseY + 
                " | quaternion x : " + x + " y : " + y + " z : " + z + 
                " | euler pitch : " + pitch + " yaw : " + yaw + " roll : " + roll  
                );
                */

                const float sensitivity = 4;
                
                if (mouseX != 0.0) {
                    t.RotateAround(gun.transform.position, -Vector3.up, -mouseX * sensitivity);
                    gun.transform.RotateAround(gun.transform.position, -Vector3.up, -mouseX * sensitivity);
                }
                
                float newRotationY = t.localEulerAngles.x + mouseY /* * sensitivity*/;
                newRotationY = Mathf.Clamp(newRotationY, 0, 20);                
                float rotateY = newRotationY - t.localEulerAngles.x;
                t.RotateAround(gun.transform.position, t.right, rotateY);

                // Muzzle pitch
                /*
                Cam : Muzzle
                0   : -3
                2.5 :  0
                10  :  8
                */
                float muzzleRotation;
                if (newRotationY < 2.5) {
                    muzzleRotation = linearInterpolate(newRotationY, 0, -3, 2.5F, 0);
                } 
                else {
                    muzzleRotation = linearInterpolate(newRotationY, 2.5F, 0, 10, 8);
                }

                float rotateYMuzzle = muzzleRotation - muzzle.transform.localEulerAngles.x;
                muzzle.transform.RotateAround(muzzle.transform.position, muzzle.transform.right, rotateYMuzzle);
                //Debug.Log("newRotationY camera : " + newRotationY + " muzzle : " + muzzleRotation);
            }

            if (Input.GetButtonDown("Fire")) {
                fireUp = true;
            }
            if (Input.GetButtonUp("Fire")) {
                fireUp = false;
            }

            if (fireUp) {
                weaponController.HandleShootInputs(false, true, false);
            }
        }

        //Debug.Log("Car position " + gun.transform.position + " player pos : " + GameObject.Find("Player").transform.position);
    }

    private float linearInterpolate(float x, float x1, float y1, float x2, float y2) 
    {
        return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
    }

    private bool isShotgunControlled()
    {
        return !userCtrl.enabled;
    }

    public void vehicleControl(GameObject playerObj)
    {
        if(inVehicle == false) {
            // Enter vehicle
            player = playerObj;
            //playerCam.enabled = false;
            carCam.enabled = true;
            carCam.GetComponent<AudioListener>().enabled = true;
            userCtrl.enabled = true;
            player.SetActive(false);
            player.transform.parent = gun.transform;      
            fireUp = false;            

            updateHUD(true);
            updateHealthBar();

            StartCoroutine(Time(true));
        }
        else {
            // Exit vehicle
            onExitVehicle();
            StartCoroutine(Time(false));
        }
    }

    private void onExitVehicle() 
    {
        player.SetActive(true);
        //playerCam.enabled = true;
        carCam.enabled = false;
        carCam.GetComponent<AudioListener>().enabled = false;
        userCtrl.enabled = false;
        player.transform.parent = null;
        player = null;   
        fireUp = false;           
        
        updateHUD(false);
    }
 
    private IEnumerator Time(bool inVehicle)
    {
        yield return new WaitForSeconds(0.5F);
        this.inVehicle = inVehicle;
    }

    public void inflictDamage(float damage)
    {
        health -= damage;
        updateHealthBar();
        //Debug.Log("Car health : " + health);

        if (health <= 0) {
            
            health = 0;

            if(inVehicle) {
                onExitVehicle();
            }

            Text playerTextMsg = GameObject.Find("Player/Canvas/Text").GetComponent<Text>();
            playerTextMsg.enabled = false;            

            // DEstroy FVX
            if (deathVFX) {
                var vfx = Instantiate(deathVFX, gun.transform.position, Quaternion.identity);
                Destroy(vfx, 5f);
            }

            // Destroy SFX
            if (destroySFXClip) {
                AudioUtility.CreateSFX(destroySFXClip, gun.transform.position, AudioUtility.AudioGroups.Impact, 1f, 3f);
            }

            Destroy(this.gameObject);
        }
    }

    public void heal()
    {
        health = 100;
        //Debug.Log("Car healing - health : " + health);
        updateHealthBar();
    }

    void updateHealthBar() 
    {
        playerHealthBar.updateValue(health / 100);
    }

    void updateHUD(bool vehicleMode) 
    {
        playerStanceIcon.SetActive(!vehicleMode);
        playerHealthBar.setManual(vehicleMode);
        weaponHUDManager.SetActive(!vehicleMode);        

        if (vehicleMode) {
            crosshairManager.OnWeaponChanged(weaponController);
        }
    }
}
