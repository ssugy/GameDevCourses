using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.ComponentModel;
using System.Threading;
using UnityEngine.TextCore.Text;
using System.Text;
using Game.Packet;

delegate void DoCharacter();
public delegate void DoInput4(int id, float x, float y, float z);

public class DelegateWrap
{
    public CHARMOVE charMove;
    public DoInput4 doMove;
}
public class P2pClient : MonoBehaviour
{
    Socket clientSocket;
    string strIP;
    int port;
    byte[] rBuffer;
    byte[] sBuffer;

    DoCharacter doCreate;
    DoInput4 doMove;
    ePACKET_TYPE ePACKETTYPE;
    GameObject myCharater;

    private Queue<byte[]> packetQueue;
    Dictionary<int, DelegateWrap> doDictionary;

    private void Awake()
    {
        strIP = "127.0.0.1";
        port = 8082;
        rBuffer = new byte[128];
        sBuffer = new byte[128];
        doCreate = null;
        ePACKETTYPE = ePACKET_TYPE.NONE;
        packetQueue = new Queue<byte[]>();
        doDictionary = new Dictionary<int, DelegateWrap>();
    }

    void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(strIP), port);
        clientSocket.BeginConnect(iPEndPoint, AddPeer, null);
    }

    private void AddPeer(IAsyncResult ar)
    {
        Socket otherPeer = (Socket)ar.AsyncState;
        Debug.Log("���ӵ�");
        doCreate += InstantiateCube;

        // ���ӿϷ�Ǹ� �޽��� �ϳ� ������
        byte[] packetType = new byte[2];
        byte[] message = new byte[126];
        packetType = BitConverter.GetBytes((int)ePACKET_TYPE.CHAR_SELECT);
        message = Encoding.Default.GetBytes("�ȳ��ϼ���");
        Array.Copy(packetType, 0, sBuffer, 0, packetType.Length);
        Array.Copy(message, 0, sBuffer, 2, message.Length);
        otherPeer.Send(sBuffer);
    }

    private void InstantiateCube()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            Instantiate(tmp);
        }
        myCharater = Instantiate<GameObject>(tmp);
        doCreate -= InstantiateCube;
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

                // ��Ŷ �����ؼ� ������ ������.  x y z ��ǥ ���� �����ؼ� ������ 4����Ʈ �Ǽ�
                byte[] packetType = BitConverter.GetBytes((ushort)ePACKET_TYPE.CHAR_MOVE);
                byte[] uid = BitConverter.GetBytes(10);   // ���̵� �ڸ�
                byte[] xPos = BitConverter.GetBytes(charMove.xPos);  
                byte[] yPos = BitConverter.GetBytes(charMove.yPos);  
                byte[] zPos = BitConverter.GetBytes(charMove.zPos);
                Array.Copy(packetType, 0, sBuffer, 0, packetType.Length);
                Array.Copy(uid, 0, sBuffer, 2, uid.Length);
                Array.Copy(xPos, 0, sBuffer, 6, xPos.Length);
                Array.Copy(yPos, 0, sBuffer, 10, yPos.Length);
                Array.Copy(zPos, 0, sBuffer, 14, zPos.Length);
                clientSocket.BeginSend(sBuffer, 0, sBuffer.Length, SocketFlags.None, SendCallBack, null);
            } 
        }
    }

    private void SendCallBack(IAsyncResult ar)
    {
        byte[] tmpBuffer = new byte[128];
        Array.Copy(sBuffer, 0, tmpBuffer, 0, sBuffer.Length);
        Array.Clear(sBuffer, 0, sBuffer.Length);
        packetQueue.Enqueue(tmpBuffer); // send������ ������ �����ؼ� ť�� �ֱ�
        
        // �ٽ� ���� �غ�
        clientSocket.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, null);
    }

    private void ReceiveCallBack(IAsyncResult ar)
    {
        byte[] tmpBuffer = new byte[128];
        Array.Copy(rBuffer, 0, tmpBuffer, 0, rBuffer.Length);
        Array.Clear(rBuffer, 0, rBuffer.Length);
        packetQueue.Enqueue(tmpBuffer);

        clientSocket.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, null);
    }

    void Update()
    {
        if (doCreate != null)
        {
            doCreate();
        }

        if (packetQueue.Count > 0)
        {
            byte[] tmpBuffer = packetQueue.Dequeue();
            byte[] packetType = new byte[2];

            // 2����Ʈ�� ��Ŷ Ÿ�� + ��Ŷ ����(126)
            Array.Copy(tmpBuffer, 0, packetType, 0, packetType.Length);
            ePACKETTYPE = (ePACKET_TYPE)BitConverter.ToInt32(packetType);

            switch (ePACKETTYPE)
            {
                case ePACKET_TYPE.NONE:
                    Debug.Log("None ó��");
                    break;
                case ePACKET_TYPE.PEERINFO:
                    Debug.Log("peerinfo ó��");
                    break;
                case ePACKET_TYPE.CHAR_SELECT:
                    // ����Ʈ ���� ó��
                    Debug.Log("����Ʈ ó��");
                    break;
                case ePACKET_TYPE.CHAR_MOVE:
                    // ���̵� �Ľ��� ���� ���� ��.
                    byte[] idBuff = new byte[4];
                    Array.Copy(tmpBuffer, 2, idBuff, 0, idBuff.Length);
                    int id = BitConverter.ToInt32(idBuff);

                    // xyz�Ľ�

                    // ���̵� �����ϸ� ���� ���� �ʿ� ����. -> xyz���� �������ش�.
                    if (doDictionary.ContainsKey(id))
                    {
                        DelegateWrap actionValue;
                        if(doDictionary.TryGetValue(id, out actionValue))
                        {
                            doDictionary[id].charMove.xPos = 10;    // �̷��� �������� heap�޸𸮿� �־�ߵǼ� ����ü�� �ȵǰ� Ŭ������ �ؾߵȴ�.
                            doDictionary[id].charMove.yPos = 10;    
                            doDictionary[id].charMove.zPos = 10;    
                            //doDictionary[id].doMove = MoveCharacter;  // �̹� ���̵� �ִٴ� �̾߱�� �Լ��� ������ �ִٴ� �̾߱�.
                        }
                    }
                    else
                    {
                        // Ű���� �������� ������, ��Ŷ������ �Ľ��ؼ� ���� �̵���Ű��
                        DelegateWrap doWrap = new DelegateWrap();
                        doWrap.charMove.uid = id;   // �ӽ� ���̵� 10 �ο�
                        doWrap.charMove.xPos = 10;  // �ӽð� �־���. ������ �Ľ��ؾߵ�
                        doWrap.charMove.yPos = 10;
                        doWrap.charMove.zPos = 10;
                        doWrap.doMove = MoveCharacter;
                        doDictionary.Add(doWrap.charMove.uid, doWrap);
                    }
                    break;
                default:
                    break;
            }
        }

        // ����ȸ�������� null�ʱ�ȭ�� ���� ����.(�³�?)
        for (int i = 0; i < doDictionary.Count; i++)
        {
            doDictionary[i].doMove(doDictionary[i].charMove.uid
                           , doDictionary[i].charMove.xPos
                           , doDictionary[i].charMove.yPos
                           , doDictionary[i].charMove.zPos);
        }
    }

    // ���� ���̵� �ش��ϴ� ��ǥ�� �̵�
    private void MoveCharacter(int uid, float x, float y, float z)
    {

    }
}
