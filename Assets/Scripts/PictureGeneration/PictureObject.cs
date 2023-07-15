using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PictureObject", menuName = "Pictures/PictureObject")]
public class PictureObject : ScriptableObject
{
    public Material PictureMaterial;
    public PictureContent PictureContent;
}

public enum PictureContent
{
    NoContent = 0,
    Attack = 1,
    Defense = 2,
    Resource = 3,
    Energy = 4,
    PassiveUtility = 5,
    ActiveUtility = 6
}