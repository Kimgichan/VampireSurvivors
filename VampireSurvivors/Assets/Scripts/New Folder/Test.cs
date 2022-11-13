using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // �Լ��� ������ ������ ���� �и��� �ϴ� ��찡 �ִ�.
    public void A()
    {
        // AA ���� ... 
        AA();
    }

    public void AA()
    {
        //����
    }


    // ���� 1. ACor�� ���� �� �Ϻκ��� AACor�� �и���Ű���� �Ѵ�.
    public IEnumerator ACor()
    {
        yield return null;

        while (true)
        {
            ////AACor�� �и���Ű�� ���� ����
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(2f);
            //
        }
    }

    // �и��� AACor ������ ������ ����.
    public IEnumerator AACor()
    {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(2f);
    }

    // ACor�� ������ � ������ �ٲ��� �ϴ°�?
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
