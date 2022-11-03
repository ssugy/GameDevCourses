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
    public CHARMOVE charMove;   // 구조체
    public DoIF4 doMove;        // 델리게이트
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
    Dictionary<int, DelegateWrap> doList;   // 딕셔너리 = 키와 밸류가 한쌍으로 이루어진 자료구조.

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
        listenSock.BeginAccept(AddPeer, null);      // 비동기함수. AddPeer 사용자가 접속후 실행된다.
    }
    public void CreateGameObject()
    {
        GameObject tmp = Resources.Load<GameObject>("Cube");
        if (tmp != null)
        {
            otherChar = GameObject.Instantiate<GameObject>(tmp);
        }
        myChar = GameObject.Instantiate<GameObject>(tmp);
        doCreate -= CreateGameObject;   //이러면 doCreate는 어떻게 되나요? null이되요.
    }

    // 왜 여기서 instantiate이런게 왜 안되냐면, main스레드가 아니라서 그렇다.
    // 이 말은 즉 AddPeer라는 BeginAccept(비동기)함수의 콜백함수는 추가적인 스레드를 생성한 것이다.
    public void AddPeer(IAsyncResult ar)
    {
        //Socket sock = (Socket)ar.AsyncState;
        otherPeer = listenSock.EndAccept(ar);   // 지금 접속한 클라이언트
        Debug.Log(otherPeer.RemoteEndPoint + "님이 접속했습니다.");
        
        // 클라이언트가 접속하면, 게임오브젝트 만드는 함수를 델리게이트형 변수에 추가해줌.
        doCreate = CreateGameObject;    // 델리게이트 변수를 선언해서, 함수를 추가해줬습니다. -> 이거 실질적인 실행은 어디서 하나요?


        // 이게 코드상의 에러는 안나는데, 실행이 실질적으로 안된다. 왜? 안되냐?
        //GameObject tmp = Resources.Load<GameObject>("Cube");
        //otherChar = GameObject.Instantiate<GameObject>(tmp);

        // 지금 접속한 클라이언트가 메시지를 받으면, 무슨 함수를 실행하나요? ReceiveCallBack이거를 실행
        otherPeer.BeginReceive(rBuffer, 0, rBuffer.Length, SocketFlags.None, ReceiveCallBack, otherPeer);  
    }

    // 클라이언트가 데이터를 보내면 QUEUE에 넣어준다.
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
        packetQueue.Enqueue(tmpBuffer); // 여기는 언제 넣는 거에요? 보낸고 난 뒤, 언제 값을 보내나요? 
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
                CHARMOVE charMove;  //구조체는 참조타입이 아니라서요 new를 안써요.

                charMove.xPos = hitInfo.point.x;    // 클릭한 곳 위치를 구조체에 값을 넣어줬다.
                charMove.yPos = hitInfo.point.y;
                charMove.zPos = hitInfo.point.z;

                byte[] packetType = BitConverter.GetBytes((short)ePACKETTYPE.CHARMOVE); // 이 배열에 들어가있는 값은 뭔가요? 1002 바이트로 변환한 값이 들어가있음
                byte[] id = BitConverter.GetBytes((int)1);  //임의의 숫자인 1로 넣어줬다.? random썻어요? id를 만들어줬어요. 1로만들어줬어요.
                byte[] xPos = BitConverter.GetBytes(charMove.xPos); // 구조체에 넣어놓은 값중 하나인 xPos을 바이트 배열로 변환해서 xPos배열에 넣어줬다.
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
    // 여기서 실질적으로 이동하는 코드
    public void MoveCharacter(int _uid, float x, float y, float z)
    {
        Vector3 end = new Vector3(x, y, z);
        if (_uid == 10) // 자기자신 = 10 =  서버
            myChar.transform.position = Vector3.MoveTowards(myChar.transform.position, end, Time.deltaTime * 2.4f);
        else if (_uid == 1) // 1은 클라이언트
            otherChar.transform.position = Vector3.MoveTowards(otherChar.transform.position, end, Time.deltaTime * 2.4f);
    }
    void Update()
    {
        MousePick();

        if (doCreate != null)
        {
            doCreate(); // 실제 실행은 update안에서 실행. 1번실행 큐브 2개를 생성해서, 1개는 나라는 존재, 다른1개는 클라이언트라는 존재로 생성
        }

        /**
         * 큐의 데이터 갯수가 1개이상이라는 의미는
         * 1) 클라이언트가 데이터를 보냈거나
         * 2) 서버가 클릭해서 클라이언트로 데이터를 보냈거나. 2개중 하나라는 의미.
         * 
         * 큐에 값이 존재하면, 그것을 꺼내서, 데이터분석해서 클라이언트용도의 큐브나 또는 서버용도의 큐브를 움직이겠다.
         * 어떻게 그게 클라언트용 큐브를 움직여야되는지, 서버용을 움직여야되는지 어떻게 아나요? uid용도
         */
        if (packetQueue.Count > 0)
        {
            byte[] data = packetQueue.Dequeue();    // 디큐하면 카운트 값이 줄어들어요. 안줄어들게 하려면, Peek사용 
            //패킷 타입(2) + 패킷 내용(126)
            byte[] _Packet = new byte[2];   // 헤더에서 패킷타입 구분하려고.
            Array.Copy(data, 0, _Packet, 0, 2);
            //패킷 타입2바이트 
            short packetType = BitConverter.ToInt16(_Packet, 0);    // 0일 수도있구요, 1000일수도있구요 1001, 1002일수있어요.
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

                        // id가 short면 숫자들이 어떻게 바뀌나요. 2, 4, 8, 12 ->
                        Array.Copy(data, 2, _id, 0, _id.Length);    // 데이터의 2번째 배열부터 id길이만큼 복사해서, id의 0번째 배열부터 값을 넣어준다.
                        Array.Copy(data, 6, _x, 0, _x.Length);      // 데이터의 6번째 배열부터 x의 길이만큼을 복사해서, x의 0번째 배열부터 값을 넣어준다.
                        Array.Copy(data, 10, _y, 0, _y.Length);
                        Array.Copy(data, 14, _z, 0, _z.Length);

                        int id = BitConverter.ToInt32(_id, 0);
                        float x = BitConverter.ToSingle(_x);
                        float y = BitConverter.ToSingle(_y);
                        float z = BitConverter.ToSingle(_z);
                        if (doList.ContainsKey(id))
                        {
                            // 아이디값이 이미 존재하는 경우. out hitinfo
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
