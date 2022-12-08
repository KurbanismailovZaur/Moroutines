using System.Collections;
using UnityEngine;

namespace Redcode.Moroutines.Demo
{
    public class ControlMoroutineExecution : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var mor = Moroutine.Run(TickEnumerable());

            yield return new WaitForSeconds(2.5f);
            mor.Stop();
            print("Moroutine was stopped!");

            yield return new WaitForSeconds(3f);
            print("Continue moroutine execution!");
            mor.Run();
        }

        private IEnumerable TickEnumerable()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(1f);
                print("Tick!");
            }
        }
    }
}