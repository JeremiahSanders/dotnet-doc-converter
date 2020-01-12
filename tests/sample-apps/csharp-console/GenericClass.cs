using System;

namespace CSharpConsole
{
  /// <summary>
  ///   A generic class.
  /// </summary>
  /// <typeparam name="TComparable">A type with constraint.</typeparam>
  public class GenericClass<TComparable> where TComparable : IComparable
  {
    /// <summary>
    ///   Creates a new instance.
    /// </summary>
    /// <param name="itemOne">An item.</param>
    /// <param name="itemTwo">Another item.</param>
    public GenericClass(TComparable itemOne, TComparable itemTwo)
    {
      ItemOne = itemOne;
      ItemTwo = itemTwo;
    }

    /// <summary>
    ///   Gets item one.
    /// </summary>
    public TComparable ItemOne { get; }

    /// <summary>
    ///   Gets item two.
    /// </summary>
    public TComparable ItemTwo { get; }

    /// <summary>
    ///   Compares <see cref="ItemOne" /> and <see cref="ItemTwo" />.
    /// </summary>
    /// <returns><see cref="IComparable.CompareTo" /> results.</returns>
    public int Compare()
    {
      return ItemOne.CompareTo(ItemTwo);
    }
  }
}
