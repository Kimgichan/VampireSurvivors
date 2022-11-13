using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // 함수의 로직을 다음과 같이 분리를 하는 경우가 있다.
    public void A()
    {
        // AA 로직 ... 
        AA();
    }

    public void AA()
    {
        //로직
    }


    // 문제 1. ACor의 로직 중 일부분을 AACor로 분리시키고자 한다.
    public IEnumerator ACor()
    {
        yield return null;

        while (true)
        {
            ////AACor로 분리시키고 싶은 로직
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(2f);
            //
        }
    }

    // 분리된 AACor 로직은 다음과 같다.
    public IEnumerator AACor()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(2f);
    }

    // ACor의 로직은 어떤 식으로 바껴야 하는가?
    public IEnumerator AnswerCor()
    {
        yield return null;

        while (true)
        {
            var aacor = AACor();
            while (aacor.MoveNext())
            {
                yield return aacor;
            }
        }
    }
}
