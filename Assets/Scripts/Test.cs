using Redcode.Moroutines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private IEnumerator Start()
    {
        var mor = Moroutine.Run(TestRoutine());
        yield return mor.WaitForComplete();

        print(mor.IsCompleted);
    }

    private IEnumerable TestRoutine()
    {
        yield return new WaitForSeconds(1f);
        print("Completed");
    }
}
