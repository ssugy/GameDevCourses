using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1단계 게임매니저를 싱글톤으로 변경
    #region 싱글톤 지정
    private static GameManager _unique;
    public static GameManager s_instance
    {
        get
        {
            return _unique;
        }
    }
    private void Awake()
    {
        if (_unique == null)
        {
            _unique = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);  // 해당 오브젝트는 씬이 전환되어도 파괴되지 않는다.
    }
    #endregion

    
}
