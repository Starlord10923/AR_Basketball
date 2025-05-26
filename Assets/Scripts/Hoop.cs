using UnityEngine;

public class Hoop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.IncreaseScore();
    }
}
