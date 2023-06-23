using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
  public int delay;

  void Start()
  {
    Destroy(gameObject, delay);
  }
}
