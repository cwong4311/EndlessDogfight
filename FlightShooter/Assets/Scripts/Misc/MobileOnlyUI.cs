using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileOnlyUI : MonoBehaviour
{
    private void Start()
    {
        if (Application.isMobilePlatform == false)
        {
            gameObject.SetActive(false);
        }
    }
}
