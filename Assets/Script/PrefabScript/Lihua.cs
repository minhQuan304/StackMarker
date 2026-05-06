using UnityEngine;

public class Lihua : MonoBehaviour
{
    private ParticleSystem[] allParticleSystems;
    [SerializeField] private Transform finishPosition;
    //[SerializeField] private GameObject chest_close;
    //[SerializeField] private GameObject chest_open;
    void Start()
    {
        allParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (ParticleSystem ps in allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Play();
                }
                //if (chest_close != null)
                //{
                //    chest_close.SetActive(false);
                //}
                //if (chest_open != null)
                //{
                //    chest_open.SetActive(true);
                //}
            }
            EventManager.Raise(new FinishEvent { FinishPosition = finishPosition.position });
        }
    }
}
