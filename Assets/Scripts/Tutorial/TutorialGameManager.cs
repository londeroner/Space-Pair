using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameManager : MonoBehaviour
{
    public static TutorialGameManager instance;

    public TutorialShip PlayerShip;
    public TutorialShip EnemyShip;
    public Transform EnemyShipPosition;

    public GameObject BulletPrefab;

    [HideInInspector]
    public RevealedState PuzzleRevealedState = RevealedState.NoRevealed;
    [HideInInspector]
    public GameState GameState = GameState.NoAction;
    [HideInInspector]
    public TurnState TurnState = TurnState.PlayerTurn;

    public event Action<TurnState> TurnEnd;
    public event Action BattleStarted;

    private TutorialPicture _firstReveal;
    private TutorialPicture _secondReveal;

    private int MoveAnimationCount = 0;

    public int globalPictureCount = 8;

    public int CollectedOre = 0;
    public int CollectedCrystall = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        BattleStarted?.Invoke();
    }

    void Update()
    {
        if (globalPictureCount == 0)
        {
            TutorialManager.instance.TutorialStep();
            globalPictureCount = 8;
        }
        //else if ()
    }

    public void RevealPicture(TutorialPicture picture)
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
            if (_firstReveal.PictureContent == _secondReveal.PictureContent)
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
        }
    }

    public void EndTurn()
    {
        TutorialPicture.RevealedCount = 0;
        if (TurnState == TurnState.PlayerTurn)
        {
            TurnEnd?.Invoke(TurnState);
            TurnState = TurnState.AITurn;
        }
        else
        {
            TurnEnd?.Invoke(TurnState);
            TurnState = TurnState.PlayerTurn;
        }
    }

    public void SpawnNewEnemy(Vector3 oldPosition, Quaternion oldRotation)
    {
        var go = Instantiate(GameSettings.instance.EnemyShipPrefab, new Vector3(oldPosition.x + 5f, oldPosition.y, oldPosition.z), oldRotation);
        EnemyShip = go.GetComponent<TutorialShip>();
        go.GetComponent<Ship>().CalculateMaxEnergy();

        StartCoroutine(MoveEnemy(oldPosition));
    }

    private IEnumerator MoveEnemy(Vector3 target)
    {
        while (EnemyShip.transform.position != target)
        {
            EnemyShip.transform.position = Vector3.MoveTowards(EnemyShip.transform.position, target, 7f * Time.deltaTime);
            yield return 0;
        }
    }

    private IEnumerator MoveFoundPic(TutorialPicture pic1, TutorialPicture pic2)
    {
        var source = TurnState == TurnState.PlayerTurn ? PlayerShip : EnemyShip;
        var target = TurnState == TurnState.PlayerTurn ? EnemyShip : PlayerShip;
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(MovePicToTarget(pic1, source, target));
        StartCoroutine(MovePicToTarget(pic2, source, target));
    }

    private IEnumerator MovePicToTarget(TutorialPicture pic, TutorialShip source, TutorialShip target)
    {
        while (pic.transform.position != source.SpawnShipPoint.position)
        {
            pic.transform.position = Vector3.MoveTowards(pic.transform.position, source.SpawnShipPoint.position, 10f * Time.deltaTime);
            yield return 0;
        }

        pic.Disable();
        MoveAnimationCount++;

        if (MoveAnimationCount == 2)
        {
            MoveAnimationCount = 0;

            EndTurn();
            PairAction(source, target, pic.PictureContent);
        }
    }

    private void PairAction(TutorialShip source, TutorialShip target, PictureContent pictureContent)
    {
        globalPictureCount -= 2;
        //if (PictureManager.instance.PictureList.Count == 0)
        //{
        //    PictureManager.instance.GenerateField();
        //}

        switch (pictureContent)
        {
            case PictureContent.Attack:
                source.Attack(target, source.CalculateDamage());
                return;
            case PictureContent.Defense:
                source.Defense();
                break;
            case PictureContent.Resource:
                break;
            case PictureContent.PassiveUtility:
                break;
            case PictureContent.ActiveUtility:
                break;
            case PictureContent.NoContent:
                break;
            case PictureContent.Energy:
                source.RefillEnergy();
                break;
            case PictureContent.Trash:
                if (source == PlayerShip)
                {
                    CollectedOre+=2;
                }
                break;
            case PictureContent.Crystalls:
                if (source == PlayerShip)
                {
                    CollectedCrystall+=2;
                }
                break;
        }

        GameState = GameState.NoAction;
    }
}