using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorial : MonoBehaviour
{
    public static int step;
    public Animator turret;
    public GameObject paper;
    public GameObject audio;
    public enum tutorialTypes
    {
        system,
        moveCheck,
        target,
        block,
        finish
    }
    public tutorialTypes type;

    void Update()
    {
        if (type == tutorialTypes.system && Input.GetKeyDown(KeyCode.F))
        {
            paper.SetActive(false);
            audio.SetActive(false);
        }
        if (type == tutorialTypes.block)
        {
            if (step>=2)
                Destroy(gameObject);
        }
        if (step == 4 && type == tutorialTypes.system)
            turret.SetTrigger("appear");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(type == tutorialTypes.target && collision.gameObject.TryGetComponent<bullet>(out bullet bul))
        {
            step++;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "tank" && type == tutorialTypes.finish)
        {
            PlayerPrefs.SetInt("tutorial", 1);
            SceneManager.LoadScene("menu");
        }
        if (other.gameObject.tag == "tank" && type == tutorialTypes.moveCheck)
        {
            step++;
            Destroy(gameObject);
        }
    }
}
