namespace cribbage_Royale_PROYECTO.Models
{
    public static class CardTextFormatter
    {
        private const string HeartsSuit = "CORAZONES";
        private const string DiamondsSuit = "DIAMANTES";
        private const string ClubsSuit = "TREBOLES";
        private const string SpadesSuit = "PICAS";

        public static string Format(string suit, byte value)
        {
            string rank = value.ToString();
            string symbol = string.Empty;

            if (value == 1)
            {
                rank = "A";
            }
            else if (value == 11)
            {
                rank = "J";
            }
            else if (value == 12)
            {
                rank = "Q";
            }
            else if (value == 13)
            {
                rank = "K";
            }

            if (suit == HeartsSuit)
            {
                symbol = "\u2665";
            }
            else if (suit == DiamondsSuit)
            {
                symbol = "\u2666";
            }
            else if (suit == ClubsSuit)
            {
                symbol = "\u2663";
            }
            else if (suit == SpadesSuit)
            {
                symbol = "\u2660";
            }

            return rank + symbol;
        }
    }
}