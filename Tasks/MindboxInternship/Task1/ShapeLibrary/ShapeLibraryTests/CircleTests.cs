using NUnit.Framework;
using ShapeLibrary;

namespace ShapeLibraryTests
{
    public class CircleTests
    {
        [TestCase(5, Math.PI * 25)]
        [TestCase(4.32, Math.PI * 4.32 * 4.32)]
        public void Circle_GetArea_ReturnsArea(double radius, double expectedResult)
        {
            IShape shape = new Circle(radius);

            double area = shape.GetArea();

            Assert.That(area, Is.EqualTo(expectedResult));
        }

        [TestCase(-1)]
        [TestCase(-254.123)]
        [TestCase(0)]
        public void Circle_InvalidRadius_ThrowsArgumentException(double radius)
        {
            Assert.Throws<ArgumentException>(() => { Circle c = new(radius); });
        }
    }
}
