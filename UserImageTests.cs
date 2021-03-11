using Microsoft.EntityFrameworkCore;
using Moq;
using ricoai.Data;
using ricoai.Models;
using ricoai.Repositories;
using ricoai.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace rico_photos_unittest
{
    public class UserImageTests
    {
        private readonly List<UserImage> _imageList;

        public UserImageTests()
        {
            _imageList = new List<UserImage>
            {
                new UserImage
                {
                    id = 5,
                    UserId = "1",
                    S3Path = "path/to/image/1.jpg",
                    S3ThumbPath = "path/to/image/Thumb_1.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 6,
                    UserId = "3",
                    S3Path = "path/to/image/3.jpg",
                    S3ThumbPath = "path/to/image/Thumb_3.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 7,
                    UserId = "5",
                    S3Path = "path/to/image/5.jpg",
                    S3ThumbPath = "path/to/image/Thumb_5.jpg",
                    IsPublic = true,
                },
            };
        }


        /// <summary>
        /// Get all the user's images based on the UserID.
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void GetAllUserImagesBasedOnId_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: "RicoAiDatabase")
                .Options;

            // Populate the DbContext
            using (var dbContext = new RicoaiDbContext(dbContextOptions))
            {
                // Add all the images to the In memory DbContext
                foreach (var img in _imageList)
                {
                    dbContext.UserImage.Add(img);
                }

                dbContext.SaveChanges();
            }

            using (var dbContext = new RicoaiDbContext(dbContextOptions))
            {
                // Execute
                string userId = "5";
                var imageRepository = new UserImagesRepository(dbContext);
                var image = await imageRepository.GetAllUsersImageAsync(userId);

                // Assert
                Assert.NotNull(image);// Verify we found data
                Assert.IsAssignableFrom<List<UserImage>>(image);                                    // Verify a list was obtained
                Assert.Collection<UserImage>(image, item => Assert.Equal(userId, item.UserId));     // Verify all images have the correct User ID
                Assert.Single(image);                                                               // Verify there was only 1 entry
            }
        }

        /// <summary>
        /// Get all the user's images based on the UserID.
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void GetAllUserImagesBasedOnId_Moq()
        {

            // Setup
            string userId = "5";
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            userImageRepoMock.Setup(s => s.GetAllUsersImageAsync(userId)).ReturnsAsync(_imageList.FindAll(u => u.UserId == userId));


            // Execute
            var image = await userImageRepoMock.Object.GetAllUsersImageAsync(userId);

            // Assert
            Assert.NotNull(image);                                                              // Verify we found data
            Assert.IsAssignableFrom<List<UserImage>>(image);                                    // Verify a list was obtained
            Assert.Collection<UserImage>(image, item => Assert.Equal(userId, item.UserId));     // Verify all images have the correct User ID
            Assert.Single(image);                                                               // Verify there was only 1 entry
        }
    }
}
