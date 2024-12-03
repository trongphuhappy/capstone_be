using Neighbor.Contract.DTOs.AuthenticationDTOs;

namespace Neighbor.Contract.Abstractions.Services;

public interface IGoogleOAuthService
{
    Task<GoogleUserInfoDTO> ValidateTokenAsync(string AccessTokenGoogle);
}
