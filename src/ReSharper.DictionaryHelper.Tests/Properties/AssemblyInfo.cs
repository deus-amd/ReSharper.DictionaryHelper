using System.Collections.Generic;
using System.Reflection;
using JetBrains.Application;
using JetBrains.Threading;
using NUnit.Framework;
using ReSharper.DictionaryHelper;
#if RESHARPER9
using JetBrains.ReSharper.Resources.Shell;
#endif
#if RESHARPER9
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
[assembly: TestDataPathBase(@".\test\data")]

[ZoneDefinition]
public class TestEnvironmentZone : ITestsZone, IRequire<PsiFeatureTestZone>
{
}

#endif
/// <summary>
/// Test environment. Must be in the global namespace.
/// </summary>
// ReSharper disable once CheckNamespace
#if RESHARPER9
[SetUpFixture]
public class ReSharperTestEnvironmentAssembly : TestEnvironmentAssembly<TestEnvironmentZone>
#else
[SetUpFixture]
public class TestEnvironmentAssembly : ReSharperTestEnvironmentAssembly
#endif
{
    /// <summary>
    /// Gets the assemblies to load into test environment.
    /// Should include all assemblies which contain components.
    /// </summary>
    private static IEnumerable<Assembly> GetAssembliesToLoad()
    {
        // Test assembly
        yield return Assembly.GetExecutingAssembly();
        yield return typeof (DictionaryContainsKeyFix).Assembly;
    }

    public override void SetUp()
    {
        base.SetUp();
        ReentrancyGuard.Current.Execute(
            "LoadAssemblies",
            () => Shell.Instance.GetComponent<AssemblyManager>().LoadAssemblies(
                GetType().Name,
                GetAssembliesToLoad()));
    }

    public override void TearDown()
    {
        ReentrancyGuard.Current.Execute(
            "UnloadAssemblies",
            () => Shell.Instance.GetComponent<AssemblyManager>().UnloadAssemblies(
                GetType().Name,
                GetAssembliesToLoad()));
        base.TearDown();
    }
}
