using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Tests.Models;

public class EvaluationRuleMappingSourceToolCallsTests
{
    [Fact]
    public void Should_Serialize_ToolCalls_To_Snake_Case()
    {
        JsonSerializer.Serialize(EvaluationRuleMappingSource.Tool_Calls).ShouldBe("\"tool_calls\"");
    }

    [Fact]
    public void Should_Deserialize_ToolCalls_From_Snake_Case()
    {
        JsonSerializer.Deserialize<EvaluationRuleMappingSource>("\"tool_calls\"")
            .ShouldBe(EvaluationRuleMappingSource.Tool_Calls);
    }
}