using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class MakeMatchGround : MonoBehaviour
{
    //�׸��� ��Ŀ
    public GameObject groundAnchor;
    public GameObject matchGround;  // �ٴڹ��
    public GameObject[] matchIcons; // ���� �����ܵ�

    // �׸��� ũ�� ����
    public int width;
    public int height;
    public float cellSize;  // 160���� ������


    private void Start()
    {
        InitMatchGame();
    }

    private void InitMatchGame()
    {
        matchGround.GetComponent<RectTransform>().sizeDelta = new Vector2(width * cellSize, height * cellSize);
        Instantiate(matchGround, groundAnchor.transform, false);

        Vector2 startPos = new Vector2(-(width * cellSize)/2, -(height * cellSize)/2) + new Vector2(cellSize, cellSize)/2;    // ���⿡�� �ε��� ����ؼ� ���ϸ�ȴ�.
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
