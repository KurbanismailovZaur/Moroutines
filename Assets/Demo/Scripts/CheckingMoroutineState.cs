using System.Collections;
using UnityEngine;

namespace Redcode.Moroutines.Demo
{
    public class CheckingMoroutineState : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var mor = Moroutine.Run(RandomAwaitingEnumerable());

            yield return new WaitForSeconds(2f);

            if (mor.IsCompleted)
                print("Moroutine Was Completed!");
            else
                print("Moroutine not completed yet..");
        }

        private IEnumerable RandomAwaitingEnumerable()
        {
            yield return new WaitForSeconds(Random.Range(0, 2) == 0 ? 1f : 3f);
            print("Random Awaiting Completed!");
        }
    }
}