using Neighbor.Contract.Services.Transactions;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Transaction : DomainEntity<Guid>
{
    public Transaction(long amount, TransactionType type, string description, Guid walletId)
    {
        Amount = amount;
        Type = type;
        Description = description;
        WalletId = walletId;
    }

    public long Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Description { get; private set; }
    public Guid WalletId { get; private set; }
    public virtual Wallet Wallet { get; private set; }

    public static Transaction CreateTransactionWithTypeDeposit(long amount, string description, Guid walletId)
    {
        return new Transaction(amount, TransactionType.Deposit, description, walletId);
    }

    public static Transaction CreateTransactionWithTypeRefund(long amount, string description, Guid walletId)
    {
        return new Transaction(amount, TransactionType.Refund, description, walletId);
    }
}
