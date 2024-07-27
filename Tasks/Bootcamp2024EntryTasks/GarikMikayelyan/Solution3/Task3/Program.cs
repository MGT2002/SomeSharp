using Task3;

int[,] matrix = ReadMatrix("D:\\Garik\\VS\\Solution3\\Task3\\input.txt");
int n = matrix.GetLength(0);

Console.Write("Enter M (1 <= M <= 6 and M <= N({0})): ", n);
int m = int.Parse(Console.ReadLine()!);
if (!(m >= 1 && m <= 6 && m <= n))
    throw new ArgumentException("Wrong input", nameof(m));

int maxSum = FindMaxSum();
long maxProduct = FindMaxProduct();


Console.WriteLine($"Max Sum of {m} number sequence: {maxSum}");
Console.WriteLine($"Max product of {m} number sequence: {maxProduct}");

long FindMaxProduct()
{
    long max = 0;
    Product row = new(0);
    Product col = new(0);

    //MayorDiag
    var t1 = FindMaxProductInMayorDiagDirection(0, 0, n, row);
    if (max < t1)
        max = t1;

    for (int i = 1; i < n - m; i++)
    {
        t1 = FindMaxProductInMayorDiagDirection(i, 0, n - i, row);
        if (max < t1)
            max = t1;

        t1 = FindMaxProductInMayorDiagDirection(0, i, n - i, row);
        if (max < t1)
            max = t1;
    }

    //Minor Diag
    t1 = FindMaxProductInMinorDiagDirection(0, n - 1, n, row);
    if (max < t1)
        max = t1;

    for (int i = 1; i < n - m; i++)
    {
        t1 = FindMaxProductInMinorDiagDirection(0, n - 1 - i, n - i, row);
        if (max < t1)
            max = t1;

        t1 = FindMaxProductInMinorDiagDirection(i, n - 1, n - i, row);
        if (max < t1)
            max = t1;
    }

    //Horizontal, Vertical
    for (int i = 0; i < n; i++)
    {
        long t = FindMaxProductInRowAndCol(i, row, col);
        if (t > max)
        {
            max = t;
        }
    }

    return max;
}

long FindMaxProductInMayorDiagDirection(int startRow, int startCol,
    int diagLength, Product buffer)
{
    int si = startRow, sj = startCol;
    buffer.Clear(matrix[si, sj]);
    for (int i = 1; i < m; i++)
        buffer.Multyply(matrix[++si, ++sj]);
    si++; sj++;

    long max = buffer.Value;
    for (int i = m; i < diagLength; i++)
    {
        buffer.Divide(matrix[si - m, sj - m]);
        buffer.Multyply(matrix[si, sj]);
        si++; sj++;

        if (buffer.Value > max)
            max = buffer.Value;
    }

    return max;
}

long FindMaxProductInMinorDiagDirection(int startRow, int startCol,
    int diagLength, Product buffer)
{
    int si = startRow, sj = startCol;
    buffer.Clear(matrix[si, sj]);
    for (int i = 1; i < m; i++)
        buffer.Multyply(matrix[++si, --sj]);
    si++; sj--;

    long max = buffer.Value;
    for (int i = m; i < diagLength; i++)
    {
        buffer.Divide(matrix[si - m, sj + m]);
        buffer.Multyply(matrix[si, sj]);
        si++; sj--;

        if (buffer.Value > max)
            max = buffer.Value;
    }

    return max;
}

long FindMaxProductInRowAndCol(int rowAndCol, Product currentRow, Product currentCol)
{
    currentRow.Clear(matrix[rowAndCol, 0]);
    currentCol.Clear(matrix[0, rowAndCol]);

    for (int i = 1; i < m; i++)
    {
        currentRow.Multyply(matrix[rowAndCol, i]);
        currentCol.Multyply(matrix[i, rowAndCol]);
    }

    long max = currentRow.Value;
    for (int i = m; i < n; i++)
    {
        currentRow.Divide(matrix[rowAndCol, i - m]);
        currentRow.Multyply(matrix[rowAndCol, i]);

        if (currentRow.Value > max)
            max = currentRow.Value;

        currentCol.Divide(matrix[i - m, rowAndCol]);
        currentCol.Multyply(matrix[i, rowAndCol]);

        if (currentCol.Value > max)
            max = currentCol.Value;
    }

    return max;
}

int FindMaxSum()
{
    int max = int.MinValue;

    //MinorDiag
    var t1 = FindMaxInMinorDiagDirection(0, n - 1, n);
    if (max < t1)
        max = t1;

    for (int i = 1; i < n - m; i++)
    {
        t1 = FindMaxInMinorDiagDirection(0, n - 1 - i, n - i);
        if (max < t1)
            max = t1;

        t1 = FindMaxInMinorDiagDirection(i, n - 1, n - i);
        if (max < t1)
            max = t1;
    }

    //MayorDiag
    t1 = FindMaxInMayorDiagDirection(0, 0, n);
    if (max < t1)
        max = t1;

    for (int i = 1; i < n - m; i++)
    {
        t1 = FindMaxInMayorDiagDirection(i, 0, n - i);
        if (max < t1)
            max = t1;

        t1 = FindMaxInMayorDiagDirection(0, i, n - i);
        if (max < t1)
            max = t1;
    }

    //Horizontal, Vertical
    for (int i = 0; i < n; i++)
    {
        var t = FindMaxInRowAndCol(i);
        if (t.maxRow > max)
            max = t.maxRow;
        if (t.maxCol > max)
            max = t.maxCol;
    }

    return max;
}

int FindMaxInMinorDiagDirection(int startRow, int startCol, int diagLength)
{
    int max = 0, si = startRow, sj = startCol;
    for (int i = 0; i < m; i++)
        max += matrix[si++, sj--];

    int current = max;
    for (int i = m; i < diagLength; i++)
    {
        current -= matrix[si - m, sj + m];
        current += matrix[si, sj];
        si++; sj--;

        if (current > max)
            max = current;
    }

    return max;
}

int FindMaxInMayorDiagDirection(int startRow, int startCol, int diagLength)
{
    int max = 0, si = startRow, sj = startCol;
    for (int i = 0; i < m; i++)
        max += matrix[si++, sj++];

    int current = max;
    for (int i = m; i < diagLength; i++)
    {
        current -= matrix[si - m, sj - m];
        current += matrix[si, sj];
        si++; sj++;

        if (current > max)
            max = current;
    }

    return max;
}

(int maxRow, int maxCol) FindMaxInRowAndCol(int rowAndColIndex)
{
    int maxR = 0, maxC = 0;

    for (int i = 0; i < m; i++)
    {
        maxR += matrix[rowAndColIndex, i];
        maxC += matrix[i, rowAndColIndex];
    }

    int currentR = maxR, currentC = maxC;
    for (int i = m; i < n; i++)
    {
        currentR -= matrix[rowAndColIndex, i - m];
        currentR += matrix[rowAndColIndex, i];
        if (currentR > maxR)
            maxR = currentR;

        currentC -= matrix[i - m, rowAndColIndex];
        currentC += matrix[i, rowAndColIndex];

        if (currentC > maxC)
            maxC = currentC;
    }

    return (maxR, maxC);
}

static int[,] ReadMatrix(string filename)
{
    int[] nums = File.ReadAllText(filename).Split(" ").Select(int.Parse).ToArray();
    int n = (int)Math.Sqrt(nums.Length);
    int[,] matrix = new int[n, n];
    int counter = 0;

    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            matrix[i, j] = nums[counter++];

    return matrix;
}

