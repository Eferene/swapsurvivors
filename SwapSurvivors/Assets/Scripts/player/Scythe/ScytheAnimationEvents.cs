using UnityEngine;

public class HideScytheAnim : MonoBehaviour
{
    [SerializeField] private ScyhteCharacter scytheCharacter;

    public void OnAnimationFinish()
    {
        if (!scytheCharacter.CheckCombo())
        {
            gameObject.SetActive(false);
        }
    }
}