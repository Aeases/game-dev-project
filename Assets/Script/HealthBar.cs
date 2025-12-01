using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setHealth(float health)
    {
        slider.value = health;
    }
    public void setMaxHealth(float health)
    {
        slider.maxValue = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
