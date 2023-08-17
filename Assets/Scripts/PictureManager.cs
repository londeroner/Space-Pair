using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public static PictureManager instance;

    public Picture PicturePrefab;
    public Transform PicSpawnPosition;
    public Vector2 Offset = new Vector2(0.25f, 0.25f);

    public float AnimationSpeed = 7f;

    [HideInInspector]
    public List<Picture> PictureList;

    private int _columns;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _columns = (int)Mathf.Ceil(Mathf.Sqrt(GameSettings.instance.ObjectsNumber));

        GenerateField();
    }

    public void GenerateField()
    {
        SpawnPictureMesh(PicSpawnPosition.position, Offset, false);
        MovePosition(PicSpawnPosition.position, Offset);
    }

    private void SpawnPictureMesh(Vector2 Pos, Vector2 offset, bool scaleDown)
    {
        for (int col = 0; col < _columns; col++)
        {
            for (int row = 0; row < _columns; row++)
            {
                if (PictureList.Count < GameSettings.instance.ObjectsNumber)
                {
                    var tempPicture = Instantiate(PicturePrefab, PicSpawnPosition.position, PicturePrefab.transform.rotation);

                    tempPicture.PictureManager = this;
                    tempPicture.name = $"{tempPicture.name}c{col}r{row}";
                    PictureList.Add(tempPicture);
                    tempPicture.OnRemove += () => PictureList.Remove(tempPicture);
                }
            }
        }

        SetupPictures();
    }

    private void SetupPictures()
    {
        Dictionary<PictureObject, (int, int)> dict = new Dictionary<PictureObject, (int, int)>();
        dict.InitDictionary(GameSettings.instance.SpawnObjects);

        for (int i = 0; i < PictureList.Count; i++)
        {
            if (!PictureList[i].MaterialSet)
            {
                var spawnObjects = GameSettings.instance.SpawnObjects;

                var rndMatIndex = Random.Range(0, spawnObjects.Count);

                if (!dict.IsDictionaryFilled())
                    while (!dict.IsIndexAvailable(spawnObjects[rndMatIndex].PictureObject)) rndMatIndex = Random.Range(0, spawnObjects.Count);

                dict.IncDictionary(spawnObjects[rndMatIndex].PictureObject);

                var rndObjectIndex = Random.Range(i + 1, PictureList.Count);

                while (PictureList[rndObjectIndex].MaterialSet) rndObjectIndex = Random.Range(i + 1, PictureList.Count);

                SetSprites(PictureList[i], spawnObjects[rndMatIndex].PictureObject.PictureSprite, spawnObjects[rndMatIndex].PictureObject.PictureContent);
                SetSprites(PictureList[rndObjectIndex], spawnObjects[rndMatIndex].PictureObject.PictureSprite, spawnObjects[rndMatIndex].PictureObject.PictureContent);
            }
        }
    }

    private void SetSprites(Picture pic, Sprite sprite, PictureContent content)
    {
        pic.SetFirstSprite(GameSettings.instance.BackSprite);
        pic.SetSecondSprite(sprite, content);
    }

    private void MovePosition(Vector2 pos, Vector2 offset)
    {
        if (_columns % 2 == 1)
        {
            Move((_columns / 2) * (-1), (_columns / 2) + 1, pos, offset);
        }
        else
        {
            Move(_columns / 2 * (-1) + 0.5f, _columns / 2 + 0.5f, pos, offset);
        }
    }

    private void Move(float start, float finish, Vector2 pos, Vector2 offset, bool HaveCenter = false)
    {
        var index = 0;
        for (float col = start; col < finish; col++)
        {
            for (float row = start; row < finish; row++)
            {
                if (!HaveCenter && !(col == 0 && row == 0))
                {
                    var targetPosition = new Vector3((pos.x + (offset.x * row)), (pos.y - (offset.y * col)), 0.0f);
                    StartCoroutine(MoveToPosition(targetPosition, PictureList[index]));
                    index++;
                }
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, Picture obj)
    {
        while(obj.transform.position != target)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, AnimationSpeed * Time.deltaTime);
            yield return 0;
        }
    }
}