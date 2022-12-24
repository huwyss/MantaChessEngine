﻿using MantaChessEngine;
using System;
using System.Text;
using System.Runtime.CompilerServices;
using Bitboard = System.UInt64;
using MantaCommon;

[assembly: InternalsVisibleTo("MantaBitboardEngineTest")]
namespace MantaBitboardEngine
{
    public class Bitboards : IBitBoard
    {
        private readonly FenParser _fenParser;
        private readonly BitMoveExecutor _moveExecutor;
        public BitPieceType[] BoardAllPieces;
        public ChessColor[] BoardColor;

        /// <summary>
        /// Bitboard of each piece. Dimension: ChessColor, BitPieceType.
        /// </summary>
        public Bitboard[,] Bitboard_Pieces { get; set; }
        
        /// <summary>
        /// Bitboard of the pieces of one color. Dimendion: ChessColor.
        /// </summary>
        public Bitboard[] Bitboard_ColoredPieces { get; set; }

        /// <summary>
        /// All pieces in one bitboard.
        /// </summary>
        public Bitboard Bitboard_AllPieces;

        public Bitboard[,] BetweenMatrix { get; set; }

        public Bitboard[,] RayAfterMatrix { get; set; }

        public Bitboard QueenSideMask { get; set; }

        public Bitboard KingSideMask { get; set; }

        /// <summary>
        /// ToSquare of pawn attacking left. Dimensions: ChessColor, Square.
        /// </summary>
        public Square[,] PawnLeft { get; set; }

