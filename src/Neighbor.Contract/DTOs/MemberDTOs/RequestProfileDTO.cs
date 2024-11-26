using Microsoft.AspNetCore.Http;
using Neighbor.Contract.Enumarations.Product;

namespace Neighbor.Contract.DTOs.MemberDTOs;

public static class RequestProfileDTO
{
    public class RequestUpdateAvatar
    {
        public IFormFile CropAvatar { get; set; }
        public IFormFile FullAvatar { get; set; }
    }

    public class RequestUpdateCoverPhoto
    {
        public IFormFile CropCoverPhoto { get; set; }
        public IFormFile FullCoverPhoto { get; set; }
    }

    public class RequestUpdateProfile
    {
        public string? Biography { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class RequestUdpateEmail
    {
        public string Email { get; set; }
    }

    public class RequestUpdateCitizen
    {
        public string CitizenId { get; set; }
        public IFormFile FrontImageCitizen { get; set; }
        public IFormFile BackImageCitizen { get; set; }
    }

    public class RequestUdpateInfoLessor
    {
        public string? WareHouseAddress { get; set; }
        public string? ShopName { get; set; }
        public LocationType? LocationType { get; set; }
    }
}
