using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class map : MonoBehaviour
{
    public GameObject mapObj;
    public string m1;
    public string m2;
    public string m3;
    public Transform[] buttons;
    public GameObject greeting;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (!PlayerPrefs.HasKey("tutorial"))
            SceneManager.LoadScene("tutorial");
        if(!PlayerPrefs.HasKey("city"))
            buttons[1].gameObject.SetActive(false); 
        if(!PlayerPrefs.HasKey("cliffs"))
            buttons[2].gameObject.SetActive(false);
        if(PlayerPrefs.HasKey("final"))
            greeting.SetActive(true);
    }
    public void Open()
    {
        mapObj.SetActive(true);
    }
    public void Close()
    {
        mapObj.SetActive(false);
    }
    public void M1()
    {
        SceneManager.LoadScene(m1);
    }
    public void M2()
    {
        SceneManager.LoadScene(m2);
    }
    public void M3()
    {
        SceneManager.LoadScene(m3);
    }
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("menu");
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
