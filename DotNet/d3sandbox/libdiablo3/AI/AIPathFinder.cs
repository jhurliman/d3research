using System;
using System.Collections.Generic;

namespace libdiablo3.AI
{
    public enum PathFinderNodeType
    {
        Start = 1,
        End = 2,
        Open = 4,
        Close = 8,
        Current = 16,
        Path = 32
    }

    public enum HeuristicFormula
    {
        Manhattan = 1,
        MaxDXDY = 2,
        DiagonalShortCut = 3,
        Euclidean = 4,
        EuclideanNoSQR = 5,
        Custom1 = 6
    }

    public struct PathFinderNode
    {
        public int F;
        public int G;
        public int H; // f = gone + heuristic
        public int X;
        public int Y;
        public int PX; // Parent
        public int PY;
    }

    public class AIPathFinder
    {
        #region Internal Structs/Classes

        private struct PathFinderNodeFast
        {
            public int F; // f = gone + heuristic
            public int G;
            public ushort PX; // Parent
            public ushort PY;
            public byte Status;
        }

        private class ComparePFNodeMatrix : IComparer<int>
        {
            PathFinderNodeFast[] mMatrix;

            public ComparePFNodeMatrix(PathFinderNodeFast[] matrix)
            {
                mMatrix = matrix;
            }

            public int Compare(int a, int b)
            {
                if (mMatrix[a].F > mMatrix[b].F)
                    return 1;
                else if (mMatrix[a].F < mMatrix[b].F)
                    return -1;
                return 0;
            }
        }

        #endregion Internal Structs/Classes

        private byte[,] mGrid;
        private PriorityQueueB<int> mOpen;
        private bool mStop;
        private bool mStopped = true;
        private int mHoriz;
        private HeuristicFormula mFormula = HeuristicFormula.DiagonalShortCut;
        private bool mDiagonals = true;
        private int mHEstimate = 2;
        private bool mPunishChangeDirection;
        private bool mTieBreaker;
        private bool mHeavyDiagonals;
        private int mSearchLimit = 2000;
        private bool mDebugProgress;
        private bool mDebugFoundPath;
        private PathFinderNodeFast[] mCalcGrid;
        private byte mOpenNodeValue = 1;
        private byte mCloseNodeValue = 2;

        private sbyte[,] mDirection = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
        ushort mGridX;
        ushort mGridY;
        ushort mGridXMinus1;
        ushort mGridYLog2;

        #region Properties

        public bool Stopped { get { return mStopped; } }

        public HeuristicFormula Formula
        {
            get { return mFormula; }
            set { mFormula = value; }
        }

        public bool Diagonals
        {
            get { return mDiagonals; }
            set
            {
                mDiagonals = value;
                if (mDiagonals)
                    mDirection = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
                else
                    mDirection = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
            }
        }

        public bool HeavyDiagonals
        {
            get { return mHeavyDiagonals; }
            set { mHeavyDiagonals = value; }
        }

        public int HeuristicEstimate
        {
            get { return mHEstimate; }
            set { mHEstimate = value; }
        }

        public bool PunishChangeDirection
        {
            get { return mPunishChangeDirection; }
            set { mPunishChangeDirection = value; }
        }

        public bool TieBreaker
        {
            get { return mTieBreaker; }
            set { mTieBreaker = value; }
        }

        public int SearchLimit
        {
            get { return mSearchLimit; }
            set { mSearchLimit = value; }
        }

        public bool DebugProgress
        {
            get { return mDebugProgress; }
            set { mDebugProgress = value; }
        }

        public bool DebugFoundPath
        {
            get { return mDebugFoundPath; }
            set { mDebugFoundPath = value; }
        }

        #endregion Properties

        public AIPathFinder(byte[,] xyGrid)
        {
            if (xyGrid == null)
                throw new ArgumentException("xyGrid cannot be null");

            mGrid = xyGrid;
            mGridX = (ushort)(mGrid.GetLength(0));
            mGridY = (ushort)(mGrid.GetLength(1));
            mGridXMinus1 = (ushort)(mGridX - 1);
            mGridYLog2 = (ushort)Math.Log(mGridY, 2);

            if (Math.Log(mGridX, 2) != (int)Math.Log(mGridX, 2) ||
                Math.Log(mGridY, 2) != (int)Math.Log(mGridY, 2))
                throw new Exception("Invalid Grid, size in X and Y must be power of 2");

            mCalcGrid = new PathFinderNodeFast[mGridX * mGridY];
            mOpen = new PriorityQueueB<int>(new ComparePFNodeMatrix(mCalcGrid));
        }

