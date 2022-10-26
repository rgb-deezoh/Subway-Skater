using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpammer : MonoBehaviour
{
    public PieceType type;
    private Piece currentPiece;

    public void Spawn()
    {
        int amObj = 0;
        switch (type)
        {
            case PieceType.jump:
                amObj = LevelManager.Instance.jumps.Count;
                break;

            case PieceType.longblock:
                amObj = LevelManager.Instance.longblocks.Count;
                break;
            case PieceType.slide:
                amObj = LevelManager.Instance.slides.Count;
                break;
            case PieceType.ramp:
                amObj = LevelManager.Instance.ramps.Count;
                break;
        }


        currentPiece = LevelManager.Instance.GetPiece(type, Random.Range(0, amObj));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);
    }

    public void DeSpawn()
    {
        currentPiece.gameObject.SetActive(false);
    }


}
