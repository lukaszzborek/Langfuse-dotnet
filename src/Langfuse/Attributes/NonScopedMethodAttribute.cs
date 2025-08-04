namespace zborek.Langfuse.Attributes;

/// <summary>
/// Marks a method that should not be used in a using statement.
/// The analyzer will suggest using the scoped variant instead.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NonScopedMethodAttribute : Attribute
{
    /// <summary>
    /// The name of the scoped variant of this method
    /// </summary>
    public string ScopedVariant { get; }

    public NonScopedMethodAttribute(string scopedVariant)
    {
        ScopedVariant = scopedVariant;
    }
}