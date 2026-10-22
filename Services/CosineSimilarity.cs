using Pgvector;

public class CosineSimilarityService
{
    public double Calculate(Vector a, Vector b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        float[] vectorA = a.ToArray();
        float[] vectorB = b.ToArray();

        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException(
                "Vectors must have the same length.");
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            throw new ArgumentException(
                "Cosine similarity is undefined for zero vectors.");
        }

        return dotProduct /
               (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}