using UnityEngine;

public class BlockPos
{
    public static readonly BlockPos Zero = new BlockPos(0, 0, 0);

    public int X { get { return m_x; } }
    public int Y { get { return m_y; } }
    public int Z { get { return m_z; } }

    private readonly int m_x;
    private readonly int m_y;
    private readonly int m_z;

    public BlockPos(int x, int y, int z)
    {
        m_x = x;
        m_y = y;
        m_z = z;
    }

    public BlockPos(Vector3 point)
    {
        m_x = (int) point.x;
        m_y = (int) point.y;
        m_z = (int) point.z;
    }

    public static BlockPos operator +(BlockPos pos1, BlockPos pos2)
    {
        return new BlockPos(pos1.X + pos2.X, pos1.Y + pos2.Y, pos1.Z + pos2.Z);
    }

    public static BlockPos operator -(BlockPos pos1, BlockPos pos2)
    {
        return new BlockPos(pos1.X - pos2.X, pos1.Y - pos2.Y, pos1.Z - pos2.Z);
    }

    public static bool operator <(BlockPos pos1, BlockPos pos2)
    {
        return pos1.X < pos2.X && pos1.Y < pos2.Y && pos1.Z < pos2.Z;
    }

    public static bool operator >(BlockPos pos1, BlockPos pos2)
    {
        return pos1.X > pos2.X && pos1.Y > pos2.Y && pos1.Z > pos2.Z;
    }

    public static bool operator <=(BlockPos pos1, BlockPos pos2)
    {
        return pos1.X <= pos2.X && pos1.Y <= pos2.Y && pos1.Z <= pos2.Z;
    }

    public static bool operator >=(BlockPos pos1, BlockPos pos2)
    {
        return pos1.X >= pos2.X && pos1.Y >= pos2.Y && pos1.Z >= pos2.Z;
    }

    public static bool operator ==(BlockPos pos1, BlockPos pos2)
    {
        return ReferenceEquals(null, pos1) ? ReferenceEquals(null, pos2) : pos1.Equals(pos2);
    }

    public static bool operator !=(BlockPos pos1, BlockPos pos2)
    {
        return !(pos1 == pos2);
    }

    protected bool Equals(BlockPos other)
    {
        if (ReferenceEquals(null, other))
            return false;

        return m_x == other.m_x && m_y == other.m_y && m_z == other.m_z;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((BlockPos)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = m_x;
            hashCode = (hashCode * 397) ^ m_y;
            hashCode = (hashCode * 397) ^ m_z;
            return hashCode;
        }
    }
    
    public override string ToString()
    {
        return string.Format("({0}, {1}, {2})", m_x, m_y, m_z);
    }

    public Vector3 ToScenePos()
    {
        return new Vector3(m_x + 0.5f, m_y + 0.5f, m_z + 0.5f);
    }
}
