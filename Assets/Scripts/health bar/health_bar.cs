using UnityEngine;
using UnityEngine.UI;
public class health_bar : MonoBehaviour
{
    public Slider slider;
    // this one is responsable for the health bar transition
   public void setmaxvalue(int health){
    slider.maxValue =health;
    slider.value =health;
   }
    public void sethealthbar(int health){
        slider.value =health;
    }
}
