using CryptoInfrastructure;
using Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UserCore.Services.Interfaces;
using UserCore.ViewModels.Respones;
using System.Drawing;
using System.IO;

namespace UserCore.Services.Implements
{
    public class UserServices : IUserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly UserDbContext _userDbContext;
        public UserServices(ILogger<UserServices> logger, UserDbContext userDbContext)
        {
            _logger = logger;
            _userDbContext = userDbContext;
        }
        public async Task<BaseRespone> AvatarUploadAsync(IFormFile avatar, string userId)
        {
            try
            {
                var response = new BaseRespone();
                var validationMessage = ValidateFile(avatar);
                if (validationMessage != Constants.StatusCode.FileValidated)
                {
                    response.IsSuccess = false;
                    response.Message = validationMessage;
                    return response;
                }
                // Here you would typically save the file to a storage service or database.
                var existingAvatar = await _userDbContext.UserFiles.FirstOrDefaultAsync(x => x.UserId == userId && x.FileType == "avatar");
                using (var memoryStream = new MemoryStream())
                {
                    await avatar.CopyToAsync(memoryStream);
                    if (existingAvatar != null)
                    {
                        existingAvatar.FileName = avatar.FileName;
                        existingAvatar.Content = memoryStream.ToArray();
                        existingAvatar.FileSize = avatar.Length;
                        existingAvatar.UploadedAt = DateTime.UtcNow;
                        _userDbContext.UserFiles.Update(existingAvatar);
                    }
                    else
                    {
                        UserFile userFile = new UserFile
                        {
                            UserId = userId,
                            FileName = avatar.FileName,
                            Content = memoryStream.ToArray(),
                            FileSize = avatar.Length,
                            UploadedAt = DateTime.UtcNow,
                            FileType = "avatar" // Assuming the file type is avatar for this example
                        };
                        _userDbContext.UserFiles.Add(userFile);
                    }
                    
                    var saveRecord = await _userDbContext.SaveChangesAsync();
                    if (saveRecord <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = "Failed to save the file.";
                        return response;
                    }
                }
                // For demonstration, we will just return a success message.
                response.IsSuccess = true;
                response.Message = "File uploaded successfully.";
                return response;
            }
            catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetAvatarAsync(string userId)
        {
            try
            {
                var userFile = await _userDbContext.UserFiles.FirstOrDefaultAsync(x => x.UserId == userId && x.FileType == "avatar");
                if (userFile == null)
                {
                    _logger.LogWarning("No avatar found for user {UserId}", userId);
                    return null;
                }
                return userFile.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving avatar for user {UserId}", userId);
                return null;
            }
        }

        private string ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "File is empty.";
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return "Invalid file type. Only JPG, JPEG, and PNG files are allowed.";
            }
            if (file.Length > 5 * 1024 * 1024) // 5 MB limit
            {
                return "File size exceeds the limit of 5 MB.";
            }
            return Constants.StatusCode.FileValidated;
        }

        private Image ConvertByteArrayToImage(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
