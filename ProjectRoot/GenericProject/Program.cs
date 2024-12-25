Enumerable.Range(1, 45).ToList().ForEach(i => Console.WriteLine(ClimbStairs(i)));

// to memoize the recursive function
int ClimbStairs(int n)
{
	int a = 0, b = 1, c = 0;
    for (int i = 0; i < n; i++)
	{
        c = a + b;
        a = b;
        b = c;
    }
    return c;
}
