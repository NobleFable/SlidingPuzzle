namespace SlidingPuzzle.Code.Utility
{
    public class TimeUtility
    {
        public static string GetTimeString(int seconds)
        {
            int minutes = seconds / 60;
            seconds = seconds - minutes * 60;
            if (minutes > 99)
                minutes = 99;
            return minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
}
