using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectScaler : MonoBehaviour
{
    GameObject gameObjectForScaling;
    float lastScreenWidth, lastScreenHeight, newObjectScale;

    void Start()
    {
        gameObjectForScaling = GameObject.FindGameObjectWithTag("Object");

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        newObjectScale = lastScreenWidth / lastScreenHeight;
        ChangeObjectsScale(newObjectScale);
    }

    void Update()
    {
        CheckResolution();
    }
    
    void CheckResolution()
    {
        if (Screen.height != Convert.ToInt32(lastScreenHeight) || Screen.width != Convert.ToInt32(lastScreenWidth))
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            newObjectScale = lastScreenWidth / lastScreenHeight;
            ChangeObjectsScale(newObjectScale);
        }
    }

    void ChangeObjectsScale(float newScale)
    {
        if (newScale < 1)
        {
            gameObjectForScaling.GetComponent<Transform>().localScale = new Vector3(newScale, newScale, 1);
        }
        else
        {
            gameObjectForScaling.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        }
    }
}
