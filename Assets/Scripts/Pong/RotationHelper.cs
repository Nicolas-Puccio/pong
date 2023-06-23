using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHelper : MonoBehaviour
{


  ///<summary>
  ///Convierte los grados a una direccion Vector2
  ///<para>0 = new Vector2(1, 0) = arriba</para>
  ///<para>90 = new Vector2(0, 1) = derecha</para>
  ///<para>180 = new Vector2(-1, 0) = abajo</para>
  ///<para>270 = new Vector2(0-1) = izquierda</para>
  ///</summary>
  public static Vector2 DegreesToVector2(float degrees)
  {
    return new Vector2(
        Mathf.Cos(degrees * Mathf.Deg2Rad),
        Mathf.Sin(degrees * Mathf.Deg2Rad)
    );
  }



  ///<summary>
  ///Rota un vector2 en base a una cierta cantidad de grados + o -
  ///<para>0 = mismo output</para>
  ///<para>90 = rota a la derecha 90 grados</para>
  ///<para>-90 = rota a la izquierda 90 grados</para>
  ///</summary>
  public static Vector2 RotateVector2(Vector2 direction, float degrees)
  {
    float radians = Mathf.Deg2Rad * degrees;
    float cos = Mathf.Cos(radians);
    float sin = Mathf.Sin(radians);

    float newX = direction.x * cos - direction.y * sin;
    float newY = direction.x * sin + direction.y * cos;

    return new Vector2(newX, newY);
  }
}
