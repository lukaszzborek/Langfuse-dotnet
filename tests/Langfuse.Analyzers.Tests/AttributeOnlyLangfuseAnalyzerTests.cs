using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Langfuse.Analyzers.AttributeOnlyLangfuseAnalyzer>;

namespace Langfuse.Analyzers.Tests;

public class AttributeOnlyLangfuseAnalyzerTests
{
    private const string TestAttributesCode = """
                                              using System;

                                              namespace zborek.Langfuse.Attributes
                                              {
                                                  [AttributeUsage(AttributeTargets.Method)]
                                                  public class NonScopedMethodAttribute : Attribute
                                                  {
                                                      public string ScopedVariant { get; }
                                                      public NonScopedMethodAttribute(string scopedVariant) => ScopedVariant = scopedVariant;
                                                  }

                                                  [AttributeUsage(AttributeTargets.Method)]
                                                  public class ScopedMethodAttribute : Attribute
                                                  {
                                                      public string NonScopedVariant { get; set; }
                                                      public ScopedMethodAttribute() { }
                                                      public ScopedMethodAttribute(string nonScopedVariant) => NonScopedVariant = nonScopedVariant;
                                                  }
                                              }

                                              namespace Test
                                              {
                                                  public class LangfuseTrace
                                                  {
                                                      [zborek.Langfuse.Attributes.NonScopedMethod("CreateEventScoped")]
                                                      public EventBody CreateEvent(string name) => new EventBody();
                                                      
                                                      [zborek.Langfuse.Attributes.ScopedMethod("CreateEvent")]
                                                      public EventBody CreateEventScoped(string name) => new EventBody();
                                                      
                                                      [zborek.Langfuse.Attributes.NonScopedMethod("CreateSpanScoped")]
                                                      public SpanBody CreateSpan(string name) => new SpanBody();
                                                      
                                                      [zborek.Langfuse.Attributes.ScopedMethod("CreateSpan")]
                                                      public SpanBody CreateSpanScoped(string name) => new SpanBody();
                                                      
                                                      [zborek.Langfuse.Attributes.NonScopedMethod("CreateGenerationScoped")]
                                                      public GenerationBody CreateGeneration(string name) => new GenerationBody();
                                                      
                                                      [zborek.Langfuse.Attributes.ScopedMethod("CreateGeneration")]
                                                      public GenerationBody CreateGenerationScoped(string name) => new GenerationBody();
                                                      
                                                      public RegularMethod CreateRegularMethod(string name) => new RegularMethod();
                                                  }
                                                  
                                                  public class EventBody : IDisposable { public void Dispose() { } }
                                                  public class SpanBody : IDisposable { public void Dispose() { } }
                                                  public class GenerationBody : IDisposable { public void Dispose() { } }
                                                  public class RegularMethod { }
                                              }
                                              """;

