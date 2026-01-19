using HMS.Authentication.Application.Commands.Profile;
using HMS.Authentication.Domain.Entities;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, Result<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UpdateProfilePictureCommandHandler> _logger;

        public UpdateProfilePictureCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILogger<UpdateProfilePictureCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(
            UpdateProfilePictureCommand request,
            CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return Result<string>.Failure("No file provided");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return Result<string>.Failure("Invalid file type. Allowed: jpg, jpeg, png, gif");

            // Validate file size (5MB max)
            if (request.File.Length > 5 * 1024 * 1024)
                return Result<string>.Failure("File size exceeds 5MB limit");

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Result<string>.Failure("User not found");

            try
            {
                // In production, upload to cloud storage (Azure Blob, AWS S3, etc.)
                // For now, save locally
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{request.UserId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream, cancellationToken);
                }

                var fileUrl = $"/uploads/profiles/{uniqueFileName}";
                user.ProfilePictureUrl = fileUrl;
                user.UpdatedAt = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);

                return Result<string>.Success(fileUrl, "Profile picture updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture for user {UserId}", request.UserId);
                return Result<string>.Failure("Failed to upload profile picture");
            }
        }
    }
}
