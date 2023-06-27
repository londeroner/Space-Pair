using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform PicFlightPosition;

    [HideInInspector]
    public RevealedState PuzzleRevealedState = RevealedState.NoRevealed;

    public int FirstRevealedPic = -1;
    public int SecondRevealedPic = -1;
    public int RevealedPicNumber = 0;

    private Picture _firstReveal;
    private Picture _secondReveal;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void RevealPicture(Picture picture)
    {
        if (PuzzleRevealedState == RevealedState.NoRevealed)
        {
            _firstReveal = picture;
            PuzzleRevealedState++;
        }
        else if (PuzzleRevealedState == RevealedState.OneRevealed)
        {
            _secondReveal = picture;

            if (_firstReveal.ComparativeHash == _secondReveal.ComparativeHash)
            {
                StartCoroutine(MoveFoundPic(_firstReveal));
                StartCoroutine(MoveFoundPic(_secondReveal));
            }
            else
            {
                _firstReveal.FlipBack();
                _secondReveal.FlipBack();
            }

            _firstReveal = null;
            _secondReveal = null;
            PuzzleRevealedState = RevealedState.NoRevealed;
        }
    }

    private IEnumerator MoveFoundPic(Picture pic)
    {
        yield return new WaitForSeconds(0.5f);
        while (pic.transform.position != PicFlightPosition.position)
        {
            pic.transform.position = Vector3.MoveTowards(pic.transform.position, PicFlightPosition.position, 7f * Time.deltaTime);
            yield return 0;
        }

        pic.Disable();
    }
}

public enum RevealedState
{
    NoRevealed = 0,
    OneRevealed = 1,
    TwoRevealed = 2
}