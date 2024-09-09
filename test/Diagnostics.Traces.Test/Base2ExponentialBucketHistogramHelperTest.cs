using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.Traces.Test
{
    [TestClass]
    public class Base2ExponentialBucketHistogramHelperTest
    {
        [TestMethod]
        public void CalculateLowerBoundary_PositiveScale_ShouldReturnCorrectValue()
        {
            // Arrange
            int index = 10;
            int scale = 2;

            // Act
            var result = Base2ExponentialBucketHistogramHelper.CalculateLowerBoundary(index, scale);

            // Assert
            Assert.IsTrue(result > double.Epsilon, "Lower boundary should be greater than double.Epsilon for positive scale.");
            Assert.AreEqual(Math.Exp(index * Math.ScaleB(Math.Log(2), -scale)), result, "Lower boundary calculation is incorrect for positive scale.");
        }

        [TestMethod]
        public void CalculateLowerBoundary_NegativeScale_SpecificCases_ShouldReturnEpsilonTimes2()
        {
            // Arrange
            int index1 = -537;
            int scale1 = -1;
            int index2 = -1074;
            int scale2 = 0;

            // Act
            var result1 = Base2ExponentialBucketHistogramHelper.CalculateLowerBoundary(index1, scale1);
            var result2 = Base2ExponentialBucketHistogramHelper.CalculateLowerBoundary(index2, scale2);

            // Assert
            Assert.AreEqual(double.Epsilon * 2, result1, "Lower boundary should be double.Epsilon * 2 for scale -1 and index -537.");
            Assert.AreEqual(double.Epsilon * 2, result2, "Lower boundary should be double.Epsilon * 2 for scale 0 and index -1074.");
        }

        [TestMethod]
        public void CalculateLowerBoundary_NegativeScale_ShouldReturnEpsilonForOutOfRange()
        {
            // Arrange
            int index = -538; // This should result in a value below double.Epsilon.
            int scale = -1;

            // Act
            var result = Base2ExponentialBucketHistogramHelper.CalculateLowerBoundary(index, scale);

            // Assert
            Assert.AreEqual(double.Epsilon, result, "Lower boundary should be double.Epsilon for very small index and negative scale.");
        }

        [TestMethod]
        public void CalculateLowerBoundary_NegativeScale_ShouldReturnValidBoundary()
        {
            // Arrange
            int index = -500;
            int scale = -2;

            // Act
            var result = Base2ExponentialBucketHistogramHelper.CalculateLowerBoundary(index, scale);

            // Assert
            Assert.AreEqual(5e-324, result, "Lower boundary calculation is incorrect for negative scale.");
        }

        [TestMethod]
        public void CalculateLowerBoundary_ZeroScale_ShouldReturnCorrectBoundary()
        {
            // Arrange
            int index = -100;

            // Act
            var result = Base2ExponentialBucketHistogramHelper.CalculateLowerBoundary(index, 0);

            // Assert
            Assert.IsTrue(result > double.Epsilon, "Lower boundary should be greater than double.Epsilon for zero scale.");
            Assert.AreEqual(Math.ScaleB(1, index), result, "Lower boundary calculation is incorrect for zero scale.");
        }

    }
}
