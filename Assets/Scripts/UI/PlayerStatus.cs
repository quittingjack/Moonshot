using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public FightStatus status;

    public Text text;

    private void Update()
    {
        text.text = string.Format("HP: {0}/{1}", status.getHP(), status.maxHP);
    }
}
