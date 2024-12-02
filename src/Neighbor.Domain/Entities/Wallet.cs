using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Wallet : DomainEntity<Guid>
{
    public Wallet(Guid lessorId, int balance)
    {
        LessorId = lessorId;
        Balance = balance;
    }

    public Guid LessorId { get; private set; }
    public int Balance { get; private set; }
    
    public virtual Lessor? Lessor { get; private set; }
    public virtual List<Transaction>? Transactions { get; private set; }

    public static Wallet CreateWallet(Guid lessorId)
    {
        var wallet = new Wallet(lessorId, 0);
        return wallet;
    }

    public void AddMoney(DateTime startDate, DateTime endDate, int rentMoney, string description)
    {
        var money = (endDate - startDate).Days * rentMoney;
        Balance += money;
        // Create new transactions
        var transaction = Transaction.CreateTransactionWithTypeDeposit(money, description, Id);
        Transactions.Add(transaction);
    }

}
