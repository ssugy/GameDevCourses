using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이걸 왜 만들었냐? 데이터를 보내는데, 용도에 따라서 묶어놓은 것.
// 묶어놓으면 보낼때 이 패킷만 보내면 된다. 라는 생각
// 네임스페이스 안에는? 
//1. 클래스. 2. 구조체, 3. 네임스페이, 4. 인터페이스
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