﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MantaChessEngine
{
    public class NormalMove : MoveBase
    {
        public NormalMove(Piece movingPiece, char sourceFile, int sourceRank, char targetFile, int targetRank, Piece capturedPiece)
            : base(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece)
        {
        }

        public NormalMove(Piece movingPiece, int sourceFile, int sourceRank, int targetFile, int targetRank, Piece capturedPiece)
            : base(movingPiece, sourceFile, sourceRank, targetFile, targetRank, capturedPiece)
        {
        }

        public override bool Equals(System.Object obj)
        {
            if (!(obj is NormalMove))
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string moveString = base.ToString();
            return moveString;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
