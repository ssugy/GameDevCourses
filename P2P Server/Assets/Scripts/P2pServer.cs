using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using UnityEditor.PackageManager.Requests;
using Unity.VisualScripting;
using Game.Packet;

delegate void DoCharacter();
public delegate void DoIF4(int id, float x, float y, float z);
public class DelegateWrap
{
    public CHARMOVE charMove;
    public DoIF4 doMove;
}
public class P2pServer : MonoBehaviour
{
    Socket listenSocket;
    string strIP;
    int port;
    byte[] rBuffer;
    byte[] sBuffer;

    DoCharacter doCreate;
    GameObject myCharacter;
    GameObject otherCharacter;
    Queue<byte[]> packetQueue;
    ePACKETTYPE ePACKETTYPE;
    Dictionary<int, DelegateWrap> doList;

    Socket otherPeer;

    private void Awake()
    {
        strIP = "127.0.0.1";
        port = 8082;
        rBuffer = new byte[128];
        sBuffer = new byte[128];
        doCreate = null;
        packetQueue = new Queue<byte[]>();
        ePACKETTYPE = ePACKETTYPE.NONE;
        doList = new Dictionary<int, DelegateWrap>();
    }

    private void Start()
    {
        //GetLocalIPAddress();
        listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(strIP), port);
        listenSocket.Bind(iPEndPoint);
        listenSocket.Listen(10);
        listenSocket.BeginAccept(AddPeer, null);
    }

    private void AddPeer(IAsyncResult ar)
    {
        otherPeer = listenSocket.EndAccept(ar);
        Debug.Log($"{otherPeer.RemoteEndPoint} 접속됨");

        doCreate = CreateGameObject;

        // 상대방의 정보를 송신 할 수 있도록 준비
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);
    }

    // 상대방이 메시지를 보내면 queue에 값을 넣는다.
    private void ReceiveCallBack(IAsyncResult ar)
    {
        Socket otherPeer = (Socket)ar.AsyncState;
        byte[] tmpBuffer = new byte[rBuffer.Length];
        Array.Copy(rBuffer, tmpBuffer, rBuffer.Length);
        Array.Clear(rBuffer, 0, rBuffer.Length);
        packetQueue.Enqueue(tmpBuffer);

        // 리시브 콜백 끝난 뒤에 다시 비긴 리시브 실행
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);
    }

    private void CreateGameObject()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            otherCharacter = Instantiate(tmp);
        }
        myCharacter = Instantiate<GameObject>(tmp);
        doCreate -= CreateGameObject;
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
                byte[] xPos =  BitConverter.GetBytes(charMove.xPos);
                byte[] yPos =  BitConverter.GetBytes(charMove.yPos);
                byte[] zPos =  BitConverter.GetBytes(charMove.zPos);
                Array.Copy(packetType, 0, sBuffer, 0, packetType.Length);
                Array.Copy(id, 0, sBuffer, 2, id.Length);
                Array.Copy(xPos, 0, sBuffer, 6, xPos.Length);
                Array.Copy(xPos, 0, sBuffer, 10, yPos.Length);
                Array.Copy(xPos, 0, sBuffer, 14, zPos.Length);
                otherPeer.BeginSend(sBuffer, 0, sBuffer.Length, SocketFlags.None, SendCallBack, otherPeer);
            }
        }
    }

    private void SendCallBack(IAsyncResult ar)
    {
        Socket otherPeer = (Socket)ar.AsyncState;

        byte[] tmpBuffer = new byte[128];
        Array.Copy(sBuffer, 0, tmpBuffer, 0, sBuffer.Length);
        Array.Clear(sBuffer, 0, sBuffer.Length);
        packetQueue.Enqueue(tmpBuffer);
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);
    }

    public void MoveCharacter(int _uid, float x, float y, float z)
    {
        Vector3 end = new Vector3(x, y, z);
        if (_uid == 10)
        {
            myCharacter.transform.position = Vector3.MoveTowards(myCharacter.transform.position, end, Time.deltaTime * 2.4f);
        }
        else if (_uid == 1)
        {
            otherCharacter.transform.position = Vector3.MoveTowards(otherCharacter.transform.position, end, Time.deltaTime * 2.4f);  
        }
    }

    #region 자기 아이피 가져오기
    //public Text hintText;
    // 자기 아이피 가져오는 방법
    //public string GetLocalIPAddress()
    //{
    //    var host = Dns.GetHostEntry(Dns.GetHostName());
    //    foreach (var ip in host.AddressList)
    //    {
    //        if (ip.AddressFamily == AddressFamily.InterNetwork)
    //        {
    //            hintText.text = ip.ToString();
    //            return ip.ToString();
    //        }
    //    }
    //    throw new System.Exception("No network adapters with an IPv4 address in the system!");
    //}
    #endregion

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
            byte[] packetType = new byte[2];

            // 2바이트는 패킷 타입 + 패킷 내용(126)
            Array.Copy(data, 0, packetType, 0, packetType.Length);
            ePACKETTYPE = (ePACKETTYPE)BitConverter.ToInt32(packetType);

            switch (ePACKETTYPE)
            {
                case ePACKETTYPE.NONE:
                    Debug.Log("None 처리");
                    break;
                case ePACKETTYPE.PEERINFO:
                    Debug.Log("None 처리");
                    break;
                case ePACKETTYPE.CHARSELECT:
                    // 셀렉트 관련 처리
                    Debug.Log("셀렉트 처리");
                    break;
                case ePACKETTYPE.CHARMOVE:
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
                    break;
                default:
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
