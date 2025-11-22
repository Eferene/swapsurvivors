using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{
    private void Update()
    {
        StartCoroutine(enumerator(3f));
    }

    IEnumerator enumerator(float f)
    {
        Debug.Log(PlayerStats.Instance.GiveDamage());
        yield return new WaitForSeconds(f);
    }
}
