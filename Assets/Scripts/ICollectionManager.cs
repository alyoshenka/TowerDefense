using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages a collection of objects
/// </summary>
/// <typeparam name="T">the enum type that the colllection shares</typeparam>
public interface ICollectionManager<T>
{
    /// <summary>
    /// get the object of the given type from the collection
    /// </summary>
    ICollectionObject Get(T type);
}

/// <summary>
/// an object in an icollectionmanager list
/// </summary>
public interface ICollectionObject
{
    // not sure what this does
    // is this even good design?
}
