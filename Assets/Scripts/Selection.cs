﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public Board board;
    public Piece selectedPiece;
    public bool pieceSelected;


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            if (pieceSelected)
            {
                MakeMove(hit.transform.GetComponent<Tile>());
            }
            else
            {
                SelectPiece(hit.transform.GetComponent<Tile>().piece);
            }
        }
    }

    public void SelectPiece(Piece piece)
    {
        if (piece != null && piece.interactable)
        {
            selectedPiece = piece;

            // Loop through each possible move that the piece can make
            foreach (Vector2Int move in selectedPiece.moves)
            {
                Tile tile = board.tiles[move.x][move.y];

                // If the piece can attack a piece, then turn it red
                if (tile.piece != null && (tile.piece.interactable != selectedPiece.interactable))
                {
                    board.SetTileColour(tile, board.pieceAttack);
                }
                // Otherwise colour it normally
                else
                {
                    board.SetTileColour(tile, board.availableMoveColour);
                }
            }

            pieceSelected = true;
        }
    }

    public void MakeMove(Tile selectedTile)
    {
        // If the clicked tile is possible for the piece to move to, move there
        if (selectedPiece.moves.Any(move => move == selectedTile.coordinates))
        {
            // Destroy the piece at that tile
            if (selectedTile.piece != null && selectedPiece.interactable != selectedTile.piece.interactable) 
            {
                // TODO: clear piece from board function
                if (selectedTile.piece.render.sharedMaterial == board.pieceBlack)
                {
                    board.blackPieces.Remove(selectedTile.piece);
                }
                else
                {
                    board.whitePieces.Remove(selectedTile.piece);
                }


                Destroy(selectedTile.piece.gameObject);
                
            }

            // Colour the board back to normal
            board.ResetPieceMoveTileColours(selectedPiece);
            selectedPiece.MoveTo(selectedTile);
            
            GameManager.instance.SwitchSides();
            GameManager.instance.FindAllPossibleMoves();
            
            if (GameManager.instance.kingInCheck != null)
            {
                GameManager.instance.CheckKingCheck();
            }
        }
        else
        {
            board.ResetPieceMoveTileColours(selectedPiece);
        }

        pieceSelected = false;
    }
}
