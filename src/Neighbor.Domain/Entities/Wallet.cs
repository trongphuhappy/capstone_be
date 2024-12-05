using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Wallet : DomainEntity<Guid>
{
    public Wallet()
    {

    }
    public Wallet(Guid lessorId, int balance)
    {
        LessorId = lessorId;
        Balance = balance;
    }

    public Guid LessorId { get; private set; }
    public int Balance { get; private set; }
    
    public virtual Lessor? Lessor { get; private set; }
    public virtual List<Transaction>? Transactions { get; private set; } = new List<Transaction>();

    public static Wallet CreateWallet(Guid lessorId)
    {
        var wallet = new Wallet(lessorId, 0);
        return wallet;
    }

    public void AddMoney(int rentMoney, string description)
    {
        Balance += rentMoney;
        // Create new transactions
        var transaction = Transaction.CreateTransactionWithTypeDeposit(rentMoney, description, Id);
        Transactions.Add(transaction);
    }

    public void WithdrawMoney(int rentMoney, string description)
    {
        Balance -= rentMoney;
        // Create new transactions
        var transaction = Transaction.CreateTransactionWithTypeRefund(rentMoney, description, Id);
        Transactions.Add(transaction);
    }

    public static Wallet CreateWalletForOrderSuccessCommandHandlerTest(Guid lessorId)
    {
        return new Wallet()
        {
            LessorId = lessorId
        };
    }

}
