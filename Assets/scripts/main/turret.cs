using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour
{
    public float rech;
    public float damage;
    public float maxHp;
    public float currentHp;
    public Transform enemy;
    public List<Transform> enemies;
    public float spotDist;
    public Transform gun;
    public Transform shootP;
    public GameObject bullet;
    public float bulSpeed;
    public float cooldown;
    bool isCharged = true;
    public bool destroyed = false;
    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.position) < spotDist)
        {
            gun.LookAt(enemy.position);
            if(Physics.Raycast(shootP.position, shootP.forward, out RaycastHit hit, spotDist))
            {
                if (hit.collider.gameObject.tag == "tank")
                    Shoot();
            }
        }
        if(currentHp <= 0 && !destroyed)
        {
            GetComponent<Animator>().SetTrigger("disappear");
            destroyed = true;
        }
    }
    void Shoot()
    {
        if (!isCharged) return;
        bullet bul = Instantiate(bullet, shootP.position, shootP.rotation).GetComponent<bullet>();
        bul.speed = bulSpeed;
        bul.damage = damage;
        isCharged = false;
        Invoke("Rech", cooldown);
    }
    void Rech()
    {
        isCharged = true;
    }
    public void getEnemy()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            //finding parent with enemy component
            Transform curParent = enemies[i];
            while (!curParent.TryGetComponent<tank_AI>(out tank_AI a))
                curParent = curParent.parent;
            //removing enemy from list if his hp <= 0
            if (curParent.TryGetComponent<tank>(out tank tank))
            {
                if (tank.cur <= 0)
                    enemies.Remove(enemies[i]);
            }
            else if (curParent.TryGetComponent<tank_AI>(out tank_AI ai))
            {
                if (ai.currentHp <= 0)
                    enemies.Remove(enemies[i]);
            }
        }
        // finding nearest enemy
        float closestDist = Mathf.Infinity;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].position, gun.position) < closestDist)
            {
                closestDist = Vector3.Distance(enemies[i].position, gun.position);
                enemy = enemies[i];
            }
        }
    }
}
