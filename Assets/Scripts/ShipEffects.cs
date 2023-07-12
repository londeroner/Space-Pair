using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ShipEffects : MonoBehaviour
{
    public GameObject ShieldEffect;
    public GameObject ReflectEffect;

    public GameObject ExplosionPrefab;

    public Image HPBar;
    public Image EnergyBar;
    public TMP_Text ESText;
    public TMP_Text ArmourText;

    public GameObject NoEnergyText;
    [HideInInspector] public Vector3 NoEnergyTextStart;

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
        NoEnergyText.transform.rotation = Camera.main.transform.rotation;
        NoEnergyTextStart = NoEnergyText.transform.position;
    }

    void Update()
    {
        _updateCounter++;

        if (_updateCounter >= _targetCounter)
        {
            _updateCounter = 0;

            ESText.text = _ship.Stats.ES.ToString();
            ArmourText.text = _ship.Stats.Armour.ToString();
            HPBar.fillAmount = _ship.Stats.CurrentHP / _ship.Stats.MaxHP;
            EnergyBar.fillAmount = _ship.Stats.CurrentEnergy / _ship.Stats.MaxEnergy;

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

    public IEnumerator ShowNoEnergy()
    {
        NoEnergyText.transform.position = NoEnergyTextStart;
        NoEnergyText.SetActive(true);
        var targetPosition = new Vector3(NoEnergyTextStart.x, NoEnergyTextStart.y + 3f, NoEnergyTextStart.z);

        while (NoEnergyText.transform.position != targetPosition)
        {
            NoEnergyText.transform.position = Vector3.MoveTowards(NoEnergyText.transform.position, targetPosition, 1f * Time.deltaTime);
            yield return 0;
        }

        NoEnergyText.SetActive(false);
    }

    void OnDestroy()
    {
        Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    }
}
