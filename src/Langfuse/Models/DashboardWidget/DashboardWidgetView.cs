using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Data view a dashboard widget is built on.
/// </summary>
[JsonConverter(typeof(KebabCaseLowerEnumConverter<DashboardWidgetView>))]
public enum DashboardWidgetView
{
    /// <summary>
    ///     Observation-level data (spans, generations, events).
    /// </summary>
    Observations,

    /// <summary>
    ///     Numeric score data.
    /// </summary>
    ScoresNumeric,

    /// <summary>
    ///     Categorical score data.
    /// </summary>
    ScoresCategorical
}