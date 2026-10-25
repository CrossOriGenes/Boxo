using UnityEngine;

public interface IVent
{
    void VentIn();
    void VentOut();    
    void Navigate(Vector2 direction);
}
