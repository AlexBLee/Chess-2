﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // to tell between white and black
    public static Board board;
    public bool playerOwned;
    public Renderer render;
    public List<Vector2Int> moves;
    public Vector2Int currentCoordinates;
    public int forwardDirection;
    
    private void Start() 
    {
        board = FindObjectOfType<Board>();
    }

    public void SetPieceColor(Material color)
    {
        render.material = color;
    }

    public void FindMoveSet()
    {
        FindLegalMoves();
        RemoveIllegalMoves();
    }

    public virtual void FindLegalMoves()
    {
    }

    public void CalculateMoves(int xIncrement, int yIncrement, bool singleJump)
    {
        int xStep = xIncrement;
        int yStep = yIncrement;
        int maxJump = 8;

        if (singleJump)
        {
            maxJump = 2;
        }
        else
        {
            maxJump = 8;
        }

        int inc = 1;
        while (inc < maxJump)
        {
            Vector2Int boardCoordPoint = 
            new Vector2Int(currentCoordinates.x + xStep, currentCoordinates.y + (yStep * forwardDirection));

            if (boardCoordPoint.x > 0 && boardCoordPoint.y > 0 && !IsPieceAtTile(boardCoordPoint))
            {
                moves.Add(boardCoordPoint);
            }
            else
            {
                break;
            }

            if (xStep < 0) { xStep -= Mathf.Abs(xIncrement); } else if (xStep > 0) { xStep += xIncrement;}
            if (yStep < 0) { yStep -= Mathf.Abs(yIncrement); } else if (yStep > 0) { yStep += yIncrement;}

            inc++;

        }
    }

    public void RemoveIllegalMoves()
    {
        moves.RemoveAll(tile => tile.x < 1 || tile.x > 8 || tile.y < 1 || tile.y > 8);

        for (int i = 0; i < moves.Count; i++)
        {
            board.tiles[(moves[i].x - 1) + (moves[i].y - 1) * 8].render.material = board.pieceWhite;
        }
    }

    public bool IsPieceAtTile(Vector2Int tile)
    {
        return board.tiles[(tile.x - 1) + (tile.y - 1) * 8].piece != null;
    }
}
