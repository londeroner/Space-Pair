using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipEffects : MonoBehaviour
{
    public GameObject ShieldEffect;
    public GameObject ReflectEffect;

    public GameObject ExplosionPrefab;

    public Image HPBar;
    public TMP_Text ESText;

    private Ship _ship;

    private int _updateCounter = 0;
    private int _targetCounter = 10;

    private bool oldESState = false;
    private bool oldReflectState = false;

    void Start()
    {
        _ship = GetComponent<Ship>();

        oldESState = (_ship.Stats.ES <= 0);
        oldReflectState = _ship.Stats.CanReflect;

        ESText.transform.rotation = Camera.main.transform.rotation;
    }

    void Update()
    {
        _updateCounter++;

        if (_updateCounter >= _targetCounter)
        {
            _updateCounter = 0;

            ESText.text = _ship.Stats.ES.ToString();
            HPBar.fillAmount = _ship.Stats.CurrentHP / _ship.Stats.MaxHP;

            if (oldESState != (_ship.Stats.ES > 0))
            {
                var newState = _ship.Stats.ES > 0;
                ShieldEffect.SetActive(newState);
                oldESState = newState;
            }
            if (oldReflectState != _ship.Stats.CanReflect)
            {
                var newState = _ship.Stats.CanReflect;
                ReflectEffect.SetActive(newState);
                oldReflectState = newState;
            }
        }
    }

    void OnDestroy()
    {
        Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    }
}
