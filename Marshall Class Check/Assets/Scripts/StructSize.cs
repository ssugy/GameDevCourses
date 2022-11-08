using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StructSize : MonoBehaviour
{
    struct StructA
    {
        public byte a;
        public long b;
        public char c;
    }

    //StructLayout : 메모리에 있는 클래스 또는 구조체의 데이터 필드에 대한 실제 레이아웃을 제어할 수 있습니다.
    // 기본값은 0이고 0으로하면 24바이트 나온다. 
    // 1은 10바이트
    // 2는 12바이트
    // 이걸 사용하는 이유는 cpu의 데이터 처리 단위를 맞춰주기 위해서다 32비트(4바이트 단위), 64비트(8바이트 단위)
    // 가짜데이터를 채워주는 것이 패딩 바이트이다.
    // 작은 바이트 순서대로 정렬해라(이게 차이가 있나? 차이는 없지만 코딩 스탠다드이다.)
    // cpu운영체에서 사용하기 좋게 정렬해두는 것이 좋다.(이거는 확인을 못함, 강사님 의견)
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DirInfo
    {
        public byte d;  //1
        //public byte d1;  //1
        //public char f;  // 1(마샬에서는 1)
        //public char f1;  // 1(마샬에서는 1)
        //public char f2;  // 1(마샬에서는 1)
        public int g;   //4
        //public long e;  //8
    }

    // 필드 오프셋 활용 예제
    [StructLayout(LayoutKind.Explicit)]
    public struct CharacterInfo
    {
        // 필드오프셋(바이트) - 시작바이트를 한 바이트씩 활용, 동일한 시작지점에서 4바이트 하나 더 만든것이다.
        [FieldOffset(0)] public uint Word;  //4바이트인데 0부터 시작
        [FieldOffset(0)] public byte Byte0; //위치가 0부터 바이트 입력
        [FieldOffset(1)] public byte Byte1; // 위치가 1바이트시점(8비트 이동 후) 바이트 입력
        [FieldOffset(2)] public byte Byte2; // 위치가 2바이트시점(16비트 이동 후) 바이트 입력
        [FieldOffset(3)] public byte Byte3; // 위치가 3바이트시점(24비트 이동 후) 바이트 입력
    }

    void Start()
    {
        Debug.Log($"sizeof(int) : {sizeof(int)}");
        Debug.Log($"sizeof(long) : {sizeof(long)}");
        Debug.Log($"sizeof(char) : {sizeof(char)}");

        int structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(StructA));
        Debug.Log($"구조체 A의 크기 : {structSize}");

        DirInfo dirInfo = new DirInfo();    // 구조체 안에 프로퍼티/어트리뷰트가 포함되어 있으면, new할당 해야된다.
                                            // 구조체 안에 사용하는 heap메모리가 존재하면 new할당으로 구조체 만들어 줘야 한다.
        Debug.Log("마샬 DirInfo 사이즈 : " + Marshal.SizeOf(dirInfo));

        var cInfo = new CharacterInfo(); 
        cInfo.Byte0 = 10;
        cInfo.Byte1 = 11;
        cInfo.Byte2 = 12;
        cInfo.Byte3 = 13;
        Debug.Log($"{(uint)cInfo.Word}");

        CharacterInfo copyInfo = new CharacterInfo();
        copyInfo = cInfo;
        Debug.Log($"카피인포 byte0 : {copyInfo.Byte0}");
        Debug.Log($"카피인포 byte1 : {copyInfo.Byte1}");
        Debug.Log($"카피인포 byte2 : {copyInfo.Byte2}");
        Debug.Log($"카피인포 byte3 : {copyInfo.Byte3}");
    }
}
