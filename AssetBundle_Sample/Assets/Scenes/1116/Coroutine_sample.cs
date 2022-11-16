using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine_sample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FiveSec());
        StartCoroutine(Check100Num());
        StartCoroutine(Test1Test2());
    }

    // �� 5�ʶ�� �ð����� 1����, 2����, 3����, 4����, 5���Ŀ� ������ �ϴ� �ڵ� �ۼ�
    IEnumerator FiveSec()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            Debug.Log($"{i+1}��° �����ϴ� �۾�");
        }
    }


    // ������ ������ ���� 100�̻��̸� �ֺܼ信 ������ ���� 100�̻��Դϴ�.�� ���(�ڷ�ƾ����);
    // wait untilt���
    int num = 0;

    IEnumerator Check100Num()
    {
        while (true)
        {
            if (num >= 100)
            {
                Debug.Log("������ ���� 100 �̻��Դϴ�.");
                break;
            }
            yield return null;
        }
    }

    IEnumerator WaitUntilSample()
    {
        yield return new WaitUntil(() => num >= 100);
        Debug.Log("������ ���� 100�̻� �Դϴ�.");
    }

    // Test1 �̶�� �Լ� ���� �Ŀ� Test2��� �Լ��� ȣ��
    private IEnumerator Test1()
    {
        for (int i = 0; i < 10; i++)
        {
            //yield return null;    // �̰� ��������� ����ȣ�� �Լ�ó�� ���δ�.
            Debug.Log($"�׽�Ʈ1�� : {i}");
        }
        Debug.Log("�׽�Ʈ1 ����");
        yield return null;
    }

    IEnumerator Test2()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"�׽�Ʈ2�� : {i}");
        }
        Debug.Log("�׽�Ʈ2 ����");
        yield return null;
    }
    
    IEnumerator Test1Test2()
    {
        //yield return Test1();   // yield�� Ư���� ����� ��. �ڷ�ƾ ����̶�� ���⿡�� �ٸ� ���̴�.
        //yield return Test2();

        yield return StartCoroutine(Test1());   // �̰� ������� ���ϴ� ��
        yield return StartCoroutine(Test2());
    }

    private void Update()
    {
        if (num < 110)
        {
            Debug.Log(++num);
        }
    }
}
