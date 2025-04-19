using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Base : MonoBehaviour
{
    public float timeToCapture;
    public float timeLeft;
    public float startCapturingTime;
    public bool isCapturing;
    public bool isCaptured;
    public enum forType
    {
        player,
        enemy
    }
    public forType type;
    private void Update()
    {
        if (isCapturing)
        {
            timeLeft = Time.time - timeToCapture - startCapturingTime;
        }if(timeLeft <= 0)
        {
            isCaptured = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<tank>(out tank t) && type == forType.player ||
            other.gameObject.TryGetComponent<tank_AI>(out tank_AI a) && type == forType.enemy)
        {
            isCapturing = true;
            startCapturingTime = Time.time;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<tank>(out tank t) && type == forType.player ||
            other.gameObject.TryGetComponent<tank_AI>(out tank_AI a) && type == forType.enemy)
        {
            isCapturing = false;
        }
    }
    public void Capture()
    {
        isCaptured = true;
    }
}
