namespace Movies.Extensions
{
    public static class IntExtensions
    {
        public static int ToFibonacci(this int n)
        {
            if(n <= 1) return n;

            return ToFibonacci(n - 1) + ToFibonacci(n - 2);
        }
    }
}