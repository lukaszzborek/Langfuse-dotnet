namespace zborek.Langfuse.Attributes;

/// <summary>
///     Marks a method that must be used in a using statement.
///     The analyzer will enforce proper disposal.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ScopedMethodAttribute : Attribute
{
    /// <summary>
    ///     The name of the non-scoped variant of this method (optional)
    /// </summary>
    public string NonScopedVariant { get; set; }

    public ScopedMethodAttribute()
    {
    }

    public ScopedMethodAttribute(string nonScopedVariant)
    {
        NonScopedVariant = nonScopedVariant;
    }
}