    [Fact]
    public async Task CreateEvent_InUsingStatement_ShouldTriggerLANG001()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using (trace.CreateEvent("test"))
                                                {
                                                }
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG001")
            .WithSpan(56, 22, 56, 33)
            .WithArguments("CreateEventScoped", "CreateEvent");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CreateSpan_InUsingDeclaration_ShouldTriggerLANG001()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using var span = trace.CreateSpan("test");
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG001")
            .WithSpan(56, 32, 56, 42)
            .WithArguments("CreateSpanScoped", "CreateSpan");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }


    [Fact]
    public async Task MultipleNonScopedInUsing_ShouldTriggerMultipleLANG001()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using var event1 = trace.CreateEvent("test1");
                                                using var span1 = trace.CreateSpan("test2");
                                            }
                                        }
                                        """;

        var expected1 = Verifier.Diagnostic("LANG001")
            .WithSpan(56, 34, 56, 45)
            .WithArguments("CreateEventScoped", "CreateEvent");
        var expected2 = Verifier.Diagnostic("LANG001")
            .WithSpan(57, 33, 57, 43)
            .WithArguments("CreateSpanScoped", "CreateSpan");
        await Verifier.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task NonScopedInNestedUsing_ShouldTriggerLANG001()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using (var outer = trace.CreateEventScoped("outer"))
                                                {
                                                    using var inner = trace.CreateEvent("inner");
                                                }
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG001")
            .WithSpan(58, 37, 58, 48)
            .WithArguments("CreateEventScoped", "CreateEvent");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CreateEventScoped_WithoutUsing_ShouldTriggerLANG002()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                trace.CreateEventScoped("test");
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG002")
            .WithSpan(56, 15, 56, 32)
            .WithArguments("CreateEventScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CreateSpanScoped_AssignedToRegularVariable_ShouldTriggerLANG002()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                var span = trace.CreateSpanScoped("test");
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG002")
            .WithSpan(56, 26, 56, 42)
            .WithArguments("CreateSpanScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CreateGenerationScoped_ReturnedFromMethod_ShouldTriggerLANG002()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public Test.GenerationBody GetGeneration()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                return trace.CreateGenerationScoped("test");
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG002")
            .WithSpan(56, 22, 56, 44)
            .WithArguments("CreateGenerationScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task MultipleScopedWithoutUsing_ShouldTriggerMultipleLANG002()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                trace.CreateEventScoped("test1");
                                                var span = trace.CreateSpanScoped("test2");
                                            }
                                        }
                                        """;

        var expected1 = Verifier.Diagnostic("LANG002")
            .WithSpan(56, 15, 56, 32)
            .WithArguments("CreateEventScoped");
        var expected2 = Verifier.Diagnostic("LANG002")
            .WithSpan(57, 26, 57, 42)
            .WithArguments("CreateSpanScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task ScopedInExpressionStatement_ShouldTriggerLANG002()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                trace.CreateGenerationScoped("test").ToString();
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG002")
            .WithSpan(56, 15, 56, 37)
            .WithArguments("CreateGenerationScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task NonScopedMethod_NormalUsage_ShouldNotTriggerDiagnostic()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                var evt = trace.CreateEvent("test");
                                                trace.CreateSpan("test");
                                            }
                                        }
                                        """;

        await Verifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ScopedMethod_InUsingStatement_ShouldNotTriggerDiagnostic()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using (trace.CreateEventScoped("test"))
                                                {
                                                }
                                            }
                                        }
                                        """;

        await Verifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ScopedMethod_InUsingDeclaration_ShouldNotTriggerDiagnostic()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using var evt = trace.CreateEventScoped("test");
                                            }
                                        }
                                        """;

        await Verifier.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NonScopedInLambdaWithUsing_ShouldTriggerLANG001()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                System.Action action = () =>
                                                {
                                                    using var evt = trace.CreateEvent("test");
                                                };
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG001")
            .WithSpan(58, 35, 58, 46)
            .WithArguments("CreateEventScoped", "CreateEvent");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ScopedInLambdaWithoutUsing_ShouldTriggerLANG002()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                System.Action action = () =>
                                                {
                                                    trace.CreateEventScoped("test");
                                                };
                                            }
                                        }
                                        """;

        var expected = Verifier.Diagnostic("LANG002")
            .WithSpan(58, 19, 58, 36)
            .WithArguments("CreateEventScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ChainedMethodCalls_ShouldTriggerCorrectDiagnostics()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using var evt = trace.CreateEvent("test");
                                                var result = trace.CreateSpanScoped("test");
                                            }
                                        }
                                        """;

        var expected1 = Verifier.Diagnostic("LANG001")
            .WithSpan(56, 31, 56, 42)
            .WithArguments("CreateEventScoped", "CreateEvent");
        var expected2 = Verifier.Diagnostic("LANG002")
            .WithSpan(57, 28, 57, 44)
            .WithArguments("CreateSpanScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task NestedUsingStatements_ComplexScenario_ShouldTriggerCorrectDiagnostics()
    {
        var test = TestAttributesCode + """

                                        public class TestClass
                                        {
                                            public void TestMethod()
                                            {
                                                var trace = new Test.LangfuseTrace();
                                                using (var outer = trace.CreateEventScoped("outer"))
                                                {
                                                    using (trace.CreateSpan("inner"))
                                                    {
                                                        trace.CreateGenerationScoped("leaf");
                                                    }
                                                }
                                            }
                                        }
                                        """;

        var expected1 = Verifier.Diagnostic("LANG001")
            .WithSpan(58, 26, 58, 36)
            .WithArguments("CreateSpanScoped", "CreateSpan");
        var expected2 = Verifier.Diagnostic("LANG002")
            .WithSpan(60, 23, 60, 45)
            .WithArguments("CreateGenerationScoped");
        await Verifier.VerifyAnalyzerAsync(test, expected1, expected2);
    }
}