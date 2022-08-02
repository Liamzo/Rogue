using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoSVision : Vision
{
    public override UnitController FindTarget(UnitController tryTarget) {
        UnitController target  = tryTarget;

        if (target == null) {
            return null;
        }

        if (base.FindTarget(tryTarget) != null) {
            // Target in range
            int dx = target.x - parent.x;
            int dy = target.y - parent.y;
            int max = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

            float xStep = dx / (float) max;
            float yStep = dy / (float) max;

            List<Vector2Int> path = new List<Vector2Int>();

            for (int i = 1; i <= parent.unitStats.stats[(int)Stats.Sight].GetValue(); i++) {
                int xPos = Mathf.RoundToInt(xStep * i) + parent.x;
                int yPos = Mathf.RoundToInt(yStep * i) + parent.y;

                Vector2Int tPos = new Vector2Int(xPos,yPos);

                if (!Game.instance.map.IsWithinMap(tPos)) {
                    return null;
                }

                if (Game.instance.map.GetTile(tPos.x,tPos.y).occupiedBy == target) {
                    break;
                }
            }

            return target;
        }

        return null;
    }

    public bool CheckClearLoS(UnitController target) {
        if (target == null) {
            return false;
        }

        if (base.FindTarget(target) != null) {
            // Target in range
            // int dx = target.x - parent.x;
            // int dy = target.y - parent.y;
            // int max = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

            // float xStep = dx / (float) max;
            // float yStep = dy / (float) max;

            // List<Vector2Int> path = new List<Vector2Int>();

            // for (int i = 1; i <= parent.unitStats.stats[(int)Stats.Sight].GetValue(); i++) {
            //     int xPos = Mathf.RoundToInt(xStep * i) + parent.x;
            //     int yPos = Mathf.RoundToInt(yStep * i) + parent.y;

            //     Vector2Int tPos = new Vector2Int(xPos,yPos);

            //     if (!Game.instance.map.IsPositionClear(tPos)) {
            //         return false;
            //     }

            //     if (Game.instance.map.GetTile(tPos.x,tPos.y).occupiedBy == target) {
            //         break;
            //     }
            // }

            List<Vector> sightPath = CastRay(Vector.Create(parent.x, parent.y), Vector.Create(target.x, target.y), false, false);

            foreach(Vector tile in sightPath) {
                if (!Game.instance.map.IsPositionClear(new Vector2Int(tile.X, tile.Y))) {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public static List<Vector> CastRay(
    Vector start, Vector end, 
    bool includeStart, bool includeEnd)
    {
        int xIncrement = (end.X > start.X) ? 1 : -1;
        int yIncrement = (end.Y > start.Y) ? 1 : -1;

        Vector delta = (start - end).Abs();
        int error = delta.X - delta.Y;
        Vector errorCorrect = delta * 2;

        List<Vector> tiles = new List<Vector>();
        var current = start;
        while (true)
        {
            if ((current == start && includeStart) ||
                (current == end && includeEnd) ||
                (current != start && current != end))
            {
                tiles.Add(current);
            }

            if (current == end)
            {
                break;
            }

            if (error > 0)
            {
                current = Vector.Create(current.X + xIncrement, current.Y);
                error -= errorCorrect.Y;
            }
            else if (error < 0)
            {
                current = Vector.Create(current.X, current.Y + yIncrement);
                error += errorCorrect.X;
            }
            else
            {
                current = Vector.Create(
                    current.X + xIncrement,
                    current.Y + yIncrement);
            }
        }

        return tiles;
    }

    public struct Vector : System.IEquatable<Vector>
    {
        private Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector Create(int x, int y) => new Vector(x, y);

        public int X {get;}
        public int Y {get;}

        public Vector Abs() => new Vector(Mathf.Abs(X), Mathf.Abs(Y));

        public double DistanceFrom(Vector target)
        {
            var diffX = Mathf.Abs(X - target.X);
            var diffY = Mathf.Abs(Y - target.Y);

            return Mathf.Sqrt((diffX * diffX) + (diffY * diffY));
        }

        public override bool Equals(object obj)
            => obj is Vector && Equals((Vector)obj);

        public bool Equals(Vector other)
            => X == other.Y && X == other.Y;

        public static bool operator ==(Vector vector1, Vector vector2)
            => vector1.Equals(vector2);

        public static bool operator !=(Vector vector1, Vector vector2)
            => !vector1.Equals(vector2);

        public static bool operator >(Vector vector1, Vector vector2)
            => vector1.X > vector2.X && vector1.Y > vector2.Y;

        public static bool operator <(Vector vector1, Vector vector2)
            => vector1.X < vector2.X && vector1.Y < vector2.Y;

        public static bool operator >=(Vector vector1, Vector vector2)
            => vector1.X >= vector2.X && vector1.Y >= vector2.Y;

        public static bool operator <=(Vector vector1, Vector vector2)
            => vector1.X <= vector2.X && vector1.Y <= vector2.Y;

        public static Vector operator +(Vector vector1, Vector vector2)
            => new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);

        public static Vector operator -(Vector vector1, Vector vector2)
            => new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);

        public static Vector operator *(Vector vector, int scale)
            => new Vector(vector.X * scale, vector.Y * scale);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = X.GetHashCode();
                hash = (hash * 397) ^ Y.GetHashCode();
                return hash;
            }
        }
    }

}
