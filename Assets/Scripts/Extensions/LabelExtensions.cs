namespace Laughter.Poker.Extensions
{
    public static class LabelExtensions
    {
        public static string ToLargeBold(int number, int font = 40)
        {
            return ToLargeBold(number.ToString(), font);
        }

        public static string ToLargeBold(string number, int font = 40)
        {
            return $"<b><size={font}>{number}</size></b>";
        }
    }
}
