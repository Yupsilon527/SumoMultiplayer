using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    float initTime = 0;
    float appearTime = 0;
    float fadeTime = 0;
    Color myColor = Color.white;

    float alpha = 0;
    private void Awake()
    {
        sprites.Add(GetComponent<SpriteRenderer>());
        sprites.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }
    public void ChangeColor(Color c)
    {
        myColor = c;
        alpha = appearTime > 0 ? 0 : 1;
        foreach (SpriteRenderer renderer in sprites)
        {
            renderer.color = new Color(myColor.r, myColor.g,myColor.b, alpha * myColor.a);
        }
    }
    private void Update()
    {
        Color tempC = myColor;

        if (Time.time - initTime < appearTime)
        {
            alpha = 1 - (Time.time - initTime) / appearTime;
        }
        else
        {
            alpha = 1 - (Time.time - initTime - appearTime) / fadeTime;
        }
        tempC.a = myColor.a * alpha;

        foreach (SpriteRenderer renderer in sprites)
        {
            renderer.color = tempC;
        }
        if ( Time.time - initTime > appearTime + fadeTime)
        {
            Disable();
        }
    }
    public void CopyEntity(GameObject entity)
    {
        int i = 0;
        transform.position = entity.transform.position;
        transform.rotation = entity.transform.rotation;
        transform.localScale = entity.transform.localScale;
        if (entity.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
        {
            sprites[i].sprite = renderer.sprite;
            sprites[i].flipX = renderer.flipX;
            sprites[i].enabled = renderer.enabled;
            i++;
                }
        foreach (SpriteRenderer childsprite in entity.GetComponentsInChildren<SpriteRenderer>())
        {
                sprites[i].transform.position = childsprite.transform.position;
                sprites[i].transform.rotation = childsprite.transform.rotation;
                sprites[i].transform.localScale = childsprite.transform.localScale;
                sprites[i].sprite = childsprite.sprite;
                sprites[i].flipX = childsprite.flipX;
                sprites[i].enabled = childsprite.enabled;
                i++;
            
        }
    }
    public void Expire(float appearduration, float trailduration)
    {
        initTime = Time.time;
        appearTime = appearduration;
        fadeTime = trailduration;
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        foreach (SpriteRenderer renderer in sprites)
        {
            renderer.enabled = false;
        }
    }
}
