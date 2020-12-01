using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoonStatus : MonoBehaviour
{
    public FightStatus status;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float hpRatio = (float)status.getHP() * 100 / status.maxHP;

        text.text = string.Format("{0}%", (int)(hpRatio));

        if (status.getHP() <= 0)
        {
            text.color = Color.red;
        }
    }
}
