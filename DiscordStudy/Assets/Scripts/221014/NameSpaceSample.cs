using UnityEngine;

namespace Study01
{
    namespace Study02
    {
        public class Test02
        {

        }
    }

    public class NameSpaceSample : MonoBehaviour
    {
        public int num1 = 10;

        public void Show()
        {
            Debug.Log("스터디 시작");
        }
    }

    public class Test01
    {
        public int num2;
    }

    struct SampleStruct
    {
        public string Name;
    }

    interface IStudy
    {
        void IStudyTest();
    }

    public class Test03
    {

    }
}
