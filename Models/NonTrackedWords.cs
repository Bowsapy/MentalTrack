using Humanizer;

namespace MentalTrack.Models
{
    public static class NonTrackedWords
    {
        public static readonly HashSet<string> English = new()
{
    "a", "an", "the",
    "and", "or", "but",
    "if", "then", "else",
    "is", "am", "are", "was", "were",
    "be", "been", "being",
    "have", "has", "had",
    "do", "does", "did",
    "will", "would", "shall", "should",
    "can", "could", "may", "might", "must",

    "i", "you", "he", "she", "it",
    "we", "they",
    "me", "him", "her", "us", "them",
    "my", "your", "his", "her", "its",
    "our", "their",
    "mine", "yours", "ours", "theirs",

    "this", "that", "these", "those",

    "in", "on", "at", "by",
    "for", "with", "about",
    "against", "between", "into",
    "through", "during", "before",
    "after", "above", "below",
    "from", "up", "down",
    "over", "under",

    "of", "to", "from",
    "as", "than",

    "not", "no", "nor",

    "very", "just", "only",
    "also", "too",

    "here", "there",
    "when", "where",
    "why", "how",
    "what", "which", "who",

    "all", "any", "some",
    "many", "much",
    "few", "more", "most",

    "other", "another",

    "again", "once",
    "always", "never",
    "sometimes",

    "because", "while",
    "although",

    "own", "same", ".", ",", "-", "etc", "so"
};
    }
}
