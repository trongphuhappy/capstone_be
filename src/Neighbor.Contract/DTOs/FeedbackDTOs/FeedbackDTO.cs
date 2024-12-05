using Neighbor.Contract.DTOs.MemberDTOs;

namespace Neighbor.Contract.DTOs.FeedbackDTOs;

public class FeedbackDTO
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid AccountId { get; set; }
    public Guid ProductId { get; set; }
    public string Content { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public ProfileDTO Account { get; set; }
}
