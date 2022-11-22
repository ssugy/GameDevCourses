using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1�ܰ� ���ӸŴ����� �̱������� ����
    #region �̱��� ����
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
        DontDestroyOnLoad(gameObject);  // �ش� ������Ʈ�� ���� ��ȯ�Ǿ �ı����� �ʴ´�.
    }
    #endregion

    
}
