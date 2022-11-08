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

    //StructLayout : �޸𸮿� �ִ� Ŭ���� �Ǵ� ����ü�� ������ �ʵ忡 ���� ���� ���̾ƿ��� ������ �� �ֽ��ϴ�.
    // �⺻���� 0�̰� 0�����ϸ� 24����Ʈ ���´�. 
    // 1�� 10����Ʈ
    // 2�� 12����Ʈ
    // �̰� ����ϴ� ������ cpu�� ������ ó�� ������ �����ֱ� ���ؼ��� 32��Ʈ(4����Ʈ ����), 64��Ʈ(8����Ʈ ����)
    // ��¥�����͸� ä���ִ� ���� �е� ����Ʈ�̴�.
    // ���� ����Ʈ ������� �����ض�(�̰� ���̰� �ֳ�? ���̴� ������ �ڵ� ���Ĵٵ��̴�.)
    // cpu�ü���� ����ϱ� ���� �����صδ� ���� ����.(�̰Ŵ� Ȯ���� ����, ����� �ǰ�)
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DirInfo
    {
        public byte d;  //1
        //public byte d1;  //1
        //public char f;  // 1(���������� 1)
        //public char f1;  // 1(���������� 1)
        //public char f2;  // 1(���������� 1)
        public int g;   //4
        //public long e;  //8
    }

    // �ʵ� ������ Ȱ�� ����
    [StructLayout(LayoutKind.Explicit)]
    public struct CharacterInfo
    {
        // �ʵ������(����Ʈ) - ���۹���Ʈ�� �� ����Ʈ�� Ȱ��, ������ ������������ 4����Ʈ �ϳ� �� ������̴�.
        [FieldOffset(0)] public uint Word;  //4����Ʈ�ε� 0���� ����
        [FieldOffset(0)] public byte Byte0; //��ġ�� 0���� ����Ʈ �Է�
        [FieldOffset(1)] public byte Byte1; // ��ġ�� 1����Ʈ����(8��Ʈ �̵� ��) ����Ʈ �Է�
        [FieldOffset(2)] public byte Byte2; // ��ġ�� 2����Ʈ����(16��Ʈ �̵� ��) ����Ʈ �Է�
        [FieldOffset(3)] public byte Byte3; // ��ġ�� 3����Ʈ����(24��Ʈ �̵� ��) ����Ʈ �Է�
    }

    void Start()
    {
        Debug.Log($"sizeof(int) : {sizeof(int)}");
        Debug.Log($"sizeof(long) : {sizeof(long)}");
        Debug.Log($"sizeof(char) : {sizeof(char)}");

        int structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(StructA));
        Debug.Log($"����ü A�� ũ�� : {structSize}");

        DirInfo dirInfo = new DirInfo();    // ����ü �ȿ� ������Ƽ/��Ʈ����Ʈ�� ���ԵǾ� ������, new�Ҵ� �ؾߵȴ�.
                                            // ����ü �ȿ� ����ϴ� heap�޸𸮰� �����ϸ� new�Ҵ����� ����ü ����� ��� �Ѵ�.
        Debug.Log("���� DirInfo ������ : " + Marshal.SizeOf(dirInfo));

        var cInfo = new CharacterInfo(); 
        cInfo.Byte0 = 10;
        cInfo.Byte1 = 11;
        cInfo.Byte2 = 12;
        cInfo.Byte3 = 13;
        Debug.Log($"{(uint)cInfo.Word}");

        CharacterInfo copyInfo = new CharacterInfo();
        copyInfo = cInfo;
        Debug.Log($"ī������ byte0 : {copyInfo.Byte0}");
        Debug.Log($"ī������ byte1 : {copyInfo.Byte1}");
        Debug.Log($"ī������ byte2 : {copyInfo.Byte2}");
        Debug.Log($"ī������ byte3 : {copyInfo.Byte3}");
    }
}
