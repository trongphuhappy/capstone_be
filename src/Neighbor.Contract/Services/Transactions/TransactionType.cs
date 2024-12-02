namespace Neighbor.Contract.Services.Transactions;

public enum TransactionType
{
    Deposit,      // Deposit money into the wallet
    Withdraw,     // Withdraw money from the wallet
    Transfer,     // Transfer money between wallets
    Payment,      // Payment for a product/service
    Refund,       // Refund
    Chargeback    // Money returned due to a dispute
}
