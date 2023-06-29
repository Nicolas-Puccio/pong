using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
  Vector3 initialPosition;
  float initialTime;
  float speed = 5;
  float height = .25f;

  public int delay;



  void Start()
  {
    if (delay == 0)
      Debug.LogError("delay not set");

    Destroy(gameObject, delay);//destroy after delay

    initialPosition = transform.position;
    initialTime = Time.time;
  }



  private void Update()
  {
    float offset = Mathf.Sin((Time.time - initialTime) * speed) * height;//get up and down offset based on time

    float y = initialPosition.y > 0 ? initialPosition.y + offset : initialPosition.y - offset;//add or subtract offset depending on position

    transform.position = new Vector3(transform.position.x, y, transform.position.z);//set new position
  }
}
