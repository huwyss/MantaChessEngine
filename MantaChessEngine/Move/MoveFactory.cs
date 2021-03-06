﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantaChessEngine
{
    public class MoveFactory
    {
        public static NormalMove MakeNormalMove(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank, Piece capturedPiece)
        {
            return new NormalMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
        }

        public static EnPassantCaptureMove MakeEnPassantCaptureMove(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank, Piece capturedPiece)
        {
            return new EnPassantCaptureMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
        }

        public static PromotionMove MakePromotionMove(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank, Piece capturedPiece)
        {
            return new PromotionMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece);
        }

        public static CastlingMove MakeCastlingMove(CastlingType castlingType, Piece king)
        {
            return new CastlingMove(castlingType, king);
        }

        public NoLegalMove MakeNoLegalMove()
        {
            return new NoLegalMove();
        }


        public IMove MakeCorrectMove(Board board, string moveStringUser) // input is like "e2e4"
        {
            Piece movingPiece;
            Piece capturedPiece;
            int sourceFile = 0;
            int sourceRank = 0;
            int targetFile = 0;
            int targetRank = 0;
            bool enPassant = false;

            if (Helper.IsCorrectMove(moveStringUser))
            {
                GetPositions(moveStringUser, out sourceFile, out sourceRank, out targetFile, out targetRank);
                movingPiece = board.GetPiece(sourceFile, sourceRank);

                // set captured Piece
                if (IsEnPassantCapture(board, sourceFile, sourceRank, targetFile, targetRank))
                {
                    capturedPiece = board.GetColor(sourceFile, sourceRank) == Definitions.ChessColor.White
                        ? board.GetPiece(targetFile, targetRank - 1)
                        : board.GetPiece(targetFile, targetRank + 1);
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
                    return new CastlingMove(CastlingType.WhiteKingSide, movingPiece);
                }

                if (IsWhiteQueenSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.WhiteQueenSide, movingPiece);
                }

                if (IsBlackKingSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.BlackKingSide, movingPiece);
                }

                if (IsBlackQueenSideCastling(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new CastlingMove(CastlingType.BlackQueenSide, movingPiece);
                }

                if (IsPromotion(movingPiece, sourceFile, sourceRank, targetFile, targetRank))
                {
                    return new PromotionMove(movingPiece, sourceFile, sourceRank, targetFile, targetRank, null);
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

        private bool IsWhiteKingSideCastling(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece is King && movingPiece.Color == Definitions.ChessColor.White &&
                    sourceFile == Helper.FileCharToFile('e') && sourceRank == 1 &&
                    targetFile == Helper.FileCharToFile('g') && targetRank == 1;
        }

        private bool IsWhiteQueenSideCastling(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece is King && movingPiece.Color == Definitions.ChessColor.White &&
                   sourceFile == Helper.FileCharToFile('e') && sourceRank == 1 &&
                   targetFile == Helper.FileCharToFile('c') && targetRank == 1;
        }

        private bool IsBlackKingSideCastling(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece is King && movingPiece.Color == Definitions.ChessColor.Black &&
                sourceFile == Helper.FileCharToFile('e') && sourceRank == 8 &&
                targetFile == Helper.FileCharToFile('g') && targetRank == 8;
        }

        private bool IsBlackQueenSideCastling(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return movingPiece is King && movingPiece.Color == Definitions.ChessColor.Black &&
                   sourceFile == Helper.FileCharToFile('e') && sourceRank == 8 &&
                   targetFile == Helper.FileCharToFile('c') && targetRank == 8;
        }

        private bool IsPromotion(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank)
        {
            return ((movingPiece is Pawn && movingPiece.Color == Definitions.ChessColor.White && targetRank == 8) || // white promotion
                    (movingPiece is Pawn && movingPiece.Color == Definitions.ChessColor.Black && targetRank == 1));  // black promotion
        }
    }
}
