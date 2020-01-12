namespace FSharpConsole

///<summary>
/// A record type.
/// </summary>
type RecordType = {
  StringValue:string
  IntValue:int
}


///<summary>
/// A union type.
/// </summary>
type UnionType =
    ///<summary>
    /// A typeless union case.
    /// </summary>
    | TypelessCase
    ///<summary>
    /// A string typed union case.
    /// </summary>
    | StringCase of string

///<summary>
/// A class type with constructor parameters.
/// </summary>
type ClassType (stringValue:string, intValue:int) =
    ///<summary>
    /// A string member.
    /// </summary>
    member this.StringValue = stringValue

    ///<summary>
    /// An int member.
    /// </summary>
    member this.IntValue = intValue
    
    ///<summary>
    /// Nullary instance function which returns the string form of <see cref="IntValue" /> and <see cref="StringValue" />. 
    /// </summary>
    /// <returns>
    /// The concatenated string value.
    /// </returns>
    member this.NullaryMethod () =
        this.IntValue.ToString() + this.StringValue
    ///<summary>
    /// Unary instance function which returns the sum of <see cref="IntValue" /> and <paramref name="value"/>. 
    /// </summary>
    /// <param name="value">A value.</param>
    /// <returns>
    /// The sum.
    /// </returns>
    member this.UnaryMethod value =
        this.IntValue + value
    ///<summary>
    /// Binary instance function which returns a tuple of: the concatenation of <see cref="StringValue" /> and <paramref name="stringValue"/>, and the sum of <see cref="IntValue" /> and <paramref name="intValue"/>. 
    /// </summary>
    /// <param name="tupleArgs">A tuple of <see cref="string" /> and <see cref="int" />.</param>
    /// <returns>
    /// The resulting tuple.
    /// </returns>
    member this.TupledBinaryMethod tupleArgs =
        let (stringValue,intValue) = tupleArgs
        (this.StringValue + stringValue, this.IntValue + intValue)
