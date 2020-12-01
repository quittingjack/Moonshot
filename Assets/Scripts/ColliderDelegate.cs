using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDelegate : MonoBehaviour
{
    public delegate void EventHandler(Collider2D collision);

    public EventHandler EventOnTriggerEnter2D;
    public EventHandler EventOnTriggerExit2D;
    public EventHandler EventOnTriggerStay2D;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        EventOnTriggerEnter2D?.Invoke(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        EventOnTriggerExit2D?.Invoke(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        EventOnTriggerStay2D?.Invoke(collision);
    }
}
