﻿using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;

namespace Neighbor.Contract.Services.Members;

public static class Query
{
    public record GetProfileQuery(Guid UserId) : IQuery<Success<ProfileDTO>>;
}
