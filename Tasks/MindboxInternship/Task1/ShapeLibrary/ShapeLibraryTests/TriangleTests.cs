using NUnit.Framework;
using ShapeLibrary;

namespace ShapeLibraryTests
{
    public class TriangleTests
    {
        [TestCase(3, 4, 5, 6)]
        [TestCase(6, 8, 10, 24)]
        [TestCase(5, 8, 5, 12)]
        public void Triangle_GetArea_Valid(double a, double b, double c,
            double expectedResult)
        {
            IShape shape = new Triangle(a, b, c);

            double area = shape.GetArea();

            Assert.That(area, Is.EqualTo(expectedResult));
        }

        [TestCase(3, 4, 5, true)]
        [TestCase(5, 12, 13, true)]
        [TestCase(5, 12, 12, false)]
        public void Triangle_IsRightAngle_ReturnsBool(double a, double b,
            double c, bool expectedResult)
        {
            // Arrange
            Triangle triangle = new(a, b, c);

            // Act & Assert
            Assert.That(triangle.IsRightAngle(), Is.EqualTo(expectedResult));
        }

        [TestCase(-1, 2, 3)]
        [TestCase(5.4, -254.123, 3)]
        [TestCase(1, 2, 0)]
        [TestCase(5, 10, 16)]
        public void Triangle_InvalidSides_ThrowsArgumentException(double a,
            double b, double c)
        {
            TestDelegate createInstance = () => { Triangle t = new(a, b, c); };

            Assert.Throws<ArgumentException>(createInstance);
        }
    }
}