        public List<PathFinderNode> FindPath(Vector2i start, Vector2i end)
        {
            int h;
            int newLocation;
            ushort locationX;
            ushort locationY;
            ushort newLocationX;
            ushort newLocationY;
            int newG;
            int closeNodeCounter = 0;
            bool found = false;
            List<PathFinderNode> output;
            int location = (start.Y << mGridYLog2) + start.X;
            int endLocation = (end.Y << mGridYLog2) + end.X;

            mStop = false;
            mStopped = false;
            mOpenNodeValue += 2;
            mCloseNodeValue += 2;
            mOpen.Clear();

            mCalcGrid[location].G = 0;
            mCalcGrid[location].F = mHEstimate;
            mCalcGrid[location].PX = (ushort)start.X;
            mCalcGrid[location].PY = (ushort)start.Y;
            mCalcGrid[location].Status = mOpenNodeValue;

            mOpen.Enqueue(location);
            while (mOpen.Count > 0 && !mStop)
            {
                location = mOpen.Dequeue();

                // Is it in closed list? means this node was already processed
                if (mCalcGrid[location].Status == mCloseNodeValue)
                    continue;

                locationX = (ushort)(location & mGridXMinus1);
                locationY = (ushort)(location >> mGridYLog2);

                if (location == endLocation)
                {
                    mCalcGrid[location].Status = mCloseNodeValue;
                    found = true;
                    break;
                }

                if (closeNodeCounter > mSearchLimit)
                {
                    mStopped = true;
                    return null;
                }

                if (mPunishChangeDirection)
                    mHoriz = (locationX - mCalcGrid[location].PX);

                // Calculate each successor
                for (int i = 0; i < (mDiagonals ? 8 : 4); i++)
                {
                    newLocationX = (ushort)(locationX + mDirection[i, 0]);
                    newLocationY = (ushort)(locationY + mDirection[i, 1]);
                    newLocation = (newLocationY << mGridYLog2) + newLocationX;

                    if (newLocationX >= mGridX || newLocationY >= mGridY)
                        continue;

                    // Unbreakable?
                    if (mGrid[newLocationX, newLocationY] == 0)
                        continue;

                    if (mHeavyDiagonals && i > 3)
                        newG = mCalcGrid[location].G + (int)(mGrid[newLocationX, newLocationY] * 2.41);
                    else
                        newG = mCalcGrid[location].G + mGrid[newLocationX, newLocationY];

                    if (mPunishChangeDirection)
                    {
                        if ((newLocationX - locationX) != 0)
                        {
                            if (mHoriz == 0)
                                newG += Math.Abs(newLocationX - end.X) + Math.Abs(newLocationY - end.Y);
                        }
                        if ((newLocationY - locationY) != 0)
                        {
                            if (mHoriz != 0)
                                newG += Math.Abs(newLocationX - end.X) + Math.Abs(newLocationY - end.Y);
                        }
                    }

                    // Is it open or closed?
                    if (mCalcGrid[newLocation].Status == mOpenNodeValue || mCalcGrid[newLocation].Status == mCloseNodeValue)
                    {
                        // The current node has less code than the previous? then skip this node
                        if (mCalcGrid[newLocation].G <= newG)
                            continue;
                    }

                    mCalcGrid[newLocation].PX = locationX;
                    mCalcGrid[newLocation].PY = locationY;
                    mCalcGrid[newLocation].G = newG;

                    switch (mFormula)
                    {
                        default:
                        case HeuristicFormula.Manhattan:
                            h = mHEstimate * (Math.Abs(newLocationX - end.X) + Math.Abs(newLocationY - end.Y));
                            break;
                        case HeuristicFormula.MaxDXDY:
                            h = mHEstimate * (Math.Max(Math.Abs(newLocationX - end.X), Math.Abs(newLocationY - end.Y)));
                            break;
                        case HeuristicFormula.DiagonalShortCut:
                            int h_diagonal = Math.Min(Math.Abs(newLocationX - end.X), Math.Abs(newLocationY - end.Y));
                            int h_straight = (Math.Abs(newLocationX - end.X) + Math.Abs(newLocationY - end.Y));
                            h = (mHEstimate * 2) * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                            break;
                        case HeuristicFormula.Euclidean:
                            h = (int)(mHEstimate * Math.Sqrt(Math.Pow((newLocationY - end.X), 2) + Math.Pow((newLocationY - end.Y), 2)));
                            break;
                        case HeuristicFormula.EuclideanNoSQR:
                            h = (int)(mHEstimate * (Math.Pow((newLocationX - end.X), 2) + Math.Pow((newLocationY - end.Y), 2)));
                            break;
                        case HeuristicFormula.Custom1:
                            Vector2i dxy = new Vector2i(Math.Abs(end.X - newLocationX), Math.Abs(end.Y - newLocationY));
                            int Orthogonal = Math.Abs(dxy.X - dxy.Y);
                            int Diagonal = Math.Abs(((dxy.X + dxy.Y) - Orthogonal) / 2);
                            h = mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);
                            break;
                    }
                    if (mTieBreaker)
                    {
                        int dx1 = locationX - end.X;
                        int dy1 = locationY - end.Y;
                        int dx2 = start.X - end.X;
                        int dy2 = start.Y - end.Y;
                        int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
                        h = (int)(h + cross * 0.001);
                    }
                    mCalcGrid[newLocation].F = newG + h;

                    mOpen.Enqueue(newLocation);
                    mCalcGrid[newLocation].Status = mOpenNodeValue;
                }

                closeNodeCounter++;
                mCalcGrid[location].Status = mCloseNodeValue;
            }

            if (found)
            {
                output = new List<PathFinderNode>();
                int posX = end.X;
                int posY = end.Y;

                PathFinderNodeFast fNodeTmp = mCalcGrid[(end.Y << mGridYLog2) + end.X];
                PathFinderNode fNode;
                fNode.F = fNodeTmp.F;
                fNode.G = fNodeTmp.G;
                fNode.H = 0;
                fNode.PX = fNodeTmp.PX;
                fNode.PY = fNodeTmp.PY;
                fNode.X = end.X;
                fNode.Y = end.Y;

                while (fNode.X != fNode.PX || fNode.Y != fNode.PY)
                {
                    output.Add(fNode);

                    posX = fNode.PX;
                    posY = fNode.PY;
                    fNodeTmp = mCalcGrid[(posY << mGridYLog2) + posX];
                    fNode.F = fNodeTmp.F;
                    fNode.G = fNodeTmp.G;
                    fNode.H = 0;
                    fNode.PX = fNodeTmp.PX;
                    fNode.PY = fNodeTmp.PY;
                    fNode.X = posX;
                    fNode.Y = posY;
                }

                output.Add(fNode);
                mStopped = true;
                return output;
            }

            mStopped = true;
            return null;
        }

        public void Cancel()
        {
            mStop = true;
        }
    }
}
