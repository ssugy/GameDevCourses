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

    // 총 5초라는 시간에서 1초후, 2초후, 3초후, 4초후, 5초후에 무엇을 하는 코드 작성
    IEnumerator FiveSec()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            Debug.Log($"{i+1}번째 실행하는 작업");
        }
    }


    // 정수형 변수의 값이 100이상이면 콘솔뷰에 변수의 값이 100이상입니다.를 출력(코루틴으로);
    // wait untilt사용
    int num = 0;

    IEnumerator Check100Num()
    {
        while (true)
        {
            if (num >= 100)
            {
                Debug.Log("변수의 값이 100 이상입니다.");
                break;
            }
            yield return null;
        }
    }

    IEnumerator WaitUntilSample()
    {
        yield return new WaitUntil(() => num >= 100);
        Debug.Log("변수의 값이 100이상 입니다.");
    }

    // Test1 이라는 함수 종료 후에 Test2라는 함수를 호출
    private IEnumerator Test1()
    {
        for (int i = 0; i < 10; i++)
        {
            //yield return null;    // 이걸 집어넣으면 동시호출 함수처럼 보인다.
            Debug.Log($"테스트1번 : {i}");
        }
        Debug.Log("테스트1 종료");
        yield return null;
    }

    IEnumerator Test2()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"테스트2번 : {i}");
        }
        Debug.Log("테스트2 종료");
        yield return null;
    }
    
    IEnumerator Test1Test2()
    {
        //yield return Test1();   // yield의 특성을 사용한 것. 코루틴 사용이라고 보기에는 다른 것이다.
        //yield return Test2();

        yield return StartCoroutine(Test1());   // 이게 강사님이 원하는 답
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
