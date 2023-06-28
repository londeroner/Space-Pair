using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : MonoBehaviour
{
    public bool MaterialSet = false;

    [HideInInspector] public bool Revealed = false;
    [HideInInspector] public PictureManager PictureManager;

    public string ComparativeHash;

    private Material _firstMaterial;
    private Material _secondMaterial;

    private Quaternion _currentRotation;

    public PictureContent PictureContent;

    void Start()
    {
        _currentRotation = gameObject.transform.rotation;
    }

    void OnMouseDown()
    {
        if (!Revealed && GameManager.instance.TurnState == TurnState.PlayerTurn && !GameManager.instance.IsGameFinished)
        {
            Flip();
        }
    }

    public void Flip()
    {
        Revealed = true;
        GameManager.instance.RevealPicture(this);
        StartCoroutine(LoopRotation(45, false));
    }

    public void FlipBack()
    {
        Revealed = false;
        StartCoroutine(LoopRotation(45, true));
    }

    private IEnumerator LoopRotation(float angle, bool flipBack)
    {
        var rot = 0f;
        const float dir = 1f;
        const float rotSpeed = 180.0f;
        var startAngle = angle;
        var assigned = false;

        if (flipBack)
        {
            yield return new WaitForSeconds(1.5f);
            while (rot < angle)
            {
                var step = Time.deltaTime * rotSpeed;
                gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 2, 0) * step * dir);
                if (rot >= (startAngle -2) && !assigned)
                {
                    ApplyFirstMaterial();
                    assigned = true;
                }

                rot += (step * dir);
                yield return null;
            }

            Debug.Log("Animation stopped from loop");
            GameManager.instance.GameState = GameState.NoAction;
        }
        else
        {
            while (angle > 0)
            {
                var step = Time.deltaTime * rotSpeed;
                gameObject.GetComponent<Transform>().Rotate(new Vector3(0, 2, 0) * step * dir);
                angle -= (step * dir);
                yield return null;
            }

            ApplySecondMaterial();
        }

        gameObject.GetComponent<Transform>().rotation = _currentRotation;
    }

    public void SetFirstMaterial(Material mat, string texturePath)
    {
        _firstMaterial = mat;
        _firstMaterial.mainTexture = Resources.Load(texturePath, typeof(Texture2D)) as Texture2D;
    }

    public void SetSecondMaterial(Material mat, string texturePath)
    {
        _secondMaterial = mat;
        _secondMaterial.mainTexture = Resources.Load(texturePath, typeof(Texture2D)) as Texture2D;

        switch (mat.name)
        {
            case "Pic1":
                PictureContent = PictureContent.Attack;
                break;
            case "Pic2":
                PictureContent = PictureContent.Defence;
                break;
        }

        ///////////////////////////TEST
        ComparativeHash = _secondMaterial.ToString();

        MaterialSet = true;
    }

    public void ApplyFirstMaterial()
    {
        gameObject.GetComponent<Renderer>().material = _firstMaterial;
    }

    public void ApplySecondMaterial()
    {
        gameObject.GetComponent<Renderer>().material = _secondMaterial;
    }

    public void Disable()
    {
        PictureManager.PictureList.Remove(this);
        Destroy(gameObject);
    }
}

public enum PictureContent
{
    NoContent = 0,
    Attack = 1,
    Defence = 2,
    Resource = 3,
    PassiveUtility = 4,
    ActiveUtility = 5
}