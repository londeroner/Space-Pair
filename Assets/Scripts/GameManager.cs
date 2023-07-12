using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Ship PlayerShip;
    public Ship EnemyShip;

    public GameObject BulletPrefab;

    [HideInInspector]
    public RevealedState PuzzleRevealedState = RevealedState.NoRevealed;
    [HideInInspector]
    public GameState GameState = GameState.NoAction;
    [HideInInspector]
    public TurnState TurnState = TurnState.PlayerTurn;

    [HideInInspector]
    public bool IsGameFinished = false;

    public int FirstRevealedPic = -1;
    public int SecondRevealedPic = -1;
    public int RevealedPicNumber = 0;

    public event Action TurnEnd;
    public event Action BattleStarted;

    private Picture _firstReveal;
    private Picture _secondReveal;

    private int MoveAnimationCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        BattleStarted.Invoke();
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

            GameState = GameState.AnimationInProgress;
            if (_firstReveal.ComparativeHash == _secondReveal.ComparativeHash)
            {
                StartCoroutine(MoveFoundPic(_firstReveal, _secondReveal));
            }
            else
            {
                _firstReveal.FlipBack();
                _secondReveal.FlipBack();
            }

            _firstReveal = null;
            _secondReveal = null;
            PuzzleRevealedState = RevealedState.NoRevealed;

            if (TurnState == TurnState.PlayerTurn)
                TurnState = TurnState.AITurn;
            else
            {
                TurnState = TurnState.PlayerTurn;
                TurnEnd.Invoke();
            }
        }
    }

    private IEnumerator MoveFoundPic(Picture pic1, Picture pic2)
    {
        var source = TurnState == TurnState.PlayerTurn ? PlayerShip : EnemyShip;
        var target = TurnState == TurnState.PlayerTurn ? EnemyShip : PlayerShip;
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(MovePicToTarget(pic1, source, target));
        StartCoroutine(MovePicToTarget(pic2, source, target));
    }

    private IEnumerator MovePicToTarget(Picture pic, Ship source, Ship target)
    {
        while (pic.transform.position != source.transform.position)
        {
            pic.transform.position = Vector3.MoveTowards(pic.transform.position, source.transform.position, 7f * Time.deltaTime);
            yield return 0;
        }

        pic.Disable();
        MoveAnimationCount++;

        if (MoveAnimationCount == 2)
        {
            MoveAnimationCount = 0;

            PairAction(source, target, pic.PictureContent);
        }
    }

    private void PairAction(Ship source, Ship target, PictureContent pictureContent)
    {
        switch (pictureContent)
        {
            case PictureContent.Attack:
                StartCoroutine(source.Attack(target, source.CalculateDamage()));
                return;
            case PictureContent.Defence:
                source.Defense();
                return;
            case PictureContent.Resource:
                return;
            case PictureContent.PassiveUtility:
                return;
            case PictureContent.ActiveUtility:
                return;
        }
    }
}

public enum RevealedState
{
    NoRevealed = 0,
    OneRevealed = 1,
    TwoRevealed = 2
}

public enum GameState
{
    NoAction = 0,
    AnimationInProgress = 1
}

public enum TurnState
{
    PlayerTurn = 0,
    AITurn = 1
}