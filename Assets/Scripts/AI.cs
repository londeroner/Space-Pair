using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public bool MakeTurn = false;

    private int _flippedIndex;
    private PictureManager _pictureManager;

    void Start()
    {
        _pictureManager = GetComponent<PictureManager>();
    }

    void Update()
    {
        if (GameManager.instance.TurnState == TurnState.AITurn && GameManager.instance.GameState == GameState.NoAction && !MakeTurn && !GameManager.instance.IsGameFinished)
        {
            MakeTurn = true;
            StartCoroutine(FlipCards());
        }
    }

    private IEnumerator FlipCards()
    {
        yield return new WaitForSeconds(0.3f);
        _flippedIndex = Random.Range(0, _pictureManager.PictureList.Count);

        _pictureManager.PictureList[_flippedIndex].Flip();
        var _secondIndex = Random.Range(0, _pictureManager.PictureList.Count);

        while (_secondIndex == _flippedIndex)
        {
            _secondIndex = Random.Range(0, _pictureManager.PictureList.Count);
        }
        yield return new WaitForSeconds(1.5f);

        _pictureManager.PictureList[_secondIndex].Flip();
    }
}
