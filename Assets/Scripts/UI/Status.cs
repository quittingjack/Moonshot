using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public FightStatus status;
    public ForceControl control;

    public Color fullBulletColor;
    public Color emptyBulletColor;
    public Color fullHPColor;
    public Color emptyHPColor;

    public Image frontBullet;
    public Image backBullet;
    public Image LBullet;
    public Image RBullet;
    public Image mainBullet;
    public Image HP;

    public Text frontBulletText;
    public Text backBulletText;
    public Text LsideBulletText;
    public Text RsideBulletText;
    public Text mainBulletText;
    public Text HPText;

    private void Update()
    {
        float hpRatio = (float)status.getHP() * 100 / status.maxHP;

        HP.fillAmount = hpRatio / 100;
        HP.color = Color.Lerp(emptyHPColor, fullHPColor, hpRatio / 100);
        HPText.text = string.Format("{0}%", (int)(hpRatio));

        float backRatio = control.backGunHeat / control.backGunMaxHeat;

        backBullet.fillAmount = backRatio;
        backBullet.color = Color.Lerp(fullHPColor, emptyHPColor, backRatio);
        backBulletText.text = string.Format("{0}%", (int)(backRatio * 100));

        UpdateBullet(control.frontGunMag, control.frontGunSlot, emptyBulletColor, fullBulletColor, frontBullet, frontBulletText, control.frontGunReloadTime, control.lastFrontGunReload);
        UpdateBullet(control.leftGunMag, control.sideGunSlot, emptyBulletColor, fullBulletColor, LBullet, LsideBulletText, control.sideGunReloadTime, control.lastLeftGunReload);
        UpdateBullet(control.rightGunMag, control.sideGunSlot, emptyBulletColor, fullBulletColor, RBullet, RsideBulletText, control.sideGunReloadTime, control.lastRightGunReload);
        UpdateBullet(control.mainGunMag, control.mainGunSlot, emptyBulletColor, fullBulletColor, mainBullet, mainBulletText, control.mainGunReloadTime, control.lastMainGunReload);
        
    }

    void UpdateBullet(int curr, int max, Color minColor, Color maxColor, Image image, Text text, float reloadTime, float lastReload)
    {
        

        
        

        if (Time.time - lastReload < reloadTime)
        {
            float ratio = (Time.time - lastReload) / reloadTime;
            image.fillAmount = ratio;
            image.color = Color.Lerp(minColor, maxColor, ratio);
            text.text = "R";
        }
        else
        {
            float ratio = (float)curr / max;
            image.fillAmount = ratio;
            image.color = Color.Lerp(minColor, maxColor, ratio);
            text.text = string.Format("{0}", curr);
        }
    }
}
