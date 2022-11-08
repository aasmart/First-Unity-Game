using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    [SerializeField] private Gradient gradient;
    [SerializeField] private float transitionSpeed;

    private float _maxHealth;
    private float _currentHealth;
    private float _targetHealth;

    public void SetMaxHealth(float health)
    {
        _maxHealth = health;
        _targetHealth = health;
        _currentHealth = health;
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }

    public void TakeDamage(float amount)
    {
        _targetHealth -= amount;
        if (_targetHealth < 0)
            _targetHealth = 0;
    }

    public void Reset()
    {
        _targetHealth = _maxHealth;
    }

    private void Update()
    {
        if (Math.Abs(_currentHealth - _targetHealth) < .001f) return;
        
        _currentHealth = Mathf.Lerp(_currentHealth, _targetHealth, transitionSpeed * Time.deltaTime);
        fill.color = gradient.Evaluate(slider.normalizedValue);
        slider.value = _currentHealth;
    }
}
