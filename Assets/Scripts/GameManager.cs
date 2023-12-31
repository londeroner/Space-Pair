using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Ship PlayerShip;
    public Ship EnemyShip;
    public Transform EnemyShipPosition;

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

    public event Action<TurnState> TurnEnd;
    public event Action BattleStarted;

    private Picture _firstReveal;
    private Picture _secondReveal;

    private int MoveAnimationCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        BattleStarted += () => { SpawnNewEnemy(EnemyShipPosition.position, Quaternion.Euler(0, -180, 0)); };
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
        }
    }

    public void EndTurn()
    {
        Picture.RevealedCount = 0;
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
        EnemyShip = go.GetComponent<Ship>();
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
        while (pic.transform.position != source.SpawnShipPoint.position)
        {
            pic.transform.position = Vector3.MoveTowards(pic.transform.position, source.SpawnShipPoint.position, 7f * Time.deltaTime);
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

    private void PairAction(Ship source, Ship target, PictureContent pictureContent)
    {
        if (PictureManager.instance.PictureList.Count == 0)
        {
            PictureManager.instance.GenerateField();
        }

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
                source.AddArmour(GameSettings.instance.ArmourFromTrash);
                break;
        }

        GameState = GameState.NoAction;
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