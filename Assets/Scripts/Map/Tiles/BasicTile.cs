using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTile : MapTile
{
    protected override void Start()
    {
        tileEnter += (() =>
        {
            transform.localScale = Vector3.one * 1.3f;
        });
        tileExit += (() =>
        {
            transform.localScale = Vector3.one * 0.9f;
        });
        tileClick += (() =>
        {
            TilePlacement.Instance.ClickTile(this);
        });

        AssignData(TileData.FindByType(Type), true);
    }

    public override void InteractWithAgent(AIAgent agent)
    {
        throw new System.NotImplementedException();
    }
}
