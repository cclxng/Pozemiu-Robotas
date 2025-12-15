namespace PozemiuRobotas;

public static class LevelLoader
{
    public static (string[] normalized, (int x, int y) start) Load(string[] layout)
    {
        int startX = 1, startY = 1;
        var normalized = new string[layout.Length];

        for (int y = 0; y < layout.Length; y++)
        {
            var row = layout[y].ToCharArray();

            for (int x = 0; x < row.Length; x++)
            {
                if (row[x] == GameConfig.StartChar)
                {
                    startX = x;
                    startY = y;
                    row[x] = GameConfig.EmptyChar;
                }
            }

            normalized[y] = new string(row);
        }

        return (normalized, (startX, startY));
    }
}
