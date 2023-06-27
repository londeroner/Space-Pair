using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public Picture PicturePrefab;
    public Transform PicSpawnPosition;
    public Vector2 Offset = new Vector2(0.25f, 0.25f);

    public float AnimationSpeed = 7f;

    [HideInInspector]
    public List<Picture> PictureList;

    private int _columns;

    private List<Material> _materialList = new List<Material>();
    private List<string> _texturePaths = new List<string>();
    private Material _defaultMaterial;
    private string _defaultTexturePath;

    void Start()
    {
        LoadMaterials();

        _columns = (int)Mathf.Ceil(Mathf.Sqrt(GameSettings.instance.ObjectsNumber));

        SpawnPictureMesh(PicSpawnPosition.position, Offset, false);
        MovePosition(PicSpawnPosition.position, Offset);
    }

    private void LoadMaterials()
    {
        var materialFilePath = GameSettings.instance.GetMaterialDirectoryName();
        var textureFilePath = GameSettings.instance.GetTextureDirectoryName();
        const string matBaseName = "Pic";
        var firstMaterialName = "Back";

        for (var i = 1; i <= 20; i++)
        {
            var currentFilePath = materialFilePath + matBaseName + i;
            Material mat = Resources.Load(currentFilePath, typeof(Material)) as Material;
            _materialList.Add(mat);

            var currentTextureFilePath = textureFilePath + matBaseName + i;
            _texturePaths.Add(currentTextureFilePath);
        }

        _defaultTexturePath = textureFilePath + firstMaterialName;
        _defaultMaterial = Resources.Load(materialFilePath + firstMaterialName, typeof(Material)) as Material;
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
                }
            }
        }

        ApplyTextures();
    }

    private void ApplyTextures()
    {
        for (int i = 0; i < PictureList.Count; i++)
        {
            if (!PictureList[i].MaterialSet)
            {
                var rndMatIndex = Random.Range(0, _materialList.Count);
                var rndObjectIndex = Random.Range(i + 1, PictureList.Count);

                while (PictureList[rndObjectIndex].MaterialSet) rndObjectIndex = Random.Range(i + 1, PictureList.Count);

                SetMaterials(PictureList[i], rndMatIndex);
                SetMaterials(PictureList[rndObjectIndex], rndMatIndex);
            }
        }
    }

    private void SetMaterials(Picture pic, int matIndex)
    {
        pic.SetFirstMaterial(_defaultMaterial, _defaultTexturePath);
        pic.ApplyFirstMaterial();
        pic.SetSecondMaterial(_materialList[matIndex], _texturePaths[matIndex]);
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