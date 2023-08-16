using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Picture : MonoBehaviour
{
    [HideInInspector] public bool MaterialSet = false;

    [HideInInspector] public bool Revealed = false;
    [HideInInspector] public PictureManager PictureManager;

    [HideInInspector] public string ComparativeHash;

    [HideInInspector] public PictureContent PictureContent;

    public event Action OnRemove;

    public Image BackImage;
    public Image FrontImage;

    private Quaternion _currentRotation;

    private static int _flippedCount;

    void Start()
    {
        _currentRotation = gameObject.transform.rotation;
    }

    public void PictureClick()
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
        StartCoroutine(LoopRotation(false));
    }

    public void FlipBack()
    {
        Revealed = false;
        StartCoroutine(LoopRotation(true));
    }

    private IEnumerator LoopRotation(bool flipBack)
    {
        if (flipBack)
        {
            yield return new WaitForSeconds(1.5f);
            while (FrontImage.fillAmount > 0)
            {
                FrontImage.fillAmount -= 0.01f;
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
            while (FrontImage.fillAmount < 1)
            {
                FrontImage.fillAmount += 0.01f;
                yield return null;
            }

        }

        gameObject.GetComponent<Transform>().rotation = _currentRotation;
    }

    public void SetFirstSprite(Sprite sprite)
    {
        BackImage.sprite = sprite;
    }

    public void SetSecondSprite(Sprite sprite, PictureContent content)
    {
        FrontImage.sprite = sprite;

        PictureContent = content;

        ///////////////////////////TEST
        ComparativeHash = sprite.ToString();

        MaterialSet = true;
    }

    public void Disable()
    {
        OnRemove.Invoke();
        Destroy(gameObject);
    }
}