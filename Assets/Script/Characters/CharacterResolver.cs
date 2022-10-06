using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterResolver : MonoBehaviour
{
    public CharacterSO[] SwappableCharacters;
    public SpriteLibrary resolver;

    private void Awake()
    {
     if (resolver == null)
        {
            resolver = GetComponent<SpriteLibrary>();
        }
    }

    CharacterSO CurrentCharacter;
    public CharacterSO GetCurrentCharacter()
    {
        return CurrentCharacter;
    }
    public void ChangeCharacter(int nCharacter)
    {
        CurrentCharacter = SwappableCharacters[nCharacter % SwappableCharacters.Length];
        UpdateVisuals();
    }
    void UpdateVisuals()
    {
        resolver.spriteLibraryAsset = CurrentCharacter.spriteLibraryAsset;
    }
}
