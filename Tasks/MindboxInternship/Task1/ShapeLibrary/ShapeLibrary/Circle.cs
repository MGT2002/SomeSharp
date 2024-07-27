namespace ShapeLibrary
{
    public class Circle : IShape
    {
        private double radius;

        public double Radius
        {
            get => radius;
            set
            {
                ThrowIfRadiusIsInvalid(value);
                radius = value;
            }
        }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public double GetArea()
        {
            return Math.PI * radius * radius;
        }

        private static void ThrowIfRadiusIsInvalid(double radius)
        {
            if (radius <= 0)
                throw new ArgumentException("The radius must be bigger than 0!",
                    nameof(radius));
        }
    }
}
