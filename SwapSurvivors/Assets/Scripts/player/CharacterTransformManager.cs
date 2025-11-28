using Unity.VisualScripting;
using UnityEngine;

public class CharacterTransformManager : MonoBehaviour
{
    [SerializeField] private GameObject scytheCharacter;
    [SerializeField] private GameObject shotgunCharacter;

    private GameObject currentCharacter;
    private bool isT = true;

    private void Start()
    {
        currentCharacter = GameObject.FindWithTag("Player");
    }

    public void TransformCharacter()
    {
        Vector2 spawnPos;
        if (isT)
        {
            spawnPos = currentCharacter.transform.position;
            Destroy(currentCharacter);
            currentCharacter = Instantiate(shotgunCharacter, spawnPos, Quaternion.identity);
            isT = false;
        }
        else
        {
            spawnPos = currentCharacter.transform.position;
            Destroy(currentCharacter);
            currentCharacter = Instantiate(scytheCharacter, spawnPos, Quaternion.identity);
            isT = true;
        }
    }
}
