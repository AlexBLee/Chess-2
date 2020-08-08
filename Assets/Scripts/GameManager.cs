﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PENWriter PENWriter;
    public Stockfish stockfish;
    public ChessTimer chessTimer;
    public Board board;
    public bool whiteTurn = true;
    public bool check;
    public bool paused;
    public King kingInCheck;
    public PromotionPanel promotionPanel;
    public ResultPanel resultPanel;
    public int moveCounter;
    public int movesWithoutCaptures;
    public static bool whiteSide;
    public Transform blackSideCameraPos;
    public bool playerTurn;
    public bool playerControlled;

    public float whiteTimerValue = 300;
    public float blackTimerValue = 300;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // If the player starts as black, use alternate camera position
        if (!whiteSide)
        {
            playerTurn = false;
            Camera.main.transform.SetPositionAndRotation(blackSideCameraPos.position, blackSideCameraPos.rotation);
        }
        else
        {
            playerTurn = true;
        }

        promotionPanel.gameObject.SetActive(false);
        resultPanel.gameObject.SetActive(false);

    }

    private void Start() {

        // if (!playerTurn)
        // {
        //     MakeBotMove();
        // }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F))
        {
            MakeBotMove();
        }
    }

    public void FindWhiteMoves()
    {
        foreach (Piece piece in board.whitePieces)
        {
            if (!piece.pinned)
            {
                piece.FindMoveSet();
            }

        }
    }

    public void FindBlackMoves()
    {
        foreach (Piece piece in board.blackPieces)
        {
            if (!piece.pinned)
            {
                piece.FindMoveSet();
            }
        }
    }

    public void FindAllPossibleMoves()
    {
        FindWhiteMoves();
        FindBlackMoves();

        // King movesets are found at the end because if one king is called first in FindXMoves without the opposite
        // side piece's to have been updated, the king will only take the previous moves into consideration.
        // PS: may need to find a better way to write this.
        King k = (King)board.whitePieces.Find(x => x is King);
        k.FindMoveSet();

        King kx = (King)board.blackPieces.Find(x => x is King);
        kx.FindMoveSet();
    }

    public void SwitchSides()
    {
        whiteTurn = (whiteTurn == true) ? false : true;
        playerTurn = (playerTurn == true) ? false : true;

        moveCounter--;
        if (moveCounter % 2 == 0)
        {
            PENWriter.moveCount++;
            moveCounter = 2;
        }

        chessTimer.StartCountdown();

        PENWriter.enPassantTile = "-";

        // Resets checks and defended statuses as well.
        if (kingInCheck != null)
        {
            kingInCheck.checkLight.enabled = false;
            kingInCheck.check = false;
            kingInCheck.checkDefended = false;
            kingInCheck = null;
        }

        foreach (Piece piece in board.whitePieces)
        {
            piece.interactable = (whiteTurn == true) ? true : false;
            piece.defended = false;
            piece.pinned = false;
        }

        foreach (Piece piece in board.blackPieces)
        {
            piece.interactable = (whiteTurn == true) ? false : true;
            piece.defended = false;
            piece.pinned = false;
        }

    }

    public void SetGameState(bool state)
    {
        // If state is true -> resume game, if state is false -> pause game
        paused = !state;
    }

    public void CheckKingCheck()
    {
        // Assign the correct list
        List<Piece> pieceList = 
        (kingInCheck.render.sharedMaterial == board.pieceBlack) ? board.blackPieces : board.whitePieces;

        for (int i = 0; i < pieceList.Count; i++)
        {
            List<Vector2Int> tempCoors = new List<Vector2Int>();

            foreach (Vector2Int move in kingInCheck.line)
            {
                // If any moves that intersect with the piece are found, add them to the list
                if (pieceList[i].moves.Any(x => x == move))
                {
                    kingInCheck.checkDefended = true;
                    tempCoors.Add(move);
                }
            }

            // Change the move list for the pieces that found any intersecting moves
            // If there's nothing, it will give an empty list, as any piece that cant move in the way shouldn't be able to move.
            if (!(pieceList[i] is King))
            {
                pieceList[i].moves = tempCoors;
            }
            
        }
    }

    public void CheckForCheckMate()
    {
        if (kingInCheck.check && kingInCheck.moves.Count == 0 && !kingInCheck.checkDefended)
        {
            string side = (kingInCheck.render.sharedMaterial == board.pieceWhite) ? "Black " : "White ";
            ActivateGameOver(side + "wins by checkmate");
        }
        kingInCheck.line.Clear();
    }

    public void NextTurn()
    {
        if (!paused)
        {
            PENWriter.AddPositionToHistory();

            SwitchSides();
            FindAllPossibleMoves();

            if (kingInCheck != null)
            {
                CheckKingCheck();
                CheckForCheckMate();
            }

            CheckDraw();

            // if (!playerTurn)
            // {
            //     MakeBotMove();
            // }

        }
    }

    public void CheckDraw()
    {
        // If there are no more legal moves but king isn't in check..
        if (kingInCheck == null && (board.blackPieces.Where(x => x.moves.Count == 0).Count() == board.blackPieces.Count ||
                                    board.whitePieces.Where(x => x.moves.Count == 0).Count() == board.blackPieces.Count))
        {
            ActivateGameOver("Stalemate");
        }

        // If there are 3 identical positions at any point in the game..
        List<string> posHistory = PENWriter.positionHistory;
        if (posHistory.Where(x => x.Equals(posHistory[posHistory.Count - 1])).Count() == 3)
        {
            ActivateGameOver("Draw by repetition");
        }

        // If 50 complete moves without captures or pawn movement has happened..
        if (PENWriter.consecutivePieceMoves >= 100 && movesWithoutCaptures >= 100)
        {
            ActivateGameOver("Fifty move draw");
        }

    }

    public void MakeBotMove()
    {
        GameManager.instance.playerControlled = false;
        
        stockfish.GetBestMove(PENWriter.WritePosition());
        PENWriter.AddPositionToHistory();

        board.tiles[stockfish.startPos.x][stockfish.startPos.y].piece.MoveTo(board.tiles[stockfish.resultPos.x][stockfish.resultPos.y]);
        NextTurn();
    }

    public void ActivateGameOver(string text)
    {
        resultPanel.gameObject.SetActive(true);
        resultPanel.DisplayText(text);
    }

}
