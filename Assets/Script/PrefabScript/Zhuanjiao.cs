using UnityEngine;

public class Zhuanjiao : MonoBehaviour
{
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float rotationY = transform.GetComponentInParent<Transform>().rotation.eulerAngles.y;
            EventManager.Raise(new Zhuanjiao1ColisionEvent { RotationY = rotationY });
            _animator.SetTrigger("isPlayAnim");

        }
    }
}
