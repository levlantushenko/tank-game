using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class tank_AI : MonoBehaviour
{
    #region variables
    [Header("movement")]
    public float speed;
    public float maxSpeed;
    public float force;
    public float rotationSpeed;
    public WheelCollider[] rightWheels;
    public WheelCollider[] leftWheels;
    [Header("tower")]
    public Vector2 mousePosition;
    public Transform tower;
    public float mouseSensitivity;
    public Vector3 towerRotationOffset;
    [Header("gun")]
    public Transform gun;
    public Vector3 gunRotationOffset;
    public Transform shootPosition;
    public GameObject bullet;
    public float bulSpeed;
    public float rechTime;
    public bool isCharged;
    public GameObject explosion;
    public Rigidbody rb;
    public float damage;
    public float spotDist;
    public AudioSource shootSound;
    public GameObject shootEffect;
    [Header("AI")]
    public Transform[] points;
    public int[] pointDirection;
    public List<Transform> enemies;
    public Transform enemy;
    public int currentEnemyInd;
    public bool isPathRound;
    [Header("hp")]
    public float hp;
    public Text hpShow;
    public float currentHp;
    Animator anim;

    #endregion
    
    void Update()
    {
        if (hp > 0)
        {
            Move();
            AIMove();
            AILook();
        }
        getEnemy();
        drawHP();
        hpCheck();
    }
    #region movement
    // drive methods : R - right, L - Left, F - Forward, B - Back
    void Rdrive()
    {

        foreach (WheelCollider wheel in rightWheels)
        {
            wheel.motorTorque = rotationSpeed;
            wheel.brakeTorque = 0;
        }
        foreach (WheelCollider wheel in leftWheels)
        {
            wheel.brakeTorque = 0; 
            wheel.motorTorque = -rotationSpeed;
        }
    }
    void Ldrive()
    {

        foreach (WheelCollider wheel in leftWheels)
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = rotationSpeed;
        }
        foreach (WheelCollider wheel in rightWheels)
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = -rotationSpeed;
        }
    }
    void Bdrive()
    {
        if (Mathf.Abs(Vector3.Dot(transform.forward, rb.velocity)) < maxSpeed)
        {
            foreach (WheelCollider wheel in leftWheels)
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = -speed;
            }
            foreach (WheelCollider wheel in rightWheels)
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = -speed;
            }
        }
        else
        {
            foreach (WheelCollider wheel in leftWheels)
            {
                wheel.motorTorque = 0;
            }
            foreach (WheelCollider wheel in rightWheels)
            {
                wheel.motorTorque = 0;
            }
        }
    }
    
    void Fdrive()
    {
        if (Mathf.Abs(Vector3.Dot(transform.forward, rb.velocity)) < maxSpeed)
        {
            foreach (WheelCollider wheel in leftWheels)
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = speed;
            }
            foreach (WheelCollider wheel in rightWheels)
            {
                wheel.brakeTorque = 0;
                wheel.motorTorque = speed;
            }
        }
        else
        {
            foreach (WheelCollider wheel in leftWheels)
            {
                wheel.motorTorque = 0;
            }
            foreach (WheelCollider wheel in rightWheels)
            {
                wheel.motorTorque = 0;
            }
        }
    }
    void Release()
    {
        foreach (WheelCollider wheel in leftWheels)
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = 0;
        }
        foreach (WheelCollider wheel in rightWheels)
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = 0;
        }
    }
    void Move()
    {
        speed = force;
    }
    #endregion


    #region shoot
    void Shoot()
    {
        if (isCharged)
        {
            // Creating bullet, signing it's variables
            bullet _bullet = Instantiate(bullet, shootPosition.position, shootPosition.rotation).GetComponent<bullet>();
            GameObject eff = Instantiate(shootEffect, shootPosition.position, shootPosition.rotation);
            Destroy(eff, 3);
            _bullet.speed = bulSpeed;
            _bullet.tankExplosion = explosion;
            _bullet.damage = damage;
            // causing recharge
            isCharged = false;
            shootSound.Play();
            shootSound.time = 0.1f;
            Invoke("Recharge", rechTime);
        }
    }
    void Recharge()
    {
        isCharged = true;
    }
    #endregion

    #region AI
    public int step = 0;
    public int curPoint;
    public float dist;
    public float AiOffset;
    Vector2 nextPos;
    void AIMove()
    {
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -0.5f, 0.5f), rb.velocity.z);
        Transform point = points[curPoint];
        Vector3 rot = new Vector3(0, transform.localEulerAngles.y - 180, 0);
        Vector3 pRot = new Vector3(0, point.localEulerAngles.y, 0);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        if (curPoint != points.Length-1)
            nextPos = new Vector2(points[curPoint + 1].position.x, transform.position.y);
        else
            nextPos = points[curPoint].position;
        if(curPoint == points.Length - 1 && isPathRound)
            nextPos = new Vector2(points[0].position.x, transform.position.y);
        //step 0
        if (step == 0)
        {
            switch (pointDirection[curPoint])
            {
                case 0:
                    Rdrive();
                    break;
                case 1:
                    Ldrive();
                    break;
            }
        }
        //step 1
        if (Mathf.Abs(rot.y - pRot.y + AiOffset) <= 2 && step == 0)
        {
            
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                point.transform.localEulerAngles.y, transform.localEulerAngles.z);
            step = 1;
        }
        
        if (step == 1)
        {
            if(curPoint >= points.Length - 1)
            {
                foreach (WheelCollider wheel in leftWheels)
                {
                    wheel.brakeTorque = 1;
                    wheel.motorTorque = 0;
                }
                foreach (WheelCollider wheel in rightWheels)
                {
                    wheel.brakeTorque = 1;
                    wheel.motorTorque = 0;
                }
            }else
                Fdrive();
        }
        if (Vector2.Distance(pos, nextPos) < dist)
        {
            step = 0;
            if(curPoint < points.Length-1)
                curPoint++;
        }
        
        
        //if(step == 1)
        //{
        //    foreach (WheelCollider wheel in leftWheels)
        //    {
        //        wheel.brakeTorque = 1;
        //        wheel.motorTorque = 0;
        //    }
        //    foreach (WheelCollider wheel in rightWheels)
        //    {
        //        wheel.brakeTorque = 1;
        //        wheel.motorTorque = 0;
        //    }
        //}
        
    }
    public bool ready;
    public Vector3 rot;
    void AILook()
    {
        tower.LookAt(enemy);
        tower.localEulerAngles = new Vector3(towerRotationOffset.x, towerRotationOffset.y + tower.localEulerAngles.y, towerRotationOffset.z);
        gun.LookAt(enemy);
        Vector3 gunEulerAngles = gun.localEulerAngles + gunRotationOffset;
        gun.localRotation = Quaternion.Euler(gunEulerAngles);
        if(Physics.Raycast(shootPosition.position, shootPosition.forward, out RaycastHit hit, spotDist))
        {
            Shoot();
        }
    }

    // recieving nearest enemy from list + removing died enemies
    public void getEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            //finding parent with enemy component
            Transform curParent = enemies[i];
            while(!curParent.TryGetComponent<tank_AI>(out tank_AI ai))
            {
                if (curParent.TryGetComponent<tank>(out tank t))
                    break;
                curParent = curParent.parent;
            }
            //removing enemy from list if his hp <= 0
            if(curParent.TryGetComponent<tank>(out tank tank))
            {
                if (tank.cur <= 0)
                    enemies.Remove(enemies[i]);
            }
            else if(curParent.TryGetComponent<tank_AI>(out tank_AI ai))
            {
                if (ai.currentHp <= 0)
                    enemies.Remove(enemies[i]);
            }
        }
        // finding nearest enemy
        float closestDist = Mathf.Infinity;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].position, tower.position) < closestDist)
            {
                closestDist = Vector3.Distance(enemies[i].position, tower.position);
                enemy = enemies[i];
            }
        }
    }
    #endregion
    public float imgStartWidth;
    public RectTransform img1, img2;
    void drawHP()
    {
        //writing hp in the text
        hpShow.text = currentHp + "|" + hp;
        // drawing fill boxes
        float percent = currentHp / hp * 100;
        img1.sizeDelta = new Vector2(imgStartWidth * percent / 100, img1.sizeDelta.y);
        img2.sizeDelta = new Vector2(imgStartWidth - img1.sizeDelta.x, img1.sizeDelta.y);
        img1.transform.parent.parent.LookAt(Camera.main.transform);
        currentHp = Mathf.Clamp(currentHp, 0, hp);
    }
    void hpCheck()
    {
        if(hp <= 0)
        {
            //tower.transform.parent = null;
            //tower.GetComponent<Rigidbody>().isKinematic = true;
            //tower.GetComponent<Rigidbody>().AddForce(transform.up * 1000, ForceMode.Impulse);
            GetComponent<tank_AI>().enabled = false;
        }
    }
    private void OnDrawGizmos()
    {
        for(int i = 0; i < points.Length; i++)
        {
            Gizmos.color = Color.green;
            if (i != points.Length-1)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(points[i].position, points[i+1].position);
            }
            if(i == points.Length - 1 && isPathRound)
            {
                Gizmos.DrawLine(points[i].position, points[0].position);
            }
            Gizmos.DrawWireSphere(points[i].position, dist);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(points[i].position, points[i].forward);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawRay(shootPosition.position, shootPosition.forward * spotDist);
    }
}
