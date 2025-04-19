using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed;
    public GameObject tankExplosion;
    public GameObject groundExplosion;
    public Material material;
    public float damage;
    
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "object")
        {
            GameObject explosion = Instantiate(groundExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 2);
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "tank")
        {
            GameObject explosion = Instantiate(tankExplosion, transform.position, Quaternion.identity);
            if (collision.gameObject.TryGetComponent<tank>(out tank pl))
                pl.cur -= Random.Range(-25, 25) + damage;
            if (collision.gameObject.TryGetComponent<tank_AI>(out tank_AI ai))
                ai.currentHp -= Random.Range(-25, 25) + damage;
            if(collision.gameObject.TryGetComponent<turret>(out turret tur)) 
                tur.currentHp -= Random.Range(-25, 25) + damage;
            Destroy(explosion, 2);
            Destroy(gameObject);
        }
    }
}