        /// <summary>
        /// ToSquare of pawn attacking right. Dimensions: ChessColor, Square.
        /// </summary>
        public Square[,] PawnRight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Square[,] PawnStep { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Square[,] PawnDoubleStep { get; set; }

        /// <summary>
        /// Bitboard of all capturing pawn move of a color from square. Dimensions: ChessColor, Square.
        /// </summary>
        public Bitboard[,] PawnCaptures { get; set; }

        /// <summary>
        /// Bitboard of all defending pawn move of a color from square. Dimensions: ChessColor, Square.
        /// </summary>
        public Bitboard[,] PawnDefends { get; set; }

        /// <summary>
        /// Bitboard of all pawn move of a color from square. Dimensions: ChessColor, Square.
        /// </summary>
        public Bitboard[,] PawnMoves { get; set; }

        /// <summary>
        /// Bitboard of all moves of piece type from square. Dimensions: BitPieceType, Square (piece: only knight, bishop, rook, queen, king, not for pawn)
        /// </summary>
        public Bitboard[,] MovesPieces { get; set; }

        public int[] Row { get; set; }
        public int[] Col { get; set; }
        public int[] DiagNO { get; set; }
        public int[] DiagNW { get; set; }

        public int[,] Rank { get; set; }

        public Bitboard[] IndexMask { get; set; }
        public Bitboard[] NotIndexMask { get; set; }
        public Bitboard[] MaskIsolatedPawns { get; set; }
        public Bitboard[] MaskWhitePassedPawns { get; set; }
        public Bitboard[] MaskWhitePawnsPath { get; set; }
        public Bitboard[] MaskBlackPassedPawns { get; set; }
        public Bitboard[] MaskBlackPawnsPath { get; set; }

        public Bitboard[] MaskCols { get; set; }
        public Bitboard Not_A_file { get; set; }
        public Bitboard Not_H_file { get; set; }

        /// <summary>
        /// Side to move.
        /// true = white, false = black.
        /// </summary>
        public bool Side { get; private set; }

        /// <summary>
        /// Other side.
        /// </summary>
        public bool XSide => !Side;

        public IBitBoardState BoardState { get ; }
       
        public string GetPositionString
        {
            get
            {
                string positionString = "";
                var row = 7;
                var col = 0;

                for (int i = 0; i < 64; i++)
                {
                    var square = (Square)(col + 8 * row);
                    var piece = GetPiece(square);
                    positionString += GetSymbol(piece.Color, piece.Piece);

                    col++;
                    if (col >= 8)
                    {
                        row--;
                        col = 0;
                    }
                }

                return positionString;
            }
        }

        public string GetPrintString
        {
            get
            {
                string boardString = "";
                var row = 7;
                var col = 0;

                boardString += "    a  b  c  d  e  f  g  h\n";
                boardString += "  +------------------------+\n";

                for (int i = 0; i < 64; i++)
                {
                    if (col == 0)
                    {
                        boardString += (row + 1) + " |";
                    }

                    var square = (Square)(col + 8 * row);
                    var piece = GetPiece(square);
                    boardString += " " + GetSymbol(piece.Color, piece.Piece) + " ";

                    if (col == 7)
                    {
                        boardString += "| " + (row + 1) + "\n";
                    }

                    col++;
                    if (col >= 8)
                    {
                        row--;
                        col = 0;
                        if (row >= 0)
                        {
                            boardString += "  |                        |\n";
                        }
                    }
                }

                boardString += "  +------------------------+\n";
                boardString += "    a  b  c  d  e  f  g  h\n";

                return boardString;
            }
        }

        public static string GetSymbol(ChessColor color, BitPieceType piece)
        {
            if (color == ChessColor.White && piece == BitPieceType.Pawn)
            {
                return "P";
            }
            else if (color == ChessColor.White && piece == BitPieceType.Knight)
            {
                return "N";
            }
            else if (color == ChessColor.White && piece == BitPieceType.Bishop)
            {
                return "B";
            }
            else if (color == ChessColor.White && piece == BitPieceType.Rook)
            {
                return "R";
            }
            else if (color == ChessColor.White && piece == BitPieceType.Queen)
            {
                return "Q";
            }
            else if (color == ChessColor.White && piece == BitPieceType.King)
            {
                return "K";
            }
            else if (color == ChessColor.Black && piece == BitPieceType.Pawn)
            {
                return "p";
            }
            else if (color == ChessColor.Black && piece == BitPieceType.Knight)
            {
                return "n";
            }
            else if (color == ChessColor.Black && piece == BitPieceType.Bishop)
            {
                return "b";
            }
            else if (color == ChessColor.Black && piece == BitPieceType.Rook)
            {
                return "r";
            }
            else if (color == ChessColor.Black && piece == BitPieceType.Queen)
            {
                return "q";
            }
            else if (color == ChessColor.Black && piece == BitPieceType.King)
            {
                return "k";
            }
            else if (color == ChessColor.Empty && piece == BitPieceType.Empty)
            {
                return ".";
            }

            return ".";
        }

        public Bitboards()
        {
            _fenParser = new FenParser();
            _moveExecutor = new BitMoveExecutor();
            BoardState = new BitBoardState();

            Bitboard_Pieces = new Bitboard[2, 7]; // todo: what is the 7th son of a seventh son?
            BoardAllPieces = new BitPieceType[64];
            BoardColor = new ChessColor[64];
            Bitboard_ColoredPieces = new Bitboard[2];

            MovesPieces = new Bitboard[6, 64];
        }

        public Bitboards(IBitBoardState state) : this ()
        {
            BoardState = state;
        }

        public void Initialize()
        {
            InitBitboardRowColDiag();
            SetBetweenMatrix();
            SetRayAfterMatrix();
            SetKingAndQueenSideMask();
            SetKnightMoves();
            SetKingMoves();
            SetQueenRookBishopMoves();
            SetIndexMask();
            SetPawnBitboards();
            SetRanks();
            SetMaskCols();
            ClearAllPieces();
    }

        public void ClearAllPieces()
        {
            for (var i = 0; i < 64; i++)
            {
                BoardAllPieces[i] = BitPieceType.Empty;
                BoardColor[i] = ChessColor.Empty;
            }

            for (var color = 0; color < 2; color++)
            {
                for (var pieceType = 0; pieceType < 7; pieceType++)
                {
                    Bitboard_Pieces[color, pieceType] = 0;
                }

                Bitboard_ColoredPieces[color] = 0;
            }

            Bitboard_AllPieces = 0;
        }

        private void InitBitboardRowColDiag()
        {

            Row = new int[64]
            {
                0, 0, 0, 0, 0, 0, 0, 0,
                1, 1, 1, 1, 1, 1, 1, 1,
                2, 2, 2, 2, 2, 2, 2, 2,
                3, 3, 3, 3, 3, 3, 3, 3,
                4, 4, 4, 4, 4, 4, 4, 4,
                5, 5, 5, 5, 5, 5, 5, 5,
                6, 6, 6, 6, 6, 6, 6, 6,
                7, 7, 7, 7, 7, 7, 7, 7,
            };

            Col = new int[64]
            {
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
                 0, 1, 2, 3, 4, 5, 6, 7,
            };

            DiagNO = new int[64]
            {
                7, 8, 9,10,11,12,13,14,
                6, 7, 8, 9,10,11,12,13,
                5, 6, 7, 8, 9,10,11,12,
                4, 5, 6, 7, 8, 9,10,11,
                3, 4, 5, 6, 7, 8, 9,10,
                2, 3, 4, 5, 6, 7, 8, 9,
                1, 2, 3, 4, 5, 6, 7, 8,
                0, 1, 2, 3, 4, 5, 6, 7,
            };

            DiagNW = new int[64]
            {
               14,13,12,11,10, 9, 8, 7,
               13,12,11,10, 9, 8, 7, 6,
               12,11,10, 9, 8, 7, 6, 5,
               11,10, 9, 8, 7, 6, 5, 4,
               10, 9, 8, 7, 6, 5, 4, 3,
                9, 8, 7, 6, 5, 4, 3, 2,
                8, 7, 6, 5, 4, 3, 2, 1,
                7, 6, 5, 4, 3, 2, 1, 0,
            };
        }

        private void SetBetweenMatrix()
        {
            BetweenMatrix = new Bitboard[64, 64];

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (Col[ x ] == Col[ y ])
                    {
                        if (y > x)
                        {
                            for (int i = x + 8; i < y; i += 8)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            for (int i = x - 8; i > y; i -= 8)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                    }
                    else if (Row[ x ] == Row[ y ])
                    {
                        if (y > x)
                        {
                            for (int i = x + 1; i < y; i++)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            for (int i = x - 1; i > y; i--)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                    }
                    else if (DiagNW[x] == DiagNW[y])
                    {
                        if (y > x)
                        {
                            for (int i = x + 7; i < y; i += 7)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            for (int i = x - 7; i > y; i -= 7)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                    }
                    else if (DiagNO[x] == DiagNO[y])
                    {
                        if (y > x)
                        {
                            for (int i = x + 9; i < y; i += 9)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            for (int i = x - 9; i > y; i -= 9)
                            {
                                SetBit(ref BetweenMatrix[x, y], i);
                            }
                        }
                    }
                }
            }
        }

        private void SetRayAfterMatrix()
        {
            RayAfterMatrix = new Bitboard[64, 64];

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (Col[x] == Col[y])
                    {
                        if (y > x)
                        {
                            var edge = GetEdge(y, 8);
                            for (int i = y; i <= edge; i += 8)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            var edge = GetEdge(y, -8);
                            for (int i = y; i >= edge; i -= 8)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                    }
                    else if (Row[x] == Row[y])
                    {
                        if (y > x)
                        {
                            var edge = GetEdge(y, 1);
                            for (int i = y; i <= edge; i++)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            var edge = GetEdge(y, -1);
                            for (int i = y; i >= edge; i--)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                    }
                    else if (DiagNW[x] == DiagNW[y])
                    {
                        if (y > x)
                        {
                            var edge = GetEdge(y, 7);
                            for (int i = y; i <= edge; i += 7)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            var edge = GetEdge(y, -7);
                            for (int i = y; i >= edge; i -= 7)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                    }
                    else if (DiagNO[x] == DiagNO[y])
                    {
                        if (y > x)
                        {
                            var edge = GetEdge(y, 9);
                            for (int i = y; i <= edge; i += 9)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                        else
                        {
                            var edge = GetEdge(y, -9);
                            for (int i = y; i >= edge; i -= 9)
                            {
                                SetBit(ref RayAfterMatrix[x, y], i);
                            }
                        }
                    }
                }
            }
        }

        public int GetEdge(int square, int direction)
        {
            while (square >= 0 && 
                (
                direction == 1 && Col[square] < 7 ||
                direction == -1 && Col[square] > 0 ||
                direction == 8 && Row[square] < 7 ||
                direction == -8 && Row[square] > 0 ||

                direction == 7 && Col[square] > 0 && Row[square] < 7 ||
                direction == 9 && Col[square] < 7 && Row[square] < 7 ||
                direction == -7 && Col[square] < 7 && Row[square] > 0 ||
                direction == -9 && Col[square] > 0 && Row[square] > 0
                ))
            {
                square += direction;
            }
            
            return square;
        }

        private void SetKingAndQueenSideMask()
        {
            Bitboard queenSideMask = 0;
            Bitboard kingSideMask = 0;
            for (int i = 0; i < 64; i++)
            {
                if (Col[i] < 2)
                {
                    SetBit(ref queenSideMask, i);
                }

                if (Col[i] > 5)
                {
                    SetBit(ref kingSideMask, i);
                }
            }

            QueenSideMask = queenSideMask;
            KingSideMask = kingSideMask;
        }

        private void SetKnightMoves()
        {
            //      -17   -15
            //    -10        -6
            //          N
            //    +6         +10
            //      +15   +17

            for (int i = 0; i < 64; i++)
            {
                if (Col[i] < 6 && Row[i] < 7)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i + 10);
                }

                if (Col[i] < 7 && Row[i] < 6)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i + 17);
                }

                if (Col[i] > 0 && Row[i] < 6)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i + 15);
                }

