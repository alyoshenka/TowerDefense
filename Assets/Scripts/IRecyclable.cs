/// <summary>
/// an object that needs to be deleted when the game is over
/// </summary>
public interface IRecyclable
{
    /// <summary>
    /// deallocate memory, destroy object
    /// </summary>
    void Recycle();
}
