using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    private Vector3 _step;
    private GameObject _source;
    private GameObject _target;
    private Damage _damage;
    private TurnState _targetState;
    private bool Destroyed = false;

    public void SetupProjectile(GameObject source, GameObject target, Damage damage)
    {
        _source = source;
        _target = target;
        _damage = damage;
        _targetState = GameManager.instance.TurnState;

        _step = new Vector3(target.transform.position.x, 0f, 0f);

        GameManager.instance.TurnEnd += MoveInvoke;

        StartCoroutine(Move());
    }

    private void MoveInvoke(TurnState e)
    {
        if (e == _targetState)
        {
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        var target = transform.position + _step;
        Debug.Log($"Moved, position: {transform.position}, step: {_step} target: {transform.position + _step}");

        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 7f * Time.deltaTime);
            yield return 0;
        }

        if (transform.position == _target.transform.position)
        {
            Destroy(this);
            var targetShip = _target.GetComponent<Ship>();

            targetShip.TakeDamage(_damage);

            if (!targetShip.Destroyed && targetShip.Stats.CanReflect)
            {
                targetShip.Reflect(_damage, _source.GetComponent<Ship>());
            }
            else GameManager.instance.GameState = GameState.NoAction;
        }
        else GameManager.instance.GameState = GameState.NoAction;
    }

    private void OnDestroy()
    {
        GameManager.instance.TurnEnd -= MoveInvoke;
    }
}
