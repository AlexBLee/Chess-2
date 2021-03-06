﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class PieceSelection : MonoBehaviour
{
    private Board board;
    private Piece selectedPiece;
    private bool pieceSelected;

    private void Start() 
    {
        board = FindObjectOfType<Board>();
    }

    void Update()
    {
        // if (GameManager.instance.whiteTurn)
        // {
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
        // }
    }

    public void SelectPiece(Piece piece)
    {
        if (piece != null && piece.interactable && !GameManager.instance.paused)
        {
            if (PhotonNetwork.IsConnected && !piece.photonView.IsMine) { return; }

            selectedPiece = piece;

            // Loop through each possible move that the piece can make
            foreach (Vector2Int move in selectedPiece.moves)
            {
                Tile tile = board.tiles[move.x][move.y];

                // If the piece can attack a piece, then turn it red
                if (tile.piece != null && (tile.piece.render.sharedMaterial != selectedPiece.render.sharedMaterial))
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
            // Used for indicating if a player or bot has made the move;
            GameManager.instance.playerControlled = true;

            // Colour the board back to normal
            board.ResetPieceMoveTileColours(selectedPiece);
            
            if (PhotonNetwork.IsConnected)
            {
                Vector2 coor = new Vector2(selectedTile.coordinates.x, selectedTile.coordinates.y);
                selectedPiece.photonView.RPC("MoveTo", RpcTarget.All, coor);
            }
            else
            {
                selectedPiece.MoveTo(selectedTile);
            }

            StartCoroutine(AddDelay());
        }
        else
        {
            board.ResetPieceMoveTileColours(selectedPiece);
        }

        pieceSelected = false;
    }

    public IEnumerator AddDelay()
    {
        yield return new WaitForSeconds(0.25f);

        if (PhotonNetwork.IsConnected)
        {
            GameManager.instance.GetComponent<PhotonView>().RPC("NextTurn", RpcTarget.All);
        }
        else
        {
            GameManager.instance.NextTurn();
        }

    }

    
}
