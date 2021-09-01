using SFML.Graphics;
using System;

namespace Shared.Helpers
{
    public static class BlueprintHelper
    {
        public static Image CreateBlueprint(uint width, uint height)
        {
            Color background = new Color(19, 93, 168);
            Color thick = new Color(57, 121, 185);
            Color thin = new Color(36, 108, 182);

            void DrawVerticalLine(Image image, uint x, Color color)
            {
                if (x < image.Size.X)
                {
                    for (uint y = 0; y < image.Size.Y; y++)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }

            void DrawHorizontalLine(Image image, uint y, Color color)
            {
                if (y < image.Size.Y)
                {
                    for (uint x = 0; x < image.Size.X; x++)
                    {
                        image.SetPixel(x, y, color);
                    }
                }
            }

            Image image = new Image(width, height, background);

            int rowCount = (int)Math.Ceiling(height / 100.0);
            int colCount = (int)Math.Ceiling(width / 100.0);

            int gridWidth = colCount * 100;
            int gridHeight = rowCount * 100;

            int startX = 0 - (gridWidth - (int)width) / 2;
            int startY = 0 - (gridHeight - (int)height) / 2;

            for (int col = 0; col < colCount; col++)
            {
                var x = (uint)(startX + col * 100);
                DrawVerticalLine(image, x + 0, thick);
                DrawVerticalLine(image, x + 25, thin);
                DrawVerticalLine(image, x + 50, thick);
                DrawVerticalLine(image, x + 75, thin);
                DrawVerticalLine(image, x + 99, thick);
            }

            for (int row = 0; row < rowCount; row++)
            {
                var y = (uint)(startY + row * 100);
                DrawHorizontalLine(image, y + 0, thick);
                DrawHorizontalLine(image, y + 25, thin);
                DrawHorizontalLine(image, y + 50, thick);
                DrawHorizontalLine(image, y + 75, thin);
                DrawHorizontalLine(image, y + 99, thick);
            }

            return image;
        }
    }
}
