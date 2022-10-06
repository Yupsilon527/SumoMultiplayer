using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageEmitter : MonoBehaviour
{
    private void Awake()
    {
        mCol = TrailColor;
    }
    void FixedUpdate()
    {
        SpawnTrailPart();
    }
    #region Speed Trails

    float lastTrail = 0;
    public Color TrailColor = Color.white;
    public float TrailInterval = .15f;
    public float TrailFailure = .1f;
    public float TrailAppearance = .05f;
    public float TrailDuration = .5f;
    public float TrailAlpha = 1;

    Color mCol = Color.white;
    public void SpawnTrailPart()
    {
        if (lastTrail > Time.time || TrailDuration + TrailAppearance < TrailFailure)
            return;

        mCol.a = TrailColor.a * TrailAlpha;

        SpecialEffectPooler.main.CreateTrailOnGameObject(gameObject, mCol, TrailAppearance, TrailDuration);
        lastTrail = Time.time + TrailInterval;

    }
    #endregion
}
