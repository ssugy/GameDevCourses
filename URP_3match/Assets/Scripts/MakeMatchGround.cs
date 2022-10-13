using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class MakeMatchGround : MonoBehaviour
{
    //그리드 앵커
    public GameObject groundAnchor;
    public GameObject matchGround;  // 바닥배경
    public GameObject[] matchIcons; // 각각 아이콘들

    // 그리드 크기 지정
    public int width;
    public int height;
    public float cellSize;  // 160으로 생각함


    private void Start()
    {
        InitMatchGame();
    }

    private void InitMatchGame()
    {
        matchGround.GetComponent<RectTransform>().sizeDelta = new Vector2(width * cellSize, height * cellSize);
        Instantiate(matchGround, groundAnchor.transform, false);

        Vector2 startPos = new Vector2(-(width * cellSize)/2, -(height * cellSize)/2) + new Vector2(cellSize, cellSize)/2;    // 여기에서 인덱스 계산해서 더하면된다.
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 iconPos = startPos + new Vector2(i, j) * (cellSize);
                GameObject icon = Instantiate(matchIcons[Random.Range(0, matchIcons.Length)], groundAnchor.transform, false);
                icon.GetComponent<RectTransform>().localPosition = iconPos;
                icon.name = $"{i}_{j}_{icon.name}";
                print(iconPos);
            }
        }
    }
}
