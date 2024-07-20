/*
* @Author: 教忠言
* @Description:
* @Date: 2024年05月12日 星期日 18:05:18
* @Modify:
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    private Collider _damageCasterCollider;
    public int damage = 30;
    public string TargetTag;
    private List<Collider> _damageTargetList;

    private void Awake()
    {
        _damageCasterCollider = GetComponent<BoxCollider>();
        _damageCasterCollider.enabled = false;
        _damageTargetList = new List<Collider>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargetTag) && !_damageTargetList.Contains(other))
        {
            Character targetCC = other.GetComponent<Character>();
            if (targetCC != null)
            {
                PlayerVFXManager _playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();

                RaycastHit hit;
                //检测盒的绘制原点
                Vector3 originalPos = transform.position + ( -_damageCasterCollider.bounds.size.z) * transform.forward;
                //检测盒各个轴上的大小
                Vector3 halfExtents = _damageCasterCollider.bounds.size / 2;
                //检测盒的最大投射距离
                float maxDistance = _damageCasterCollider.bounds.size.z;
                bool isHit = Physics.BoxCast(originalPos, halfExtents, transform.forward, out hit, 
                    transform.rotation, maxDistance, 1 << 6);
                if (isHit)
                {
                    targetCC.ApplyDamage(damage, transform.parent.position);
                    if(TargetTag == "Enemy")
                        _playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                }
            }
            _damageTargetList.Add(other);
        }
    }

    public void EnableDamageCaster()
    {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }

    public void DisableDamageCaster()
    {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }

    // private void OnDrawGizmos()
    // {
    //     if (_damageCasterCollider == null)
    //         _damageCasterCollider = GetComponent<BoxCollider>();
    //
    //     RaycastHit hit;
    //     //检测盒的绘制原点
    //     Vector3 originalPos = transform.position + ( -_damageCasterCollider.bounds.size.z) * transform.forward;
    //     //检测盒各个轴上的大小
    //     Vector3 halfExtents = _damageCasterCollider.bounds.size / 2;
    //     //检测盒的最大投射距离
    //     float maxDistance = _damageCasterCollider.bounds.size.z;
    //     bool isHit = Physics.BoxCast(originalPos, halfExtents, transform.forward, out hit, 
    //         transform.rotation, maxDistance, 1 << 6);
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawLine(originalPos + (transform.forward * _damageCasterCollider.bounds.extents.z), originalPos + transform.forward * (_damageCasterCollider.bounds.size.z + _damageCasterCollider.bounds.extents.z));
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawCube(originalPos, _damageCasterCollider.bounds.size);
    //     if (isHit)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawWireSphere(hit.point, 0.3f);
    //     }
    // }
}
