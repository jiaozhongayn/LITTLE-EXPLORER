/*
* @Author: 教忠言
* @Description:
* @Date: 2024年04月24日 星期三 15:04:20
* @Modify:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect footStep;
    public ParticleSystem Blade01;
    public VisualEffect Slash;

    public void Update_FootStep(bool state)
    {
        if (state)
            footStep.Play();
        else
            footStep.Stop();
    }

    public void PlayBlade01()
    {
        Blade01.Play();
    }

    public void PlaySlash(Vector3 pos)
    {
        Slash.transform.position = pos;
        Slash.Play();
    }
}
