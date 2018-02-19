using System;
using System.Linq;
using System.Reflection;
using TestHelpers.DiffAssertions.Settings;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    internal class MultiTestFrameworkAsserter : ITestFrameworkAsserter
    {
        private readonly Lazy<Action<string[]>> _equalsAction;

        public MultiTestFrameworkAsserter(TestFrameworkIdentifier testFrameworkIdentifier)
        {
            _equalsAction = new Lazy<Action<string[]>>(() => GetFrameworkEqualsAction(testFrameworkIdentifier));
        }

        public void Equals(string expected, string actual)
        {
            try
            {
                _equalsAction.Value(new[] {expected, actual});
            }
            catch (TargetInvocationException e)
            {
                throw e.GetBaseException();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when trying to invoke the test framework equals method.\nMake sure you have specified the correct framework in the json-settings file!", ex);
            }
        }

        private Action<string[]> GetFrameworkEqualsAction(TestFrameworkIdentifier testFrameworkIdentifier)
        {
            switch (testFrameworkIdentifier)
            {
                case TestFrameworkIdentifier.MsTest: return CreateEqualsActionForMsTest();
                case TestFrameworkIdentifier.xUnit: return CreateEqualsActionForNUnit();
                case TestFrameworkIdentifier.xUnit2: return CreateEqualsActionForXUnit2();
                case TestFrameworkIdentifier.nUnit: return CreateEqualsActionForNUnit();
                default:
                    throw new ArgumentOutOfRangeException(nameof(testFrameworkIdentifier), testFrameworkIdentifier, null);
            }
        }

        private Action<string[]> CreateEqualsActionForMsTest()
        {
            var type = Type.GetType("Microsoft.VisualStudio.TestTools.UnitTesting.Assert, Microsoft.VisualStudio.QualityTools.UnitTestFramework");
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;

            return args => type.InvokeMember("AreEqual", bindingFlags, null, null, args);
        }

        private Action<string[]> CreateEqualsActionForNUnit()
        {
            var type = Type.GetType("NUnit.Framework.Assert, nunit.framework");
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;

            return args => type.InvokeMember("AreEqual", bindingFlags, null, null, args);
        }

        private Action<string[]> CreateEqualsActionForXUnit()
        {
            var type = Type.GetType("Xunit.Assert, xunit.assert");
            var method = type.GetMethods().First(m => m.Name == "Equal" && m.IsGenericMethod && m.GetParameters().Length == 2);
            
            return args => method.MakeGenericMethod(typeof(string)).Invoke(null, args);
        }

        private Action<string[]> CreateEqualsActionForXUnit2()
        {
            var type = Type.GetType("Xunit.Assert, xunit.assert");
            var method = type.GetMethods().First(m => m.Name == "Equal" && m.GetParameters().Length == 2);
            
            return args => method.Invoke(null, args); ;
        }
    }
}