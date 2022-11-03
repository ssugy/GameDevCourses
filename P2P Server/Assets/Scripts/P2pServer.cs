using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Game.Packet;
//public enum ePACKETTYPE
//{
//    NONE,
//    PEERINFO = 1000,
//    CHARSELECT
//}
//public struct PEERINFO
//{
//    public int uid;
//    public string name;
//    public short charType;
//}

public delegate void Do();
public delegate void DoIF4(int id, float x, float y, float z);

public class DelegateWrap
{
    public CHARMOVE charMove;
    public DoIF4 doMove;
}
public class P2pServer : MonoBehaviour
{
    Socket listenSock;
    string IP;
    int port;
    byte[] sBuffer;
    byte[] rBuffer;
    GameObject myChar;
    GameObject otherChar;
    Do doCreate;
    Queue<byte[]> packetQueue;
    ePACKETTYPE ePacketType;
    Dictionary<int, DelegateWrap> doList;

    Socket otherPeer;

    private void Awake()
    {
        IP = "127.0.0.1";
        port = 8082;
        sBuffer = new byte[128];
        rBuffer = new byte[128];
        packetQueue = new Queue<byte[]>();
        doCreate = null;
        ePacketType = ePACKETTYPE.NONE;
        doList = new Dictionary<int, DelegateWrap>();
    }
    void Start()
    {
        listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IP), port);
        listenSock.Bind(ip);
        listenSock.Listen(10);
        listenSock.BeginAccept(AddPeer, null);
    }
    public void CreateGameObject()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            otherChar = GameObject.Instantiate<GameObject>(tmp);
        }
        myChar = GameObject.Instantiate<GameObject>(tmp);
        doCreate -= CreateGameObject;
    }
    public void AddPeer(IAsyncResult ar)
    {
        //Socket sock = (Socket)ar.AsyncState;
        otherPeer = listenSock.EndAccept(ar);
        Debug.Log(otherPeer.RemoteEndPoint + "님이 접속했습니다.");
        doCreate = CreateGameObject;
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);
    }
    public void ReceiveCallBack(IAsyncResult ar)
    {
        Socket otherPeer = (Socket)ar.AsyncState;
        // queue에 enqueue
        byte[] tmp = new byte[128];
        Array.Copy(rBuffer, tmp, rBuffer.Length);
        Array.Clear(rBuffer, 0, rBuffer.Length);
        packetQueue.Enqueue(tmp);
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);
    }
    public void SendCallBack(IAsyncResult ar)
    {
        Socket otherPeer = (Socket)ar.AsyncState;

        byte[] tmpBuffer = new byte[128];
        Array.Copy(sBuffer, 0, tmpBuffer, 0, sBuffer.Length);
        Array.Clear(sBuffer, 0, sBuffer.Length);
        packetQueue.Enqueue(tmpBuffer);
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);
    }

    public void MousePick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                CHARMOVE charMove;
                charMove.xPos = hitInfo.point.x;
                charMove.yPos = hitInfo.point.y;
                charMove.zPos = hitInfo.point.z;
                byte[] packetType = BitConverter.GetBytes((short)ePACKETTYPE.CHARMOVE);
                byte[] id = BitConverter.GetBytes((int)1);
                byte[] xPos = BitConverter.GetBytes(charMove.xPos);
                byte[] yPos = BitConverter.GetBytes(charMove.yPos);
                byte[] zPos = BitConverter.GetBytes(charMove.zPos);
                Array.Copy(packetType, 0, sBuffer, 0, packetType.Length);
                Array.Copy(id, 0, sBuffer, 2, id.Length);
                Array.Copy(xPos, 0, sBuffer, 6, xPos.Length);
                Array.Copy(yPos, 0, sBuffer, 10, yPos.Length);
                Array.Copy(zPos, 0, sBuffer, 14, zPos.Length);
                otherPeer.BeginSend(sBuffer, 0, sBuffer.Length, SocketFlags.None, SendCallBack, otherPeer);
            }
        }
    }
    public void MoveCharacter(int _uid, float x, float y, float z)
    {
        Vector3 end = new Vector3(x, y, z);
        if (_uid == 10)
            myChar.transform.position = Vector3.MoveTowards(myChar.transform.position, end, Time.deltaTime * 2.4f);
        else if (_uid == 1)
            otherChar.transform.position = Vector3.MoveTowards(otherChar.transform.position, end, Time.deltaTime * 2.4f);
    }
    void Update()
    {
        MousePick();

        if (doCreate != null)
        {
            doCreate();
        }
        if (packetQueue.Count > 0)
        {
            byte[] data = packetQueue.Dequeue();
            //패킷 타입(2) + 패킷 내용(126)
            byte[] _Packet = new byte[2];
            Array.Copy(data, 0, _Packet, 0, 2);
            //패킷 타입2바이트 
            short packetType = BitConverter.ToInt16(_Packet, 0);
            switch ((int)packetType)
            {
                case (int)ePACKETTYPE.PEERINFO:

                    break;
                case (int)ePACKETTYPE.CHARSELECT:
                    {
                        // 내용을 
                    }
                    break;
                case (int)ePACKETTYPE.CHARMOVE:
                    {
                        byte[] _id = new byte[4];
                        byte[] _x = new byte[4];
                        byte[] _y = new byte[4];
                        byte[] _z = new byte[4];
                        Array.Copy(data, 2, _id, 0, _id.Length);
                        Array.Copy(data, 6, _x, 0, _x.Length);
                        Array.Copy(data, 10, _y, 0, _y.Length);
                        Array.Copy(data, 14, _z, 0, _z.Length);
                        int id = BitConverter.ToInt32(_id, 0);
                        float x = BitConverter.ToSingle(_x);
                        float y = BitConverter.ToSingle(_y);
                        float z = BitConverter.ToSingle(_z);
                        if (doList.ContainsKey(id))
                        {
                            DelegateWrap actionValue;
                            if (doList.TryGetValue(id, out actionValue))
                            {
                                actionValue.charMove.xPos = x;
                                actionValue.charMove.yPos = y;
                                actionValue.charMove.zPos = z;
                                //actionValue.doMove = MoveCharacter;
                            }
                        }
                        else
                        {
                            DelegateWrap newData = new DelegateWrap();
                            newData.charMove.uid = id;
                            newData.charMove.xPos = x;
                            newData.charMove.yPos = y;
                            newData.charMove.zPos = z;
                            newData.doMove = MoveCharacter;
                            doList.Add(id, newData);
                        }
                    }
                    break;
            }
        }
        foreach (KeyValuePair<int, DelegateWrap> one in doList)
        {
            one.Value.doMove(one.Value.charMove.uid,
                       one.Value.charMove.xPos,
                       one.Value.charMove.yPos,
                       one.Value.charMove.zPos);
        }
    }
}
