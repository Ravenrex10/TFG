using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class xpManager : MonoBehaviour
{
    public Slider slider;

    public void setXP(int xp)
    {
        slider.value = xp;
    }

    public void setMaxXP(int xp)
    {
        slider.maxValue = xp;
    }

    public void gain(int xp)
    {
        setXP(Mathf.FloorToInt(slider.value) - xp);
    }
}
