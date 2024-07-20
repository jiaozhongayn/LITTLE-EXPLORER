/*
* @Author: 教忠言
* @Description:
* @Date: 2024年04月24日 星期三 13:04:53
* @Modify:
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput, VerticalInput;
    public bool ShiftKeyDown;
    public bool MouseButtonDown;

    private void Update()
    {
        if (!MouseButtonDown && Time.timeScale != 0)
        {
            MouseButtonDown = Input.GetMouseButtonDown(0);
        }
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        ShiftKeyDown = Input.GetKey(KeyCode.LeftShift);
    }

    private void OnDisable()
    {
        HorizontalInput = 0;
        VerticalInput = 0;
        MouseButtonDown = false;
    }
}
