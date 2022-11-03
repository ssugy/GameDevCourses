using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Game.Packet;

public delegate void Do();
public delegate void DoIF4(int id, float x, float y, float z);

public class DelegateWrap
{
    public CHARMOVE charMove;   // ����ü
    public DoIF4 doMove;        // ��������Ʈ
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
    Dictionary<int, DelegateWrap> doList;   // ��ųʸ� = Ű�� ����� �ѽ����� �̷���� �ڷᱸ��.

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
        listenSock.BeginAccept(AddPeer, null);      // �񵿱��Լ�. AddPeer ����ڰ� ������ ����ȴ�.
    }
    public void CreateGameObject()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            otherChar = GameObject.Instantiate<GameObject>(tmp);
        }
        myChar = GameObject.Instantiate<GameObject>(tmp);
        doCreate -= CreateGameObject;   //�̷��� doCreate�� ��� �ǳ���? null�̵ǿ�.
    }

    // �� ���⼭ instantiate�̷��� �� �ȵǳĸ�, main�����尡 �ƴ϶� �׷���.
    // �� ���� �� AddPeer��� BeginAccept(�񵿱�)�Լ��� �ݹ��Լ��� �߰����� �����带 ������ ���̴�.
    public void AddPeer(IAsyncResult ar)
    {
        //Socket sock = (Socket)ar.AsyncState;
        otherPeer = listenSock.EndAccept(ar);   // ���� ������ Ŭ���̾�Ʈ
        Debug.Log(otherPeer.RemoteEndPoint + "���� �����߽��ϴ�.");
        
        // Ŭ���̾�Ʈ�� �����ϸ�, ���ӿ�����Ʈ ����� �Լ��� ��������Ʈ�� ������ �߰�����.
        doCreate = CreateGameObject;    // ��������Ʈ ������ �����ؼ�, �Լ��� �߰�������ϴ�. -> �̰� �������� ������ ��� �ϳ���?


        // �̰� �ڵ���� ������ �ȳ��µ�, ������ ���������� �ȵȴ�. ��? �ȵǳ�?
        //GameObject tmp = Resources.Load<GameObject>("Cube");
        //otherChar = GameObject.Instantiate<GameObject>(tmp);

        // ���� ������ Ŭ���̾�Ʈ�� �޽����� ������, ���� �Լ��� �����ϳ���? ReceiveCallBack�̰Ÿ� ����
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);  
    }

    // Ŭ���̾�Ʈ�� �����͸� ������ QUEUE�� �־��ش�.
    public void ReceiveCallBack(IAsyncResult ar)
    {
        Socket otherPeer = (Socket)ar.AsyncState;
        // queue�� enqueue
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
        packetQueue.Enqueue(tmpBuffer); // ����� ���� �ִ� �ſ���? ������ �� ��, ���� ���� ��������? 
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
                CHARMOVE charMove;  //����ü�� ����Ÿ���� �ƴ϶󼭿� new�� �Ƚ��.

                charMove.xPos = hitInfo.point.x;    // Ŭ���� �� ��ġ�� ����ü�� ���� �־����.
                charMove.yPos = hitInfo.point.y;
                charMove.zPos = hitInfo.point.z;

                byte[] packetType = BitConverter.GetBytes((short)ePACKETTYPE.CHARMOVE); // �� �迭�� ���ִ� ���� ������? 1002 ����Ʈ�� ��ȯ�� ���� ������
                byte[] id = BitConverter.GetBytes((int)1);  //������ ������ 1�� �־����.? random�����? id�� ���������. 1�θ��������.
                byte[] xPos = BitConverter.GetBytes(charMove.xPos); // ����ü�� �־���� ���� �ϳ��� xPos�� ����Ʈ �迭�� ��ȯ�ؼ� xPos�迭�� �־����.
                byte[] yPos = BitConverter.GetBytes(charMove.yPos);
                byte[] zPos = BitConverter.GetBytes(charMove.zPos);

                Array.Copy(packetType, 0, sBuffer, 0, packetType.Length);   // 
                Array.Copy(id, 0, sBuffer, 2, id.Length);
                Array.Copy(xPos, 0, sBuffer, 6, xPos.Length);
                Array.Copy(yPos, 0, sBuffer, 10, yPos.Length);
                Array.Copy(zPos, 0, sBuffer, 14, zPos.Length);

                otherPeer.BeginSend(sBuffer, 0, sBuffer.Length, SocketFlags.None, SendCallBack, otherPeer);
            }
        }
    }
    // ���⼭ ���������� �̵��ϴ� �ڵ�
    public void MoveCharacter(int _uid, float x, float y, float z)
    {
        Vector3 end = new Vector3(x, y, z);
        if (_uid == 10) // �ڱ��ڽ� = 10 =  ����
            myChar.transform.position = Vector3.MoveTowards(myChar.transform.position, end, Time.deltaTime * 2.4f);
        else if (_uid == 1) // 1�� Ŭ���̾�Ʈ
            otherChar.transform.position = Vector3.MoveTowards(otherChar.transform.position, end, Time.deltaTime * 2.4f);
    }
    void Update()
    {
        MousePick();

        if (doCreate != null)
        {
            doCreate(); // ���� ������ update�ȿ��� ����. 1������ ť�� 2���� �����ؼ�, 1���� ����� ����, �ٸ�1���� Ŭ���̾�Ʈ��� ����� ����
        }

        /**
         * ť�� ������ ������ 1���̻��̶�� �ǹ̴�
         * 1) Ŭ���̾�Ʈ�� �����͸� ���°ų�
         * 2) ������ Ŭ���ؼ� Ŭ���̾�Ʈ�� �����͸� ���°ų�. 2���� �ϳ���� �ǹ�.
         * 
         * ť�� ���� �����ϸ�, �װ��� ������, �����ͺм��ؼ� Ŭ���̾�Ʈ�뵵�� ť�곪 �Ǵ� �����뵵�� ť�긦 �����̰ڴ�.
         * ��� �װ� Ŭ���Ʈ�� ť�긦 �������ߵǴ���, �������� �������ߵǴ��� ��� �Ƴ���? uid�뵵
         */
        if (packetQueue.Count > 0)
        {
            byte[] data = packetQueue.Dequeue();    // ��ť�ϸ� ī��Ʈ ���� �پ����. ���پ��� �Ϸ���, Peek��� 
            //��Ŷ Ÿ��(2) + ��Ŷ ����(126)
            byte[] _Packet = new byte[2];   // ������� ��ŶŸ�� �����Ϸ���.
            Array.Copy(data, 0, _Packet, 0, 2);
            //��Ŷ Ÿ��2����Ʈ 
            short packetType = BitConverter.ToInt16(_Packet, 0);    // 0�� �����ֱ���, 1000�ϼ����ֱ��� 1001, 1002�ϼ��־��.
            switch ((int)packetType)
            {
                case (int)ePACKETTYPE.PEERINFO:

                    break;
                case (int)ePACKETTYPE.CHARSELECT:
                    {
                        // ������ 
                    }
                    break;
                case (int)ePACKETTYPE.CHARMOVE:
                    {
                        byte[] _id = new byte[4];
                        byte[] _x = new byte[4];
                        byte[] _y = new byte[4];
                        byte[] _z = new byte[4];

                        // id�� short�� ���ڵ��� ��� �ٲ��. 2, 4, 8, 12 ->
                        Array.Copy(data, 2, _id, 0, _id.Length);    // �������� 2��° �迭���� id���̸�ŭ �����ؼ�, id�� 0��° �迭���� ���� �־��ش�.
                        Array.Copy(data, 6, _x, 0, _x.Length);      // �������� 6��° �迭���� x�� ���̸�ŭ�� �����ؼ�, x�� 0��° �迭���� ���� �־��ش�.
                        Array.Copy(data, 10, _y, 0, _y.Length);
                        Array.Copy(data, 14, _z, 0, _z.Length);

                        int id = BitConverter.ToInt32(_id, 0);
                        float x = BitConverter.ToSingle(_x);
                        float y = BitConverter.ToSingle(_y);
                        float z = BitConverter.ToSingle(_z);
                        if (doList.ContainsKey(id))
                        {
                            // ���̵��� �̹� �����ϴ� ���. out hitinfo
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
