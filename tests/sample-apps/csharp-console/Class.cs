using System;

namespace CSharpConsole
{
  /// <summary>
  ///   A class.
  /// </summary>
  /// <remarks>
  ///   This class contains a variety of members.
  /// </remarks>
  public class Class
  {
    /// <summary>
    ///   A string constant.
    /// </summary>
    public const string ConstantString = "constant string";

    /// <summary>
    ///   A static readonly string field.
    /// </summary>
    /// <remarks>
    ///   TODO: Consider moving initialization of static fields into static constructor.
    /// </remarks>
    public static readonly string StaticReadonlyField = "static readonly string";

    /// <summary>
    ///   A readonly string field.
    /// </summary>
    public readonly string InstanceReadonlyField = "readonly string";

    /// <summary>
    ///   A static get-only property.
    /// </summary>
    public static string StaticReadonlyProperty { get; } = "static string property";

    /// <summary>
    ///   A static mutable property.
    /// </summary>
    public static int StaticMutableProperty { get; set; }

    /// <summary>
    ///   A get-only property.
    /// </summary>
    public string InstanceReadonlyProperty { get; } = "string property";

    /// <summary>
    ///   A mutable property.
    /// </summary>
    public int InstanceMutableProperty { get; set; }

    /// <summary>
    ///   A static, nullary method.
    /// </summary>
    /// <returns>The answer.</returns>
    public static int StaticNullaryMethod()
    {
      return 42;
    }

    /// <summary>
    ///   A static method with one parameter.
    /// </summary>
    /// <param name="value">A string.</param>
    /// <returns>Identity.</returns>
    public static string StaticUnaryParameterMethod(string value)
    {
      return value;
    }

    /// <summary>
    ///   A static method with one generic parameter.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <typeparam name="T">A type.</typeparam>
    /// <returns>Identity.</returns>
    public static T StaticUnaryGenericParameterMethod<T>(in T value)
    {
      return value;
    }

    /// <summary>
    ///   A nullary method.
    /// </summary>
    /// <remarks>IDE spell check rejects &quot;nullary&quot;.</remarks>
    /// <returns>The mutable property value.</returns>
    public int InstanceNullaryMethod()
    {
      return InstanceMutableProperty;
    }

    /// <summary>
    ///   A method with one parameter.
    /// </summary>
    /// <param name="value">A string.</param>
    /// <returns>Identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value" /> is null.</exception>
    public string InstanceUnaryParameterMethod(string value)
    {
      if (value == null) throw new ArgumentNullException(nameof(value));
      return InstanceMutableProperty + value;
    }

    /// <summary>
    ///   A method with one generic parameter.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <typeparam name="T">A type.</typeparam>
    /// <returns>Identity.</returns>
    public T InstanceUnaryGenericParameterMethod<T>(in T value)
    {
      return value;
    }
  }
}
