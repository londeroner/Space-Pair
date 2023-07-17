using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    public bool MaterialSet = false;

    [HideInInspector] public bool Revealed = false;
    [HideInInspector] public PictureManager PictureManager;

    public string ComparativeHash;

    public PictureContent PictureContent;

    public event Action OnRemove;

    private Material _firstMaterial;
    private Material _secondMaterial;

    private Quaternion _currentRotation;

    private static int _flippedCount;

    void Start()
    {
        _currentRotation = gameObject.transform.rotation;
    }

    void OnMouseDown()
    {
        if (!Revealed && GameManager.instance.TurnState == TurnState.PlayerTurn && !GameManager.instance.IsGameFinished)
        {
            Flip();
        }
    }

    public void Flip()
    {
        Revealed = true;
        AI.instance.AddToMemory(this);
        GameManager.instance.RevealPicture(this);
        StartCoroutine(LoopRotation(45, false));
    }

    public void FlipBack()
    {
        Revealed = false;
        StartCoroutine(LoopRotation(45, true));
    }

    private IEnumerator LoopRotation(float angle, bool flipBack)
    {
        var rot = 0f;
        const float dir = 1f;
        const float rotSpeed = 180.0f;
        var startAngle = angle;
        var assigned = false;

        if (flipBack)
        {
            yield return new WaitForSeconds(1.5f);
            while (rot < angle)
            {
                var step = Time.deltaTime * rotSpeed;
                gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 2, 0) * step * dir);
                if (rot >= (startAngle -2) && !assigned)
                {
                    ApplyFirstMaterial();
                    assigned = true;
                }

                rot += (step * dir);
                yield return null;
            }

            _flippedCount++;

            if (_flippedCount == 2)
            {
                GameManager.instance.GameState = GameState.NoAction;
                GameManager.instance.EndTurn();
                _flippedCount = 0;
            }
        }
        else
        {
            while (angle > 0)
            {
                var step = Time.deltaTime * rotSpeed;
                gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 2, 0) * step * dir);
                angle -= (step * dir);
                yield return null;
            }

            ApplySecondMaterial();
        }

        gameObject.GetComponent<Transform>().rotation = _currentRotation;
    }

    public void SetFirstMaterial(Material mat)
    {
        _firstMaterial = mat;
    }

    public void SetSecondMaterial(Material mat, PictureContent content)
    {
        _secondMaterial = mat;

        PictureContent = content;

        ///////////////////////////TEST
        ComparativeHash = _secondMaterial.ToString();

        MaterialSet = true;
    }

    public void ApplyFirstMaterial()
    {
        gameObject.GetComponent<Renderer>().material = _firstMaterial;
    }

    public void ApplySecondMaterial()
    {
        gameObject.GetComponent<Renderer>().material = _secondMaterial;
    }

    public void Disable()
    {
        OnRemove.Invoke();
        Destroy(gameObject);
    }
}