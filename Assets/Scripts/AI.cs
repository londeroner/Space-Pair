using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    public static AI instance;

    [Range(0, 100)]
    [Tooltip("Chance to use memory for open pair")]
    public int UseMemoryChance = 50;

    [Tooltip("Count of turns before AI forgot opened card")]
    public int MemorySize = 3;

    public Dictionary<Picture, int> _memory = new Dictionary<Picture, int>();

    private PictureManager _pictureManager;

    private bool MakeTurn = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        _pictureManager = GetComponent<PictureManager>();

        GameManager.instance.TurnEnd += (e) =>
        {
            if (e == TurnState.AITurn)
            {
                foreach (var key in _memory.Keys.ToList())
                {
                    _memory[key]--;

                    if (_memory[key] <= 0)
                    {
                        _memory.Remove(key);
                    }
                }
                MakeTurn = false;
            }
        };
    }

    void Update()
    {
        if (GameManager.instance.TurnState == TurnState.AITurn && GameManager.instance.GameState == GameState.NoAction && !MakeTurn && !GameManager.instance.IsGameFinished)
        {
            MakeTurn = true;
            StartCoroutine(FlipCards());
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
        yield return new WaitForSeconds(0.3f);

        var _flippedIndex = Random.Range(0, _pictureManager.PictureList.Count);
        _pictureManager.PictureList[_flippedIndex].Flip();

        yield return new WaitForSeconds(1.5f);

        var chanceToMemory = Random.Range(0, 100);
        bool found = false;
        if (chanceToMemory < UseMemoryChance)
        {
            Picture secondPicture = GetFromMemory(_pictureManager.PictureList[_flippedIndex]);

            if (secondPicture is not null)
            {
                found = true;
                secondPicture.Flip();
            }
        }

        if (!found)
        {
            var _secondIndex = Random.Range(0, _pictureManager.PictureList.Count);

            while (_secondIndex == _flippedIndex)
            {
                _secondIndex = Random.Range(0, _pictureManager.PictureList.Count);
            }

            _pictureManager.PictureList[_secondIndex].Flip();
        }
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
