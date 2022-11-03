using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Packet
{
    public enum ePACKET_TYPE
    {
        NONE,
        PEERINFO = 1000,
        CHAR_SELECT,
        CHAR_MOVE
    }

    public struct PEERINFO
    {
        public ePACKET_TYPE packetType;
        public int uid;
        public string name;
        public short charType;
    }

    public struct CHARMOVE
    {
        public ePACKET_TYPE packetType;
        public int uid;
        public float xPos;
        public float yPos;
        public float zPos;
    }
}
