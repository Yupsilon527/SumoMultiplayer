using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppliationQuitButton : MonoBehaviour, iButton
{
    public void Pressed()
    {
#if UNITY_EDITOR
        Debug.Log("Application QUIT");
#else
        Application.Quit();   
#endif
    }
}
