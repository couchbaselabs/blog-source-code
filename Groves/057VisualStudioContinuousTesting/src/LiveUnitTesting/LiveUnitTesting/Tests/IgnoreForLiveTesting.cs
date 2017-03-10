using NUnit.Framework;
using System;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace LiveUnitTesting.Tests
{
    // tag::IgnoreForLiveTesting[]
    public class IgnoreForLiveTesting : Attribute, ITestAction
    {
        readonly string _ignoreReason;

        public IgnoreForLiveTesting(string ignoreReason = null)
        {
            _ignoreReason = ignoreReason;
        }

        public ActionTargets Targets { get; set; }

        public void AfterTest(ITest test) { }

        public void BeforeTest(ITest test)
        {
            var isLiveTesting = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name == "Microsoft.CodeAnalysis.LiveUnitTesting.Runtime");
            if (isLiveTesting)
                Assert.Ignore(_ignoreReason ?? "Ignoring this test");
        }
    }
    // end::IgnoreForLiveTesting[]
}
