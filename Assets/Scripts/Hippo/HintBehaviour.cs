using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
  Vector3 initialPosition;
  float initialTime;

  public int delay;

  private void Update()
  {
    float offset = Mathf.Sin((Time.time - initialTime) * 5) * .25f;//speed and height
    float y = initialPosition.y > 0 ? initialPosition.y + offset : initialPosition.y - offset;

    transform.position = new Vector3(transform.position.x, y, transform.position.z);
  }

  void Start()
  {
    Destroy(gameObject, delay);
    initialPosition = transform.position;
    initialTime = Time.time;
  }
}
