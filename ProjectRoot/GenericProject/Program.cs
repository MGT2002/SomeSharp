using System;
using System.Threading;

// 1. Create a new class Matrix
// 2. Matrix will have a method called StartGame()
// 3. StartGame will print 1, 0 in grin like in a fil Matrix
// 4. the numbers must fall down in console Like in a Matrix
class Matrix
{
    public void StartGame()
    {
        // Ensure width and height do not exceed console buffer size
        int width = Math.Min(2000, Console.BufferWidth);
        int height = Math.Min(2000, Console.BufferHeight);
        Random rand = new Random();

        // Hide cursor for effect
        Console.CursorVisible = false;

        // Prepare flow heads and lengths for each column
        int[] heads = new int[width];
        int[] lengths = new int[width];

        // Initialize heads randomly, but keep lengths fixed for each column
        for (int i = 0; i < width; i++)
        {
            heads[i] = rand.Next(height);
            lengths[i] = rand.Next(4, 10); // Each column gets a fixed length between 4 and 9
        }

        Console.ForegroundColor = ConsoleColor.Green; // Set color once

        // Infinite loop for continuous effect
        while (true)
        {
            for (int col = 0; col < width; col++)
            {
                // Erase the trail (the last character of the previous flow)
                int tail = (heads[col] - lengths[col] + height) % height;
                if (col < Console.BufferWidth && tail < Console.BufferHeight)
                {
                    Console.SetCursorPosition(col, tail);
                    Console.Write(' ');
                }

                // Draw the new head
                int row = heads[col];
                if (col < Console.BufferWidth && row < Console.BufferHeight)
                {
                    Console.SetCursorPosition(col, row);
                    Console.Write(rand.Next(2));
                }

                // Move the head down
                heads[col] = (heads[col] + 1) % height;

                // Only randomize the head's position occasionally, not the length
                if (rand.NextDouble() < 0.01)
                {
                    heads[col] = rand.Next(height);
                }
            }

            //Thread.Sleep(1020);
        }
    }
}

class Program
{
    static void Main()
    {
        Matrix matrix = new Matrix();
        matrix.StartGame();
    }
}
