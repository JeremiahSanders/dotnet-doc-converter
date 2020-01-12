namespace CSharpConsole
{
  /// <summary>
  ///   A static class.
  /// </summary>
  public static class StaticClass
  {
    /// <summary>
    ///   A string constant.
    /// </summary>
    public const string ConstantString = "constant string";

    /// <summary>
    ///   A static readonly string field.
    /// </summary>
    public static readonly string ReadonlyField = "static readonly string";

    /// <summary>
    ///   A static get-only property.
    /// </summary>
    public static string ReadonlyProperty { get; } = "static string property";

    /// <summary>
    ///   A static mutable property.
    /// </summary>
    public static int MutableProperty { get; set; }

    /// <summary>
    ///   A static, nullary method.
    /// </summary>
    /// <returns>The answer.</returns>
    public static int NullaryMethod()
    {
      return 42;
    }

    /// <summary>
    ///   A static method with one parameter.
    /// </summary>
    /// <param name="value">A string.</param>
    /// <returns>Identity.</returns>
    public static string UnaryParameterMethod(string value)
    {
      return value;
    }

    /// <summary>
    ///   A static method with one generic parameter.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <typeparam name="T">A type.</typeparam>
    /// <returns>Identity.</returns>
    public static T UnaryGenericParameterMethod<T>(in T value)
    {
      return value;
    }
  }
}
