namespace Transkeeper.Models;

public class Transaction
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Amount of something.
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Timestamp
    /// </summary>
    public DateTime TransactionDate { get; set; }
}