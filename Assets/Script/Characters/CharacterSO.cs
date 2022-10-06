using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "New Character", menuName = "Custom/Character")]
public class CharacterSO : ScriptableObject
{
    public Sprite characterIcon;
    public SpriteLibraryAsset spriteLibraryAsset;
}
