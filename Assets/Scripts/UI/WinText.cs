using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinText : MonoBehaviour
{
    public Text timeText;
    public void SetTime(float time)
    {
        timeText.text = string.Format("{0:F2}s", time);
    }
}
