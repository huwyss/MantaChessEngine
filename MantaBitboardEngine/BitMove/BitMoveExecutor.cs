﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantaBitboardEngine
{
    public class BitMoveExecutor
    {
        public void DoMove(BitMove bitMove, IBitBoard bitBoards)
        {
            bitBoards.RemovePiece(bitMove.FromSquare);
            if (bitMove.CapturedSquare != Square.NoSquare)
            {
                bitBoards.RemovePiece(bitMove.CapturedSquare);
            }

            if (bitMove.IsPromotionMove())
            {
                bitBoards.SetPiece(bitMove.MovingColor, bitMove.PromotionPiece, bitMove.ToSquare);
            }
            else
            {
                bitBoards.SetPiece(bitMove.MovingColor, bitMove.MovingPiece, bitMove.ToSquare);
            }

            var castlingDoneWhiteQueenSide = false;
            var castlingDoneRightWhiteKingSide = false;
            var castlingDoneRightBlackQueenSide = false;
            var castlingDoneRightBlackKingSide = false;

            // Do rook move in case of castling
            switch (bitMove.Castling)
            {
                case CastlingType.KingSide:
                    if (bitMove.MovingColor == BitColor.White)
                    {
                        bitBoards.RemovePiece(Square.H1);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.F1);
                        castlingDoneRightWhiteKingSide = true;
                    }
                    else
                    {
                        bitBoards.RemovePiece(Square.H8);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.F8);
                        castlingDoneRightBlackKingSide = true;
                    }
                    break;

                case CastlingType.QueenSide:
                    if (bitMove.MovingColor == BitColor.White)
                    {
                        bitBoards.RemovePiece(Square.A1);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.D1);
                        castlingDoneWhiteQueenSide = true;
                    }
                    else
                    {
                        bitBoards.RemovePiece(Square.A8);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.D8);
                        castlingDoneRightBlackQueenSide = true;
                    }
                    break;

                default:
                    break;
            }

            bitBoards.BoardState.Add(
                bitMove,
                GetEnPassantSquare(bitMove),
                bitBoards.BoardState.LastCastlingRightWhiteQueenSide && !castlingDoneWhiteQueenSide,
                bitBoards.BoardState.LastCastlingRightWhiteKingSide && !castlingDoneRightWhiteKingSide,
                bitBoards.BoardState.LastCastlingRightWhiteQueenSide && !castlingDoneRightBlackQueenSide,
                bitBoards.BoardState.LastCastlingRightWhiteKingSide && !castlingDoneRightBlackKingSide,
                BitHelper.OtherColor(bitMove.MovingColor));
        }

        private Square GetEnPassantSquare(BitMove move)
        {
            var enPassantSquare = Square.NoSquare;

            if (move.MovingPiece != BitPieceType.Pawn)
            {
                return enPassantSquare;
            }

            // black en passant field
            if (move.MovingColor == BitColor.Black) // black pawn
            {
                if (move.FromSquare - move.ToSquare == 16) // 2 fields move
                {
                    enPassantSquare = move.FromSquare - 8;
                }
            }
            else // white pawn
            {
                if (move.ToSquare - move.FromSquare == 16) // 2 fields move
                {
                    enPassantSquare = move.FromSquare + 8;
                }
            }

            return enPassantSquare;
        }

        public void UndoMove(BitMove bitMove, IBitBoard bitBoards)
        {
            bitBoards.RemovePiece(bitMove.ToSquare);
            bitBoards.SetPiece(bitMove.MovingColor, bitMove.MovingPiece, bitMove.FromSquare);
            if (bitMove.CapturedSquare != Square.NoSquare)
            {
                bitBoards.SetPiece(BitHelper.OtherColor(bitMove.MovingColor), bitMove.CapturedPiece, bitMove.CapturedSquare);
            }

            // Undo rook move in case of castling
            switch (bitMove.Castling)
            {
                case CastlingType.KingSide:
                    if (bitMove.MovingColor == BitColor.White)
                    {
                        bitBoards.RemovePiece(Square.F1);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.H1);
                    }
                    else
                    {
                        bitBoards.RemovePiece(Square.F8);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.H8);
                    }
                    break;

                case CastlingType.QueenSide:
                    if (bitMove.MovingColor == BitColor.White)
                    {
                        bitBoards.RemovePiece(Square.D1);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.A1);
                    }
                    else
                    {
                        bitBoards.RemovePiece(Square.D8);
                        bitBoards.SetPiece(BitColor.White, BitPieceType.Rook, Square.A8);
                    }
                    break;

                default:
                    break;
            }

            bitBoards.BoardState.Back();
        }
    }
}
