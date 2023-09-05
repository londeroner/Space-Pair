using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialAI : MonoBehaviour
{
    public static TutorialAI instance;

    [Range(0, 100)]
    [Tooltip("Chance to use memory for open pair")]
    public int UseMemoryChance = 50;

    [Tooltip("Count of turns before AI forgot opened card")]
    public int MemorySize = 3;

    public Dictionary<Picture, int> _memory = new Dictionary<Picture, int>();

    public bool IsAIDisabled = false;

    public List<TutorialPicture> Pictures = new List<TutorialPicture>();

    private bool MakeTurn = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        TutorialGameManager.instance.TurnEnd += (e) => { MakeTurn = false; };
    }

    void Update()
    {
        if (TutorialGameManager.instance.TurnState == TurnState.AITurn && TutorialGameManager.instance.GameState == GameState.NoAction && !MakeTurn)
        {
            if (!IsAIDisabled)
            {
                MakeTurn = true;
                StartCoroutine(FlipCards());
            }
            else
            {
                TutorialGameManager.instance.EndTurn();
            }
        }
    }

    public void AddToMemory(Picture pic)
    {
        if (!_memory.ContainsKey(pic))
        {
            _memory.Add(pic, MemorySize);
            pic.OnRemove += () => _memory.Remove(pic);
        }
    }

    private IEnumerator FlipCards()
    {
        if (Pictures.Count == 0)
        {
            TutorialGameManager.instance.EndTurn();
            yield break;
        }

        yield return new WaitForSeconds(0.3f);

        var _flippedIndex = Random.Range(0, Pictures.Count);
        Pictures[_flippedIndex].Flip();

        yield return new WaitForSeconds(1.5f);
        
        var _secondIndex = Random.Range(0, Pictures.Count);

        while (_secondIndex == _flippedIndex)
        {
            _secondIndex = Random.Range(0, Pictures.Count);
        }

        Pictures[_secondIndex].Flip();
    }

    private Picture GetFromMemory(Picture pic)
    {
        foreach (var key in _memory.Keys)
        {
            if (key != pic && key.PictureContent == pic.PictureContent)
                return key;
        }

        return null;
    }
}
