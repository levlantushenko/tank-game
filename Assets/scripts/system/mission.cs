using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mission : MonoBehaviour
{
    public tank_AI[] enemies;
    //public Base _base;
    public bool completed;
    public Animator end;
    public tank pl;
    public enum TaskEnum
    {
        kill,
        claim,
        protect
    }
    [SerializeField] private TaskEnum task;
    public string missionName;
    public string scene;

    void Update()
    {
        if(task == TaskEnum.kill)
        {
            for(int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].currentHp > 0)
                    break;
                if(i == enemies.Length-1 && !completed)
                {
                    Invoke("Win", 10f);
                    end.SetTrigger("win");
                    completed = true;
                }
            }
        }
        if (pl.cur <= 0)
        {
            end.SetTrigger("lose");
        }
        //if(task == TaskEnum.claim && _base.isCaptured == true)
        //{
        //    Win();
        //}
    }
    public void Win()
    {
        PlayerPrefs.SetInt(missionName, 1);
        PlayerPrefs.Save();
        Debug.Log("Mission complete!");
        SceneManager.LoadScene(scene);
    }
    public void Lose()
    {
        Debug.Log("Mission failled!");
        SceneManager.LoadScene(scene);
    }

}