                if (Col[i] > 1 && Row[i] < 7)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i + 6);
                }

                if (Col[i] > 0 && Row[i] > 1)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i - 17);
                }

                if (Col[i] > 1 && Row[i] > 0)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i - 10);
                }

                if (Col[i] < 7 && Row[i] > 1)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i - 15);
                }

                if (Col[i] < 6 && Row[i] > 0)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.Knight, i], i - 6);
                }
            }
        }

        private void SetKingMoves()
        {
            //       -9  -8  -7
            //       -1   K  +1
            //       +7  +8  +9 

            for (int i = 0; i < 64; i++)
            {
                if (Col[i] > 0 && Row[i] > 0)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i - 1);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i - 8);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i - 9);
                }

                if (Col[i] < 7 && Row[i] > 0)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i - 7);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i - 8);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i + 1);
                }

                if (Col[i] > 0 && Row[i] < 7)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i - 1);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i + 7);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i + 8);
                }

                if (Col[i] < 7 && Row[i] < 7)
                {
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i + 1);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i + 8);
                    SetBit(ref MovesPieces[(int)BitPieceType.King, i], i + 9);
                }
            }
        }

        private void SetQueenRookBishopMoves()
        {
            //       -9  -8  -7
            //       -1   K  +1
            //       +7  +8  +9 

            for (int i = 0; i < 64; i++)
            {
                if (Row[i] < 7)
                {
                    var edge = GetEdge(i, 8);
                    for (int z = i+8; z <= edge; z += 8)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Rook, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Row[i] > 0)
                {
                    var edge = GetEdge(i, -8);
                    for (int z = i - 8; z >= edge; z -= 8)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Rook, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Col[i] < 7)
                {
                    var edge = GetEdge(i, 1);
                    for (int z = i + 1; z <= edge; z++)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Rook, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Col[i] > 0)
                {
                    var edge = GetEdge(i, -1);
                    for (int z = i - 1; z >= edge; z--)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Rook, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Col[i] > 0 && Row[i] < 7)
                {
                    var edge = GetEdge(i, 7);
                    for (int z=i + 7; z <= edge; z +=7)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Bishop, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Col[i] < 7 && Row[i] < 7)
                {
                    var edge = GetEdge(i, 9);
                    for (int z = i + 9; z <= edge; z += 9)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Bishop, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Col[i] < 7 && Row[i] > 0)
                {
                    var edge = GetEdge(i, -7);
                    for (int z = i - 7; z >= edge; z -= 7)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Bishop, i], z);
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }

                if (Col[i] > 0 && Row[i] > 0)
                {
                    var edge = GetEdge(i, -9);
                    for (int z = i - 9; z >= edge; z -= 9)
                    {
                        SetBit(ref MovesPieces[(int)BitPieceType.Bishop, i], z); 
                        SetBit(ref MovesPieces[(int)BitPieceType.Queen, i], z);
                    }
                }
            }
        }

        private void SetIndexMask()
        {
            IndexMask = new Bitboard[64];
            NotIndexMask = new Bitboard[64];

            for (int i = 0; i < 64; i++)
            {
                SetBit(ref IndexMask[i], i);
                NotIndexMask[i] = ~IndexMask[i];
            }
        }

        private void SetPawnBitboards()
        {
            SetPawnCaptures();
            SetPawnStraightMoves();
            SetPawnMasks();
        }

        private void SetPawnCaptures()
        {
            PawnCaptures = new Bitboard[2, 64];
            PawnLeft = new Square[2, 64];
            PawnRight = new Square[2, 64];
            PawnDefends = new Bitboard[2, 64];

            // clear PawnLeft and PawnRight
            for (int i = 0; i < 64; i++)
            {
                PawnLeft[(int)ChessColor.White, i] = Square.NoSquare;
                PawnLeft[(int)ChessColor.Black, i] = Square.NoSquare;
                PawnRight[(int)ChessColor.White, i] = Square.NoSquare;
                PawnRight[(int)ChessColor.Black, i] = Square.NoSquare;
            }

            for (int i = 0; i < 64; i++)
            {
                if (Col[i] > 0)
                {
                    if (Row[i] < 7)
                    {
                        // white captures left
                        var stepLeftWhite = i + 7;
                        SetBit(ref PawnCaptures[(int)ChessColor.White, i], stepLeftWhite);
                        PawnLeft[(int)ChessColor.White, i] = (Square)stepLeftWhite;
                    }

                    if (Row[i] > 0)
                    {
                        // black captures left
                        var stepLeftBlack = i - 9;
                        SetBit(ref PawnCaptures[(int)ChessColor.Black, i], stepLeftBlack);
                        PawnLeft[(int)ChessColor.Black, i] = (Square)stepLeftBlack;
                    }
                }

                if (Col[i] < 7)
                {
                    if (Row[i] < 7)
                    {
                        // white captures right
                        var stepRightWhite = i + 9;
                        SetBit(ref PawnCaptures[(int)ChessColor.White, i], stepRightWhite);
                        PawnRight[(int)ChessColor.White, i] = (Square)stepRightWhite;
                    }

                    if (Row[i] > 0)
                    {
                        // black captures right
                        var stepRightBlack = i - 7;
                        SetBit(ref PawnCaptures[(int)ChessColor.Black, i], stepRightBlack);
                        PawnRight[(int)ChessColor.Black, i] = (Square)stepRightBlack;
                    }
                }

                PawnDefends[(int)ChessColor.White, i] = PawnCaptures[(int)ChessColor.Black, i];
                PawnDefends[(int)ChessColor.Black, i] = PawnCaptures[(int)ChessColor.White, i];
            }
        }

        private void SetPawnStraightMoves()
        {
            PawnMoves = new Bitboard[2, 64];
            PawnStep = new Square[2, 64];
            PawnDoubleStep = new Square[2, 64];

            for (int i = 0; i < 64; i++)
            {
                if (Row[i] < 7 )
                {
                    // step white
                    SetBit(ref PawnMoves[(int)ChessColor.White, i], i + 8);
                    PawnStep[(int)ChessColor.White, i] = (Square)(i + 8);
                }

                if (Row[i] == 1)
                {
                    // double step white
                    SetBit(ref PawnMoves[(int)ChessColor.White, i], i + 16);
                    PawnDoubleStep[(int)ChessColor.White, i] = (Square)(i + 16);
                }

                if (Row[i] > 0)
                {
                    // step black
                    SetBit(ref PawnMoves[(int)ChessColor.Black, i], i - 8);
                    PawnStep[(int)ChessColor.Black, i] = (Square)(i - 8);
                }

                if (Row[i] == 6)
                {
                    // step black
                    SetBit(ref PawnMoves[(int)ChessColor.Black, i], i - 16);
                    PawnDoubleStep[(int)ChessColor.Black, i] = (Square)(i - 16);
                }
            }
        }

        private void SetPawnMasks()
        {
            MaskIsolatedPawns = new Bitboard[64];

            MaskWhitePassedPawns = new Bitboard[64];
            MaskWhitePawnsPath = new Bitboard[64];
            MaskBlackPassedPawns = new Bitboard[64];
            MaskBlackPawnsPath = new Bitboard[64];

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    if (Math.Abs(Col[x] - Col[y]) < 2)
                    {
                        if (Row[x] < Row[y] && Row[y] < 7)
                        {
                            SetBit(ref MaskWhitePassedPawns[x], y);
                        }

                        if (Row[x] > Row[y] && Row[y] > 0)
                        {
                            SetBit(ref MaskBlackPassedPawns[x], y);
                        }
                    }

                    if (Math.Abs(Col[x] - Col[y]) == 1)
                    {
                        SetBit(ref MaskIsolatedPawns[x], y);
                        SetBit(ref MaskIsolatedPawns[x], y);
                    }

                    if (Col[x] == Col[y])
                    {
                        if (Row[x] < Row[y])
                        {
                            SetBit(ref MaskWhitePawnsPath[x], y);
                        }

                        if (Row[x] > Row[y])
                        {
                            SetBit(ref MaskBlackPawnsPath[x], y);
                        }
                    }
                }
            }
        }

        public static string PrintBitboard(Bitboard bitboard)
        {
            var board = new StringBuilder();
            for (int y = 7; y >=0; y--)
            {
                for (int x = 0; x<= 7; x++)
                {
                    board.Append(GetBit(bitboard, x + 8 * y) ? " 1" : " .");
                }

                board.Append("\n");
            }

            return board.ToString();
        }

        private static void SetBit(ref UInt64 bitboard, int index)
        {
            bitboard |= ((UInt64)0x01 << index);
        }

        private static void ClearBit(ref UInt64 bitboard, int index)
        {
            bitboard &= (~((UInt64)0x01 << index));
        }
        private static bool GetBit(UInt64 bitboard, int index)
        {
            return (bitboard & ((UInt64)0x01 << index)) != (UInt64)0x00;
        }

        public static UInt64 ConvertToUInt64(byte[] input)
        {
            UInt64 result = 0;
            for (int i=0; i<64; i++)
            {
                if (input[i] != 0)
                {
                    SetBit(ref result, i);
                }
            }

            return result;
        }

        private const ulong DeBruijnSequence = 0x37E84A99DAE458F;

        private readonly int[] MultiplyDeBruijnBitPosition =
        {
            0, 1, 17, 2, 18, 50, 3, 57,
            47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6,
            61, 44, 12, 35, 60, 11, 10, 9,
        };

        /// <summary>
        /// Search the mask data from least significant bit (LSB) to the most significant bit (MSB) for a set bit (1)
        /// using De Bruijn sequence approach. Warning: Will return zero for b = 0.
        /// </summary>
        /// <param name="b">Target number.</param>
        /// <returns>Zero-based position of LSB (from right to left).</returns>
        public int BitScanForward(ulong b)
        {
            return MultiplyDeBruijnBitPosition[((ulong)((long)b & -(long)b) * DeBruijnSequence) >> 58];
        }

        private void SetRanks()
        {
            Rank = new int[2, 64];
            for (int i=0; i<64; i++)
            {
                Rank[0, i] = Row[i];
                Rank[1, i] = 7 - Row[i];

            }
        }

        private void SetMaskCols()
        {
            MaskCols = new Bitboard[64];

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    if (Col[x] == Col[y])
                    {
                        SetBit(ref MaskCols[x], y);
                    }
                }
            }

            Not_A_file = ~MaskCols[0];
            Not_H_file = ~MaskCols[7];
        }

        public string SetFenPosition(string fen)
        {
            PositionInfo positionInfo;
            try
            {
                positionInfo = _fenParser.ToPositionInfo(fen);
            }
            catch (Exception ex)
            {
                return "FEN error: " + ex.StackTrace;
            }

            SetPosition(positionInfo.PositionString);
            var enpassantSquare = positionInfo.EnPassantFile != '\0'
                ? (Square)(positionInfo.EnPassantFile - '0' - 1 + 8 * positionInfo.EnPassantRank)
                : Square.NoSquare;

            BoardState.Clear();
            BoardState.Add(
                BitMove.CreateEmptyMove(),
                enpassantSquare,
                positionInfo.CastlingRightWhiteQueenSide,
                positionInfo.CastlingRightWhiteKingSide,
                positionInfo.CastlingRightBlackQueenSide,
                positionInfo.CastlingRightBlackKingSide,
                positionInfo.SideToMove == ChessColor.White ? ChessColor.White : ChessColor.Black);

            return string.Empty;
        }

        public string GetFenString()
        {
            throw new NotImplementedException();
        }

        public void SetInitialPosition()
        {
            string initPosition = "rnbqkbnr" + // black a8-h8
                                  "pppppppp" +
                                  "........" +
                                  "........" +
                                  "........" +
                                  "........" +
                                  "PPPPPPPP" +
                                  "RNBQKBNR"; // white a1-h1

            SetPosition(initPosition);
        }

        public void SetPosition(string position)
        {
            ClearAllPieces();
            var row = 7;
            var col = 0;

            for (int i = 0; i < 64; i++)
            {
                var square = (Square)(col + 8 * row);
                
                switch (position[i])
                {
                    case 'P':
                        SetPiece(ChessColor.White, BitPieceType.Pawn, square);
                        break;
                    case 'N':
                        SetPiece(ChessColor.White, BitPieceType.Knight, square);
                        break;
                    case 'B':
                        SetPiece(ChessColor.White, BitPieceType.Bishop, square);
                        break;
                    case 'R':
                        SetPiece(ChessColor.White, BitPieceType.Rook, square);
                        break;
                    case 'Q':
                        SetPiece(ChessColor.White, BitPieceType.Queen, square);
                        break;
                    case 'K':
                        SetPiece(ChessColor.White, BitPieceType.King, square);
                        break;
                    case 'p':
                        SetPiece(ChessColor.Black, BitPieceType.Pawn, square);
                        break;
                    case 'n':
                        SetPiece(ChessColor.Black, BitPieceType.Knight, square);
                        break;
                    case 'b':
                        SetPiece(ChessColor.Black, BitPieceType.Bishop, square);
                        break;
                    case 'r':
                        SetPiece(ChessColor.Black, BitPieceType.Rook, square);
                        break;
                    case 'q':
                        SetPiece(ChessColor.Black, BitPieceType.Queen, square);
                        break;
                    case 'k':
                        SetPiece(ChessColor.Black, BitPieceType.King, square);
                        break;
                    case '.':
                    case ' ':
                        break;
                    default:
                        throw new MantaEngineException($"Piece character unknown: {position[i]}");
                }

                col++;
                if (col >= 8)
                {
                    row--;
                    col = 0;
                }
            }
        }

        public BitPiece GetPiece(Square square)
        {
            return new BitPiece(BoardColor[(int)square], BoardAllPieces[(int)square]);
        }

        public void SetPiece(ChessColor color, BitPieceType piece, Square square)
        {
            BoardAllPieces[(int)square] = piece;
            BoardColor[(int)square] = color;
            SetBit(ref Bitboard_Pieces[(int)color, (int)piece], (int)square);
            SetBit(ref Bitboard_AllPieces, (int)square); // todo test this
            SetBit(ref Bitboard_ColoredPieces[(int) color], (int)square); // todo test this
        }

        public void RemovePiece(Square square)
        {
            var piece = BoardAllPieces[(int)square];
            var color = BoardColor[(int)square];
            ClearBit(ref Bitboard_Pieces[(int)color, (int)piece], (int)square);
            ClearBit(ref Bitboard_AllPieces, (int)square); // todo test this
            ClearBit(ref Bitboard_ColoredPieces[(int)color], (int)square); // todo test this

            BoardAllPieces[(int)square] = BitPieceType.Empty;
            BoardColor[(int)square] = ChessColor.Empty;
        }

        public void Move(BitMove nextMove)
        {
            _moveExecutor.DoMove(nextMove, this);
        }

        public void Back()
        {
            _moveExecutor.UndoMove(BoardState.LastMove, this);
        }
    }
}