using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �̰� �� �������? �����͸� �����µ�, �뵵�� ���� ������� ��.
// ��������� ������ �� ��Ŷ�� ������ �ȴ�. ��� ����
// ���ӽ����̽� �ȿ���? 
//1. Ŭ����. 2. ����ü, 3. ���ӽ�����, 4. �������̽�
namespace Game.Packet
{
    public enum ePACKETTYPE
    {
        NONE,           // 0
        PEERINFO = 1000,
        CHARSELECT,     // 1001
        CHARMOVE        // 1002
    }

    public struct PEERINFO
    {
        public ePACKETTYPE packetType;      // 1000
        public int uid;
        public string name;
        public short charType;
    }

    public struct CHARMOVE
    {
        public ePACKETTYPE packetType;      // 1002
        public int uid;
        //public short uid;
        public float xPos;
        public float yPos;
        public float zPos;
    }
}