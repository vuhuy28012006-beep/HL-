using UnityEngine;

public static class StarCalculator
{
    public static int CalculateStars(
        int movesLeft,
        int maxMoves,
        int hintsUsed,
        int undosUsed)
    {
        // Điều kiện 3 sao
        if (hintsUsed == 0 &&
            undosUsed == 0 &&
            movesLeft >= maxMoves * 0.5f)
        {
            return 3;
        }

        // Điều kiện 2 sao
        if (hintsUsed <= 1 &&
            undosUsed <= 1 &&
            movesLeft >= maxMoves * 0.2f)
        {
            return 2;
        }

        // Chỉ cần thắng
        return 1;
    }
}