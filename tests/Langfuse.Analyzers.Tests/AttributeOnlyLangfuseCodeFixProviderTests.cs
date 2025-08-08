using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using AnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<Langfuse.Analyzers.AttributeOnlyLangfuseAnalyzer>;
using CodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<Langfuse.Analyzers.AttributeOnlyLangfuseAnalyzer, Langfuse.Analyzers.AttributeOnlyLangfuseCodeFixProvider>;

namespace Langfuse.Analyzers.Tests;

public class AttributeOnlyLangfuseCodeFixProviderTests
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
                                                  }
                                                  
                                                  public class EventBody : IDisposable { public void Dispose() { } }
                                                  public class SpanBody : IDisposable { public void Dispose() { } }
                                                  public class GenerationBody : IDisposable { public void Dispose() { } }
                                              }
                                              """;

    [Fact]
    public async Task LANG001_UsingStatement_ShouldFixWithScopedVariant()
    {
        var source = TestAttributesCode + """

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

        var fixedSource = TestAttributesCode + """

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

        var expected = AnalyzerVerifier.Diagnostic("LANG001")
            .WithSpan(53, 22, 53, 33)
            .WithArguments("CreateEventScoped", "CreateEvent");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG001_UsingDeclaration_ShouldFixWithScopedVariant()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  using var span = trace.CreateSpan("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       using var span = trace.CreateSpanScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG001")
            .WithSpan(53, 32, 53, 42)
            .WithArguments("CreateSpanScoped", "CreateSpan");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_ExpressionStatement_ShouldWrapInUsingDeclaration()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  trace.CreateEventScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       using var eventScoped = trace.CreateEventScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(53, 15, 53, 32)
            .WithArguments("CreateEventScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_VariableDeclaration_ShouldAddUsingKeyword()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  var span = trace.CreateSpanScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       using var span = trace.CreateSpanScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(53, 26, 53, 42)
            .WithArguments("CreateSpanScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_ExplicitType_ShouldAddUsingKeyword()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  Test.GenerationBody gen = trace.CreateGenerationScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       using Test.GenerationBody gen = trace.CreateGenerationScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(53, 41, 53, 63)
            .WithArguments("CreateGenerationScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_WithUniqueVariableNaming_ShouldGenerateUniqueName()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  var eventScoped = "existing";
                                                  trace.CreateEventScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       var eventScoped = "existing";
                                                       using var eventScoped1 = trace.CreateEventScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(54, 15, 54, 32)
            .WithArguments("CreateEventScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_PreservesIndentationAndComments_ShouldMaintainFormatting()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  
                                                  // This is a comment
                                                  trace.CreateSpanScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       
                                                       // This is a comment
                                                       using var span = trace.CreateSpanScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(55, 15, 55, 31)
            .WithArguments("CreateSpanScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_ChangeToNonScopedVariant_ShouldReplaceMethodName()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  var evt = trace.CreateEventScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       var evt = trace.CreateEvent("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(53, 25, 53, 42)
            .WithArguments("CreateEventScoped");
        
        var test =
            new CSharpCodeFixTest<AttributeOnlyLangfuseAnalyzer, AttributeOnlyLangfuseCodeFixProvider, XUnitVerifier>()
            {
                TestCode = source,
                FixedCode = fixedSource,
                CodeActionIndex = 1,
                ExpectedDiagnostics = { expected }
            };
        await test.RunAsync();
    }

    [Fact]
    public async Task NestedScenario_ShouldFixCorrectMethod()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  using (var outer = trace.CreateEventScoped("outer"))
                                                  {
                                                      using (trace.CreateSpan("inner"))
                                                      {
                                                          // Valid usage
                                                      }
                                                  }
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       using (var outer = trace.CreateEventScoped("outer"))
                                                       {
                                                           using (trace.CreateSpanScoped("inner"))
                                                           {
                                                               // Valid usage
                                                           }
                                                       }
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG001")
            .WithSpan(55, 26, 55, 36)
            .WithArguments("CreateSpanScoped", "CreateSpan");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_LambdaExpression_ShouldWrapInUsing()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  System.Action action = () =>
                                                  {
                                                      trace.CreateGenerationScoped("test");
                                                  };
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       System.Action action = () =>
                                                       {
                                                           using var generation = trace.CreateGenerationScoped("test");
                                                       };
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(55, 19, 55, 41)
            .WithArguments("CreateGenerationScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [Fact]
    public async Task LANG002_MethodArgumentNaming_ShouldGenerateAppropriateVariableName()
    {
        var source = TestAttributesCode + """

                                          public class TestClass
                                          {
                                              public void TestMethod()
                                              {
                                                  var trace = new Test.LangfuseTrace();
                                                  trace.CreateEventScoped("test");
                                              }
                                          }
                                          """;

        var fixedSource = TestAttributesCode + """

                                               public class TestClass
                                               {
                                                   public void TestMethod()
                                                   {
                                                       var trace = new Test.LangfuseTrace();
                                                       using var eventScoped = trace.CreateEventScoped("test");
                                                   }
                                               }
                                               """;

        var expected = AnalyzerVerifier.Diagnostic("LANG002")
            .WithSpan(53, 15, 53, 32)
            .WithArguments("CreateEventScoped");
        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }
}