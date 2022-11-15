using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * FPS : ÇÁ·¹ÀÓ per sec
 */
public class FPSChecker : MonoBehaviour
{
    public Text uiText;
    float elapsed;
    float count;
    void Start()
    {
        elapsed = 0;
        count = 0;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        count++;
        if (elapsed >= 1)
        {
            uiText.text = count.ToString() + " FPS";
            elapsed -= 1;
            count = 0;
        }
    }
}
