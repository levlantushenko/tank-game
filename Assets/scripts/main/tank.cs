using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class tank : MonoBehaviour
{
    #region variables
    
    [Header("movement")]
    public float speed;
    public bool isForceInverted;
    public float max;
    public float force;
    public float stopForce;
    public float rotSpeed;
    public float startRotSpeed;
    public WheelCollider[] right;
    public WheelCollider[] left;
    [Header("tower")]
    public Vector2 mouse;
    public Transform tower;
    public float HorSens;
    public float towerSpeed;
    public Transform cam;
    public Vector3 offset;
    public bool isLimited;
    public Vector2 Tlimit;
    [Header("gun")]
    public Transform gun;
    public float VerSens;
    public Vector2 limit;
    public Vector3 gunOffset;
    public float gunSpd;
    public Transform shootP;
    public GameObject bul;
    public float bulSpd;
    public float bulFall;
    public float rechT;
    public bool charged;
    public Text rechText;
    public float damage;
    public AudioSource shootSD;
    public float shootF;
    public GameObject shootEff;
    public float sensBake;
    [Header("other")]
    public GameObject expl;
    public Rigidbody rb;
    [Header("hp")]
    public float hp;
    public Text hpShow;
    public float cur;
    public RectTransform img1, img2;
    public float imgStartWidth;
    #endregion
    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        main = cam.GetChild(0).GetComponent<Camera>();
        if(PlayerPrefs.HasKey("sens"))
            sensSlider.value = PlayerPrefs.GetInt("sens");
        else
            sensSlider.value = 5;
    }
    void Update()
    {
        Rot();
        Move();
        Shoot();
        CameraAim();
        drawHP();
        Settings();
    }
    #region movement
    // drive methods : R - right, L - Left, F - Forward, B - Back
    void Rdrive()
    {
        foreach (WheelCollider wheel in left)
        {
            wheel.motorTorque = -rotSpeed;
        }
        foreach (WheelCollider wheel in right)
        {
            wheel.motorTorque = rotSpeed;
        }
    }
    void Ldrive()
    {
        foreach (WheelCollider wheel in left)
        {
            wheel.motorTorque = rotSpeed;
        }
        foreach (WheelCollider wheel in right)
        {
            wheel.motorTorque = -rotSpeed;
        }
    }
    void Bdrive()
    {
        foreach (WheelCollider wheel in left)
        {
            wheel.motorTorque = -speed;
        }
        foreach (WheelCollider wheel in right)
        {
            wheel.motorTorque = -speed;
        }
    }
    void Fdrive()
    {
        if(Mathf.Abs(Vector3.Dot(transform.forward, rb.velocity)) < max)
        {
            foreach (WheelCollider wheel in left)
            {
                wheel.motorTorque = speed;
            }
            foreach (WheelCollider wheel in right)
            {
                wheel.motorTorque = speed;
            }
        }
        else
        {
            foreach (WheelCollider wheel in left)
            {
                wheel.motorTorque = 0;
            }
            foreach (WheelCollider wheel in right)
            {
                wheel.motorTorque = 0;
            }
        }
    }

    float torque = 0;
    void Move()
    {
        // signing keyboard movement using "_Drive" voids
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -1f, 1f), rb.velocity.z);
        switch (isForceInverted)
        {
            case false:
                speed = force;
                rotSpeed = startRotSpeed;
                break;
            case true:
                speed = -force;
                rotSpeed = -startRotSpeed;
                break;
        }
        if(Input.GetKey(KeyCode.A))
            Ldrive();
        if (Input.GetKey(KeyCode.W))
            Fdrive();
        if (Input.GetKey(KeyCode.D))
            Rdrive();
        if (Input.GetKey(KeyCode.S))
            Bdrive();
        // stopping tank on Space press
        if (Input.GetKey(KeyCode.Space))
        {
            foreach (WheelCollider wheel in right)
            {
                rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, 0, stopForce), rb.velocity.y, Mathf.Lerp(rb.velocity.z, 0, stopForce));
                wheel.motorTorque = 0;
            }
            foreach (WheelCollider wheel in left)
            {
                rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, 0, stopForce), rb.velocity.y, Mathf.Lerp(rb.velocity.z, 0, stopForce));
                wheel.motorTorque = 0;
            }
        }
        // don't allow tank to fall
        Mathf.Clamp(transform.eulerAngles.z, -60, 60);
        Mathf.Clamp(transform.eulerAngles.x, -60, 60);
    }
    #endregion

    #region aim
    public Vector2 gunM;
    void Rot()
    {
        // getting mouse coordinates
        mouse = new Vector2(mouse.x + Input.GetAxis("Mouse X"), mouse.y + Input.GetAxis("Mouse Y"));
        // rotate tower
        if(isLimited)
            tower.localEulerAngles = new Vector3(offset.x, Mathf.Clamp(mouse.x * HorSens + offset.y, Tlimit.x, Tlimit.y), offset.z);
        else
            tower.localEulerAngles = new Vector3(offset.x, mouse.x * HorSens + offset.y, offset.z);

        //if in aim mode - rotate gun
        if (isAim)
            gunM = new Vector2(mouse.x / 60 * aimCam.GetComponent<Camera>().fieldOfView, Mathf.Clamp(gunM.y + Input.GetAxis("Mouse Y") / 30 * aimCam.GetComponent<Camera>().fieldOfView * HorSens * sensBake, limit.x, limit.y));
        gun.localEulerAngles = new Vector3(gunOffset.x, gunOffset.y, gunM.y + gunOffset.z);
    }
    public float explForce;
    float time;
    void Shoot()
    {
        // showing if the gun is ready to shoot to player
        if (!charged)
        {
            rechText.text = (Mathf.Round((time + rechT - (float)Time.timeAsDouble) * 10) / 10).ToString();
        }
        else
        {
            rechText.text = "ready";
        }
        // shooting if LMB pressed & gun is charged
        if (Input.GetMouseButtonDown(0) && charged)
        {
            bullet bullet = Instantiate(bul, shootP.position, shootP.rotation).GetComponent<bullet>();
            GameObject eff = Instantiate(shootEff, shootP.position, shootP.rotation);
            Destroy(eff, 3);
            bullet.speed = bulSpd;
            bullet.tankExplosion = expl;
            bullet.damage = damage;
            time = (float)Time.timeAsDouble;
            shootSD.Play();
            shootSD.time = 0.1f;
            charged = false;
            Invoke("Recharge", rechT);
            rb.AddForceAtPosition(-shootP.forward * shootF, tower.position, ForceMode.Impulse);
        }
    }
    void Recharge()
    {
        charged = true;
    }
    [Header("Camera")]
    public float fovSens;
    public float aimFovSens;
    public Vector2 fovLimit;
    public bool isAim;
    public GameObject aimCam;
    public GameObject Cam;
    Camera main;
    void CameraAim()
    {
        //changing camera rotation & FOV for comfortable shooting
        if(!isOpen)
            mouse = (new Vector2(mouse.x + Input.GetAxis("Mouse X"), mouse.y + Input.GetAxis("Mouse Y")));
        if(isLimited)
            mouse.x = Mathf.Clamp(mouse.x, Tlimit.x, Tlimit.y);
        mouse.y = Mathf.Clamp(mouse.y, limit.x, limit.y);
        cam.localEulerAngles = new Vector3(cam.localEulerAngles.x, mouse.x * HorSens + offset.y, cam.localEulerAngles.z);
        if(!isAim)
            main.fieldOfView += -Input.GetAxis("Mouse Wheel") * fovSens;
        else
            aimCam.GetComponent<Camera>().fieldOfView += -Input.GetAxis("Mouse Wheel") * aimFovSens;
        cam.localScale += new Vector3(-Input.GetAxis("Mouse Wheel") * fovSens, -Input.GetAxis("Mouse Wheel") * fovSens, -Input.GetAxis("Mouse Wheel") * fovSens);
        cam.localScale = new Vector3(Mathf.Clamp(cam.localScale.x, fovLimit.x, fovLimit.y), cam.localScale.x, cam.localScale.x);
        //switching aim modes
        if (isAim)
        {
            Cam.SetActive(false);
            aimCam.SetActive(true);
            if(Input.GetKeyDown(KeyCode.LeftShift))
                isAim = false;
        }
        else
        {
            aimCam.SetActive(false);
            Cam.SetActive(true);
            if (Input.GetKeyDown(KeyCode.LeftShift))
                isAim = true;
        }
    }
    #endregion
    void drawHP()
    {
        //writing hp in the text
        hpShow.text = cur + "/" + hp;
        // drawing fill boxes
        float percent = cur / hp * 100;
        img1.sizeDelta = new Vector2(imgStartWidth * percent / 100, img1.sizeDelta.y);
        img2.sizeDelta = new Vector2(imgStartWidth - img1.sizeDelta.x, img1.sizeDelta.y);
    }
    [Header("Settings")]
    public Transform canv;
    public bool isOpen = false;
    public Text sensCur;
    public Slider sensSlider;
    void Settings()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canv.gameObject.SetActive(true);
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            isOpen = true;
        }   
        sensCur.text = "чувствительность : " + sensSlider.value.ToString();
        HorSens = sensSlider.value;
        VerSens = sensSlider.value * sensBake;
        PlayerPrefs.SetInt("sens", (int)sensSlider.value);
        PlayerPrefs.Save();
    }
    public void HideCursor()
    {
        isOpen = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }
}
