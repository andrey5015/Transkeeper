using Transkeeper.Models;

namespace Transkeeper.Abstractions;

public interface IRepository<in T> where T : class
{
    /// <summary>
    /// Saves transaction.
    /// </summary>
    /// <param name="item">Transaction to save.</param>
    /// <returns>Indicates success of the operation.</returns>
    public bool TrySave(T item);

    /// <summary>
    /// Retrieves transaction by Id.
    /// </summary>
    /// <param name="id">Id of the transaction.</param>
    /// <returns>Null if not found.</returns>
    public Transaction? GetById(int id);
}
