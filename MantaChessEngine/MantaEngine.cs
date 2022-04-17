﻿using System;

namespace MantaChessEngine
{
    public enum EngineType
    {
        Random,
        Minimax,
        MinimaxPosition,
        AlphaBeta
    }

    public class MantaEngine
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IMoveGenerator _moveGenerator;
        private MoveFactory _moveFactory;
        private ISearchService _search;
        private IEvaluator _evaluator;
        private Board _board;
        private IMoveOrder _moveOrder;

        public MantaEngine(EngineType engineType)
        {
            _moveFactory = new MoveFactory();
            _moveGenerator = new MoveGenerator(_moveFactory);

            switch (engineType)
            {
                case EngineType.Random:
                    _search = new SearchRandom(_moveGenerator);
                    break;

                case EngineType.Minimax:
                    _evaluator = new EvaluatorSimple();
                    _search = new SearchMinimax(_evaluator, _moveGenerator);
                    break;
                
                case EngineType.MinimaxPosition:
                    _evaluator = new EvaluatorPosition();
                    _search = new SearchMinimax(_evaluator, _moveGenerator);
                    break;
                
                // strongest --------------------------------
                case EngineType.AlphaBeta:
                    _evaluator = new EvaluatorPosition();
                    _moveOrder = new MoveOrderPV();
                    _search = new SearchAlphaBeta(_evaluator, _moveGenerator, 4, _moveOrder);
                    break;
                // -------------------------------------------

                default:
                    throw new Exception("No engine type defined.");
            }
        }

        public void SetBoard(Board board)
        {
            _board = board;
        }

        public void SetInitialPosition()
        {
            _board.SetInitialPosition();
        }

        public void SetPosition(string position)
        {
            _board.SetPosition(position);
        }

        public string GetString()
        {
            return _board.GetString;
        }

        public string GetPrintString()
        {
            return _board.GetPrintString2;
        }

        public bool MoveUci(string moveStringUci)
        {
            IMove move = _moveFactory.MakeMoveUci(_board, moveStringUci);
            if (move == null)
            {
                return false;
            }

            bool valid = _moveGenerator.IsMoveValid(_board, move);
            if (valid)
            {
                _board.Move(move);
            }

            return valid;
        }

        public bool Move(string moveStringUser)
        {
            IMove move = _moveFactory.MakeMove(_board, moveStringUser);
            if (move == null)
            {
                return false;
            }

            bool valid = _moveGenerator.IsMoveValid(_board, move);
            if (valid)
            {
                _board.Move(move);
            }

            return valid;
        }

        public bool Move(IMove move)
        {
            _board.Move(move);
            return true;
        }

        public bool IsWinner(Definitions.ChessColor color)
        {
            return _board.IsWinner(color);
        }

        public MoveRating DoBestMove(Definitions.ChessColor color)
        {
            MoveRating nextMove = _search.Search(_board, color);
            _board.Move(nextMove.Move);
            _log.Debug("Score: " + nextMove.Score);

            return nextMove;
        }
        
        public Definitions.ChessColor SideToMove()
        {
            return _board.SideToMove;
        }

        public bool IsCheck(Definitions.ChessColor color)
        {
            return _moveGenerator.IsCheck(_board, color);
        }

        public void Back()
        {
            _board.Back();
            _board.Back();
        }

        public void SetMaxSearchDepth(int depth)
        {
            if (_search != null)
            {
                _search.SetMaxDepth(depth);
            }
        }
    }
}
