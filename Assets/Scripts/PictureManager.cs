using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public Picture PicturePrefab;
    public Transform PicSpawnPosition;
    public Vector2 Offset = new Vector2(1.5f, 1.52f);

    public float AnimationSpeed = 7f;

    [HideInInspector]
    public List<Picture> PictureList;

    private int _columns;

    void Start()
    {
        _columns = GameSettings.instance.ColumnsNumber;
        int rows = GameSettings.instance.ObjectsNumber % _columns == 0 ? GameSettings.instance.ObjectsNumber / _columns : GameSettings.instance.ObjectsNumber / _columns + 1;

        SpawnPictureMesh(rows, PicSpawnPosition.position, Offset, false);
        MovePosition(rows, PicSpawnPosition.position, Offset);
    }

    void Update()
    {
    }

    private void SpawnPictureMesh(int rows, Vector2 Pos, Vector2 offset, bool scaleDown)
    {
        for (int col = 0; col < rows; col++)
        {
            for (int row = 0; row < _columns; row++)
            {
                if (PictureList.Count < GameSettings.instance.ObjectsNumber)
                {
                    var tempPicture = (Picture)Instantiate(PicturePrefab, PicSpawnPosition.position, PicSpawnPosition.transform.rotation);

                    tempPicture.name = $"{tempPicture.name}c{col}r{row}";
                    PictureList.Add(tempPicture);
                }
            }
        }
    }

    private void MovePosition(int rows, Vector2 pos, Vector2 offset)
    {
        var index = 0;
        for (var col = 0; col < rows; col++)
        {
            for (int row = 0; row < _columns; row++)
            {
                if (index < GameSettings.instance.ObjectsNumber)
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
