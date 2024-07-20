/*
* @Author: 教忠言
* @Description:
* @Date: 2024年04月24日 星期三 17:04:34
* @Modify:
*/
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect footStep;
    public VisualEffect attackVFX;
    public ParticleSystem beingHitVFX;
    public VisualEffect splashVFX;
    private List<VisualEffect> splashVFXList = new();

    public void BurstFootStep()
    {
        footStep.Play();
    }

    public void PlayAttackVFX()
    {
        attackVFX.Play();
    }

    public void PlayBeingHit(Vector3 attackPos)
    {
        Vector3 forceForward = transform.position - attackPos;
        forceForward.Normalize();
        forceForward.y = 0;
        beingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        beingHitVFX.Play();

        VisualEffect splashInstantiate = CreateSplash();
        splashInstantiate.Play();
        StartCoroutine(CountDownDestorySplash(10, splashInstantiate));
    }

    public VisualEffect CreateSplash()
    {
        foreach (var splash in splashVFXList)
        {
            if (!splash.gameObject.activeSelf)
            {
                splash.gameObject.SetActive(true);
                return splash;
            }
        }
        Vector3 splashPos = transform.position + new Vector3(0, 2f, 0);
        VisualEffect newSplashVFX = Instantiate(splashVFX, splashPos, quaternion.identity);
        splashVFXList.Add(newSplashVFX);
        return newSplashVFX;
    }

    IEnumerator CountDownDestorySplash(int time, VisualEffect splash)
    {
        yield return new WaitForSeconds(time);
        splash.gameObject.SetActive(false);
    }
}
