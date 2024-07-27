namespace ShapeLibrary
{
    public class Triangle : IShape
    {
        private double a;
        private double b;
        private double c;

        public double A
        {
            get => a;
            set
            {
                ThrowIfSideIsLessOrEqualZero(value);
                ThrowIfTriangleDoesNotExist(value, B, C);
                a = value;
            }
        }
        public double B
        {
            get => b;
            set
            {
                ThrowIfSideIsLessOrEqualZero(value);
                ThrowIfTriangleDoesNotExist(value, A, C);
                b = value;
            }
        }
        public double C
        {
            get => c;
            set
            {
                ThrowIfSideIsLessOrEqualZero(value);
                ThrowIfTriangleDoesNotExist(value, A, B);
                c = value;
            }
        }

        public Triangle(double sideA, double sideB, double sideC)
        {
            ThrowIfTriangleDoesNotExist(sideA, sideB, sideC);

            ThrowIfSideIsLessOrEqualZero(a = sideA);
            ThrowIfSideIsLessOrEqualZero(b = sideB);
            ThrowIfSideIsLessOrEqualZero(c = sideC);           
        }

        public double GetArea()
        {
            double halfP = (a + b + c) / 2;
            return Math.Sqrt(halfP * (halfP - a) * (halfP - b) * (halfP - c));
        }

        public bool IsRightAngle()
        {
            return a * a + b * b == c * c;
        }

        private static void ThrowIfSideIsLessOrEqualZero(double side)
        {
            if (side <= 0)
                throw new ArgumentException("The side must be bigger than 0!",
                    nameof(side));
        }
        public static void ThrowIfTriangleDoesNotExist(double sideA, double sideB, double sideC)
        {
            if (!(sideA + sideB > sideC &&
                sideA + sideC > sideB &&
                sideB + sideC > sideA))
                throw new ArgumentException("Triangle with the sides doesn't exist!");
        }
    }
}
