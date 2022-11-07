using System.Collections;
using UnityEngine;

namespace Redcode.Moroutines.Demo
{
    public class AwaitingMoroutine : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var mor = Moroutine.Run(TickEnumerable());
            yield return mor.WaitForComplete();

            print("Moroutine Was Completed!");
        }

        private IEnumerable TickEnumerable()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1f);
                print("Tick!");
            }
        }
    }
}