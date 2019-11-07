using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagCloud;

namespace TagCloud_Tests
{
    //�� ������ ��������� ���������� �����
    //TestMethod_Condition_ExpectedResult
    // � ���� ���� ������ �������� �� 
    // TestMethod_ExpectedResult_AtCondition
    //��� ���� ����� ��������� ����� �������� � ����������� �� �������� �����
    public class CircularCloudLayouter_Tests
    {
        public static CircularCloudLayouter cloudLayouter;
        [SetUp]
        public void Setup()
        {
            var center = new Point(0, 0);
            cloudLayouter = new CircularCloudLayouter(center);
        }

        [Test]
        public void SizeOfCloud_ReturnsSameSize_AtPositiveSize()
        {
            var size = new Size(10, 5);

            cloudLayouter.PutNextRectangle(size);
            var currentSize = cloudLayouter.SizeOfCloud;

            currentSize
                .Should()
                .Be(size);
        }

        [TestCase(10, 10, TestName = "WithMaxHeightAndWidth10")]
        [TestCase(50, 50, TestName = "WithMaxHeightAndWidth50")]
        public void ContainsRectanglesThatNotIntersectEachOther_AtAddingRectanglesList(int widthMax, int heightMax)
        {
            var sizes = new List<Size>();
            for (var width = 5; width < widthMax; width += 2)
                for (var height = 5; height < heightMax; height += 2)
                    sizes.Add(new Size(width, height));
            var rectangles = cloudLayouter.Rectangles;

            sizes.ForEach(s => cloudLayouter.PutNextRectangle(s));
            var isAnyIntersects = rectangles.Any(r1 => rectangles.Any(r2 => (r1 != r2 && r1.IntersectsWith(r2))));

            isAnyIntersects.Should().BeFalse();
        }

        [Test]
        public void ContainsTheSameSizes_AtAddingRectanglesList()
        {

            var sizes = new List<Size>();
            for (var width = 5; width < 50; width += 2)
                for (var height = 5; height < 50; height += 2)
                    sizes.Add(new Size(width, height));

            sizes.ForEach(s => cloudLayouter.PutNextRectangle(s));
            var rectangles = cloudLayouter.Rectangles.Select(rect => new Size(rect.Width, rect.Height));

            rectangles
                .Should()
                .BeEquivalentTo(sizes);
        }

        [TestFixture]
        public class PutNextRectangle
        {
            [SetUp]
            public void Setup()
            {
                var center = new Point(0, 0);
                cloudLayouter = new CircularCloudLayouter(center);
            }

            //���� ������� ����� ���� ��� �������� ���������, ���������� ��������� � ������
            [TestCase(5, 5)]
            [TestCase(20, 20)]
            public void ReturnSameSizeRectangle_AtPositiveSizes(int width, int height)
            {
                var size = new Size(width, height);

                var rectangleSize = cloudLayouter.PutNextRectangle(size).Size;

                rectangleSize
                    .Should()
                    .Be(size);
            }

            public static IEnumerable TestCases
            {
                get
                {
                    var testName = "ThrowsExceptionAt_";
                    yield return new TestCaseData(new Size(0, 10)).SetName(testName + "ZeroWidth");
                    yield return new TestCaseData(new Size(10, 0)).SetName(testName + "ZeroHeight");
                    yield return new TestCaseData(Size.Empty).SetName(testName + "EmptySize");
                }
            }
            //� ���� ���� ����� �� ������������ �������� ��������� �����, �� ����� ��� ���������
            //�� ��� ���� ������������ � �������� �� ���������
            [TestCaseSource(nameof(TestCases))]
            public void ThrowsException(Size size)
            {
                Action rectAdding = () => cloudLayouter.PutNextRectangle(size);

                rectAdding
                    .Should()
                    .Throw<ArgumentException>();
            }
        }
    }
}
