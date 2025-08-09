namespace zborek.Langfuse.Attributes;

/// <summary>
///     Marks a method that must be used in a using statement.
///     The analyzer will enforce proper disposal.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class ScopedMethodAttribute : Attribute
{
    /// <summary>
    ///     The name of the non-scoped variant of this method
    /// </summary>
    public string NonScopedVariant { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nonScopedVariant">The name of the non-scoped variant of this method</param>
    public ScopedMethodAttribute(string nonScopedVariant)
    {
        NonScopedVariant = nonScopedVariant;
    }
}