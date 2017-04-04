﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantaChessEngine
{
    public class MoveFactory
    {
        public NormalMove MakeNormalMove(char movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank, char capturedPiece)
        {
            return new NormalMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
        }

        public EnPassantCaptureMove MakeEnPassantCaptureMove(char movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank, char capturedPiece)
        {
            return new EnPassantCaptureMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
        }

        public CastlingMove MakeCastlingMove(CastlingType castlingType)
        {
            return new CastlingMove(castlingType);
        }

        public NoLegalMove MakeNoLegalMove()
        {
            return new NoLegalMove();
        }


        public MoveBase GetCorrectMove(Board board, string moveStringUser) // input is like "e2e4"
        {
            char movingPiece;
            char capturedPiece;
            int sourceFile = 0;
            int sourceRank = 0;
            int targetFile = 0;
            int targetRank = 0;
            bool enPassant = false;

            if (MoveBase.IsCorrectMove(moveStringUser))
            {
                GetPositions(moveStringUser, out sourceFile, out sourceRank, out targetFile, out targetRank);
                movingPiece = board.GetPiece(sourceFile, sourceRank);

                // set captured Piece
                if (IsEnPassantCapture(board, sourceFile, sourceRank, targetFile, targetRank))
                {
                    capturedPiece = board.GetColor(sourceFile, sourceRank) == Definitions.ChessColor.White
                        ? Definitions.PAWN.ToString().ToLower()[0]
                        : Definitions.PAWN.ToString().ToUpper()[0];
                    enPassant = true;
                }
                else
                {
                    capturedPiece = board.GetPiece(targetFile, targetRank);
                }

                if (enPassant)
                {
                    return new EnPassantCaptureMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
                }

                if (IsWhiteKingSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.WhiteKingSide);
                }

                if (IsWhiteQueenSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.WhiteQueenSide);
                }

                if (IsBlackKingSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.BlackKingSide);
                }

                if (IsBlackQueenSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.BlackQueenSide);
                }

                return new NormalMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
            }

            return null;
        }
        

        private bool IsEnPassantCapture(Board board, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return board.GetColor(targetFile, targetRank) == Definitions.ChessColor.Empty &&
                   board.History.LastEnPassantFile == targetFile && 
                   board.History.LastEnPassantRank == targetRank;
        }

        public void GetPositions(string moveString, out int sourceFile, out int sourceRank, out int targetFile, out int targetRank)
        {
            if (moveString.Length >= 4)
            {
                sourceFile = Helper.FileCharToFile(moveString[0]);
                sourceRank = int.Parse(moveString[1].ToString());
                targetFile = Helper.FileCharToFile(moveString[2]);
                targetRank = int.Parse(moveString[3].ToString());
            }
            else
            {
                sourceFile = 0;
                sourceRank = 0;
                targetFile = 0;
                targetRank = 0;
            }
        }

        private bool IsWhiteKingSideCastling(char movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece == Definitions.KING.ToString().ToUpper()[0] &&
                    sourceFile == Helper.FileCharToFile('e') && sourceRank == 1 &&
                    targetFile == Helper.FileCharToFile('g') && targetRank == 1;
        }

        private bool IsWhiteQueenSideCastling(char movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece == Definitions.KING.ToString().ToUpper()[0] &&
                   sourceFile == Helper.FileCharToFile('e') && sourceRank == 1 &&
                   targetFile == Helper.FileCharToFile('c') && targetRank == 1;
        }

        private bool IsBlackKingSideCastling(char movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece == Definitions.KING.ToString().ToLower()[0] &&
                sourceFile == Helper.FileCharToFile('e') && sourceRank == 8 &&
                targetFile == Helper.FileCharToFile('g') && targetRank == 8;
        }

        private bool IsBlackQueenSideCastling(char movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece == Definitions.KING.ToString().ToLower()[0] &&
                   sourceFile == Helper.FileCharToFile('e') && sourceRank == 8 &&
                   targetFile == Helper.FileCharToFile('c') && targetRank == 8;
        }
    }
}