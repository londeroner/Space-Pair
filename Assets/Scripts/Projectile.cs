using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    public GameObject ExplosionPrefab;

    private Vector3 _step;
    private GameObject _source;
    private GameObject _target;
    private Vector3 _targetPosition;
    private Damage _damage;
    private TurnState _targetState;

    public void SetupProjectile(GameObject source, GameObject target, Damage damage, Vector3 targetPosition)
    {
        _source = source;
        _target = target;
        _targetPosition = targetPosition;
        _damage = damage;
        _targetState = GameManager.instance.TurnState;

        transform.LookAt(targetPosition);

        _step = new Vector3(_targetPosition.x, 0f, 0f);

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

        if (transform.position == _targetPosition)
        {
            var targetShip = _target.GetComponent<Ship>();

            targetShip.TakeDamage(_damage);

            if (!targetShip.Destroyed && targetShip.Stats.CanReflect)
            {
                targetShip.Reflect(_damage, _source.GetComponent<Ship>());
            }
            else GameManager.instance.GameState = GameState.NoAction;

            Destroy(gameObject);
        }
        else GameManager.instance.GameState = GameState.NoAction;
    }

    void OnDestroy()
    {
        Instantiate(ExplosionPrefab, new Vector3(_targetPosition.x, _targetPosition.y, _targetPosition.z - 0.15f), new Quaternion());
        GameManager.instance.TurnEnd -= MoveInvoke;
    }
}
