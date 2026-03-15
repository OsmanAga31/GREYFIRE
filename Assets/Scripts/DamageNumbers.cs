using System;
using UnityEngine;
using PixelBattleText;

public class DamageNumbers : MonoBehaviour
{
    public TextAnimation textAnimation;

    public void PlayDamageEffect(Vector3 position, float damage)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(position);
        
        PixelBattleTextController.DisplayText(damage.ToString(), textAnimation, viewportPosition);
    }
    
}
