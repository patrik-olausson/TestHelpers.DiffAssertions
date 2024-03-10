using System;
using FakeItEasy;
using TestHelpers.DiffAssertions;
using TestHelpers.DiffAssertions.DefaultImplementations;
using Xunit;
using Xunit.Abstractions;

namespace DiffAsserterTests
{
    public class CompareStrings : DiffAsserterTestharness
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CompareStrings(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GivenArgumentsThatAreEqual_ThenTheTestSucceeds()
        {
            var sut = CreateSut();

            sut.CompareStrings("Hello", "Hello");
        }

        [Fact]
        public void GivenArgumentsThatAreEqual_ThenItDoesNotTryToStartDiffTool()
        {
            var diffTool = A.Fake<IDiffTool>();
            var sut = CreateSut(diffTool: diffTool);

            sut.CompareStrings("Hello", "Hello");

            A.CallTo(() =>
                diffTool.CompareFiles(A<ITestFile>.That.IsNotNull(), A<ITestFile>.That.IsNotNull()))
                .MustNotHaveHappened();
        }

        [Fact]
        public void GivenArgumentsThatAreDifferent_ThenItThrowsDiffAssertException()
        {
            var sut = CreateSut();

            var ex = Assert.Throws<DiffAssertException>(() => sut.CompareStrings("Hello", "Goodbye"));
            _testOutputHelper.WriteLine(ex.InnerException?.Message);
        }

        [Fact]
        public void GivenArgumentsThatAreDifferentAndIsAllowedToStartDiffTool_ThenItTriesToStartDiffTool()
        {
            var diffTool = A.Fake<IDiffTool>();
            var sut = CreateSut(diffTool:diffTool);

            try
            {
                sut.CompareStrings("Hello", "Goodbye");
            }
            catch (DiffAssertException)
            {
                A.CallTo(() => 
                    diffTool.CompareFiles(A<ITestFile>.That.IsNotNull(), A<ITestFile>.That.IsNotNull()))
                    .MustHaveHappened();
            }
        }

        [Fact]
        public void GivenThatTheAsserterThrowsAnExcpetionAndTheDiffToolIsUnableToBeUsed_ThenItJustRethrowsTheOriginalException()
        {
            var testFrameworkAsserter = A.Fake<ITestFrameworkAsserter>();
            A.CallTo(() => testFrameworkAsserter.Equals(A<string>.Ignored, A<string>.Ignored)).Throws<ArgumentException>();
            var diffTool = CreateFakeDiffTool(isUnableToUse:true);
            var sut = CreateSut(testFrameworkAsserter, diffTool);

            Assert.Throws<ArgumentException>(() => sut.CompareStrings("", ""));
        }

        [Fact]
        public void GivenThatTheAsserterThrowsAnExcpetionAndTheDiffToolAbleToBeUsed_ThenItThrowsDiffAssertException()
        {
            var testFrameworkAsserter = A.Fake<ITestFrameworkAsserter>();
            A.CallTo(() => testFrameworkAsserter.Equals(A<string>.Ignored, A<string>.Ignored)).Throws<ArgumentException>();
            var diffTool = CreateFakeDiffTool(isUnableToUse: false);
            var sut = CreateSut(testFrameworkAsserter, diffTool);

            Assert.Throws<DiffAssertException>(() => sut.CompareStrings("", ""));
        }
    }

    public class DiffAsserterTestharness
    {
        protected IDiffAsserter CreateSut(
            ITestFrameworkAsserter testFrameworkAsserter = null,
            IDiffTool diffTool = null,
            ITestFileManager fileManager = null)
        {
            return DiffAssert.CreateInstance(
                testFrameworkAsserter ?? new FluentAssertionsAsserter(), 
                diffTool ?? CreateFakeDiffTool(),
                fileManager ?? A.Fake<ITestFileManager>());
        }

        internal IDiffTool CreateFakeDiffTool(bool isUnableToUse = false)
        {
            var diffTool = A.Fake<IDiffTool>();
            A.CallTo(() => diffTool.IsUnableToUse).Returns(isUnableToUse);

            return diffTool;
        }
    } 
}
