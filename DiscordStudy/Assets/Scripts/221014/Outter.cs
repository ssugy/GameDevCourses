using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Study01;

public class Outter : MonoBehaviour
{
    private void Start()
    {
        NameSpaceSample ns = new NameSpaceSample();
        ns.Show();
    }

}
