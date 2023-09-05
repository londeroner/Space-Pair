using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPicture : MonoBehaviour
{
    [HideInInspector] public bool Revealed = false;

    public PictureContent PictureContent;

    public event Action OnRemove;

    public Image FrontImage;
    public Image BackImage;

    public static int RevealedCount = 0;

    private static int _flippedCount;

    void Start()
    {
        OnRemove += () =>
        {
            TutorialAI.instance.Pictures.Remove(this);
        };
    }

    public void PictureClick()
    {
        if (!Revealed && TutorialGameManager.instance.TurnState == TurnState.PlayerTurn && RevealedCount < 2)
        {
            RevealedCount++;
            Flip();

            if (TutorialManager.instance.TutorialSteps == TutorialSteps.AsteroidSecondPicture)
            {
                TutorialManager.instance.TutorialStep();
            }
            else if (TutorialManager.instance.TutorialSteps == TutorialSteps.AsteroidCollect)
            {
                TutorialManager.instance.TutorialStep();
            }
        }
    }

    public void Flip()
    {
        Revealed = true;
        TutorialGameManager.instance.RevealPicture(this);
        StartCoroutine(LoopRotation(false));

        GetComponent<AudioSource>().Play();
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
                FrontImage.fillAmount -= 3f * Time.deltaTime;
                yield return null;
            }

            _flippedCount++;

            if (_flippedCount == 2)
            {
                TutorialGameManager.instance.GameState = GameState.NoAction;
                TutorialGameManager.instance.EndTurn();
                _flippedCount = 0;
            }
        }
        else
        {
            while (FrontImage.fillAmount < 1)
            {
                FrontImage.fillAmount += 3f * Time.deltaTime;
                yield return null;
            }

        }
    }

    public IEnumerator PictureAppearance()
    {
        while (BackImage.color.a < 1)
        {
            BackImage.color = new Color(BackImage.color.r, BackImage.color.g, BackImage.color.b, BackImage.color.a + 1f * Time.deltaTime);
            yield return null;
        }
    }

    public void Disable()
    {
        OnRemove?.Invoke();
        Destroy(gameObject);
    }
}