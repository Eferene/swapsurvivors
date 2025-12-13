using UnityEngine;

public class NewWaveArea : MonoBehaviour
{
    private WaveController waveController;
    public bool playerInside = false;

    private void Awake()
    {
        waveController = GameObject.FindGameObjectWithTag("WaveController").GetComponent<WaveController>();
    }

    public void NewWave() => waveController.NewWave();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            BaseCharacterController controller = collision.GetComponent<BaseCharacterController>();

            if(controller != null)
            {
                playerInside = true;
                controller.SetCurrentObject(transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            BaseCharacterController controller = collision.GetComponent<BaseCharacterController>();

            if(controller != null)
            {
                playerInside = false;
                controller.ClearCurrentObject();
            }
        }
    }
}
