using Microsoft.EntityFrameworkCore;
using Moq;
using ricoai.Data;
using ricoai.Models;
using ricoai.Repositories;
using ricoai.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace rico_photos_unittest
{
    /// <summary>
    /// Test using Moq and In-Memory DB.  The Moq will allow the DBContext to be 
    /// removed from the testing.  The In-Memory will allow testing things like 
    /// trying to add a entry into the database with an ID already used.
    /// </summary>
    public class UserImagesRepositoryTests
    {
        // List of images to share with all the test cases.
        private readonly List<UserImage> _imageList;

        /// <summary>
        /// Initialize the test with a list of images so this does not
        /// have to be repeated for each test case.
        /// </summary>
        public UserImagesRepositoryTests()
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
                    UserId = "3",
                    S3Path = "path/to/image/4.jpg",
                    S3ThumbPath = "path/to/image/Thumb_4.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 8,
                    UserId = "5",
                    S3Path = "path/to/image/5.jpg",
                    S3ThumbPath = "path/to/image/Thumb_5.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 9,
                    UserId = "9",
                    S3Path = "path/to/image/9.jpg",
                    S3ThumbPath = "path/to/image/Thumb_9.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 10,
                    UserId = "3",
                    S3Path = "path/to/image/10.jpg",
                    S3ThumbPath = "path/to/image/Thumb_10.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 11,
                    UserId = "11",
                    S3Path = "path/to/image/11.jpg",
                    S3ThumbPath = "path/to/image/Thumb_11.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 12,
                    UserId = "12",
                    S3Path = "path/to/image/12.jpg",
                    S3ThumbPath = "path/to/image/Thumb_12.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 13,
                    UserId = "13",
                    S3Path = "path/to/image/13.jpg",
                    S3ThumbPath = "path/to/image/Thumb_13.jpg",
                    IsPublic = true,
                },

                new UserImage
                {
                    id = 14,
                    UserId = "14",
                    S3Path = "path/to/image/14.jpg",
                    S3ThumbPath = "path/to/image/Thumb_14.jpg",
                    IsPublic = false,
                },

                new UserImage
                {
                    id = 15,
                    UserId = "15",
                    S3Path = "path/to/image/15.jpg",
                    S3ThumbPath = "path/to/image/Thumb_15.jpg",
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
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                Assert.All<UserImage>(image, item => Assert.Equal(userId, item.UserId));            // Verify all images have the correct User ID
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
            userImageRepoMock.Setup(s => s.GetAllUsersImageAsync(It.IsAny<string>())).ReturnsAsync((string s) => _imageList.FindAll(u => u.UserId == s));


            // Execute
            var image = await userImageRepoMock.Object.GetAllUsersImageAsync(userId);

            // Assert
            Assert.NotNull(image);                                                              // Verify we found data
            Assert.IsAssignableFrom<List<UserImage>>(image);                                    // Verify a list was obtained
            Assert.All<UserImage>(image, item => Assert.Equal(userId, item.UserId));            // Verify all images have the correct User ID
            Assert.Single(image);                                                               // Verify there was only 1 entry
        }

        /// <summary>
        /// Get all the user's images based on the UserID.
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void GetAllUserImagesBasedOnIdMultiple_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                string userId = "3";
                var imageRepository = new UserImagesRepository(dbContext);
                var images = await imageRepository.GetAllUsersImageAsync(userId);

                // Assert
                Assert.NotNull(images);// Verify we found data
                Assert.IsAssignableFrom<List<UserImage>>(images);                                    // Verify a list was obtained
                Assert.All<UserImage>(images, item => Assert.Equal(userId, item.UserId));            // Verify all images have the correct User ID
                Assert.Equal(_imageList.FindAll(u => u.UserId == userId).Count, images.Count);       // Verify there were multiple entries
            }
        }


        /// <summary>
        /// Get all the user's images based on the UserID.
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void GetAllUserImagesBasedOnIdMultiple_Moq()
        {

            // Setup
            string userId = "3";
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            userImageRepoMock.Setup(s => s.GetAllUsersImageAsync(It.IsAny<string>())).ReturnsAsync((string s) => _imageList.FindAll(u => u.UserId == s));


            // Execute
            var images = await userImageRepoMock.Object.GetAllUsersImageAsync(userId);

            // Assert
            Assert.NotNull(images);                                                              // Verify we found data
            Assert.IsAssignableFrom<List<UserImage>>(images);                                    // Verify a list was obtained
            Assert.All<UserImage>(images, item => Assert.Equal(userId, item.UserId));            // Verify all images have the correct User ID
            Assert.Equal(_imageList.FindAll(u => u.UserId == userId).Count, images.Count);       // Verify there were multiple entries
        }

        /// <summary>
        /// Get all the user's images based on the UserID.
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void GetLastTenPublic_Moq()
        {

            // Setup
            int maxCount = 10;
            List<UserImage> publicImages = _imageList.Where(ui => ui.IsPublic == true).ToList();
            List<UserImage> lastTenItems = publicImages.Skip(Math.Max(0, _imageList.Count() - maxCount-1)).ToList();
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            userImageRepoMock.Setup(s => s.GetLastTenPublicAsync()).ReturnsAsync(lastTenItems);


            // Execute
            var images = await userImageRepoMock.Object.GetLastTenPublicAsync();

            // Assert
            Assert.NotNull(images);                                                              // Verify we found data
            Assert.IsAssignableFrom<List<UserImage>>(images);                                    // Verify a list was obtained
            Assert.All<UserImage>(images, item => Assert.True(item.IsPublic));                   // Verify all images have the correct User ID
            Assert.Equal(10, images.Count);                                                      // Verify there were only 10 entry
        }


        /// <summary>
        /// Get all the user's images based on the UserID.
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void GetLastTenPublic_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                var imageRepository = new UserImagesRepository(dbContext);
                var images = await imageRepository.GetLastTenPublicAsync();

                // Assert
                Assert.NotNull(images);                                                              // Verify we found data
                Assert.IsAssignableFrom<List<UserImage>>(images);                                    // Verify a list was obtained
                Assert.All<UserImage>(images, item => Assert.True(item.IsPublic));                   // Verify all images have the correct User ID
                Assert.Equal(10, images.Count);                                                      // Verify there were only 10 entry
            }
        }



        /// <summary>
        /// Get an image based on the ID
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void GetImageFromId_Moq()
        {

            // Setup
            int id = 10;
            UserImage selectedImages = _imageList.Where(ui => ui.id == id).FirstOrDefault();
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            //userImageRepoMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(selectedImages);
            userImageRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => _imageList.Where(x => x.id == i).FirstOrDefault());

            // Execute
            var image = await userImageRepoMock.Object.GetByIdAsync(id);

            // Assert
            Assert.NotNull(image);                                                        // Verify we found data
            Assert.IsAssignableFrom<UserImage>(image);                                    // Verify it is an UserImage
            Assert.Equal(id, image.id);                                                   // Verify the ID
        }

        /// <summary>
        /// Get an image based on the ID
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void GetImageFromId_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                var imageRepository = new UserImagesRepository(dbContext);
                int id = 10;
                var image = await imageRepository.GetByIdAsync(id);

                // Assert
                Assert.NotNull(image);                                                        // Verify we found data
                Assert.IsAssignableFrom<UserImage>(image);                                    // Verify it is an UserImage
                Assert.Equal(id, image.id);                                                   // Verify the ID
            }
        }

        /// <summary>
        /// Get an image based on the ID
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void GetImageFromIdDoesNotExist_Moq()
        {

            // Setup
            int id = 100;
            UserImage selectedImages = _imageList.Where(ui => ui.id == id).FirstOrDefault();
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            //userImageRepoMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(selectedImages);
            userImageRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((int i) => _imageList.Where(x => x.id == i).FirstOrDefault());

            // Execute
            var image = await userImageRepoMock.Object.GetByIdAsync(id);

            // Assert
            Assert.Null(image);                                                        // Verify null returned
        }

        /// <summary>
        /// Remove the image.
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void RemoveImage_Moq()
        {

            // Setup
            int id = 10;
            UserImage selectedImages = _imageList.Where(ui => ui.id == id).FirstOrDefault();
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            userImageRepoMock.Setup(s => s.Remove(id));
            //userImageRepoMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(selectedImages);


            // Execute
            //var imageBefore = await userImageRepoMock.Object.GetByIdAsync(id);
            var result = await userImageRepoMock.Object.Remove(id);
            //var imageAfter = await userImageRepoMock.Object.GetByIdAsync(id);

            // Assert
            userImageRepoMock.Verify(s => s.Remove(id));
            //Assert.NotNull(imageBefore);            // Verify the ID existed
            //Assert.True(result);                    // Verify remove was successful
            //Assert.Null(imageAfter);                // Verify the ID does not exist
        }

        /// <summary>
        /// Remove the image.
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void RemoveImage_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                var imageRepository = new UserImagesRepository(dbContext);
                int id = 10;
                var imageBefore = await imageRepository.GetByIdAsync(id);
                var result = await imageRepository.Remove(id);
                var imageAfter = await imageRepository.GetByIdAsync(id);

                // Assert
                Assert.NotNull(imageBefore);            // Verify the ID existed
                Assert.True(result);                    // Verify remove was successful
                Assert.Null(imageAfter);                // Verify the ID does not exist
            }
        }

        /// <summary>
        /// Fail removing an image that does not exist.
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void RemoveImageFail_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                var imageRepository = new UserImagesRepository(dbContext);
                int id = 55;
                var imageBefore = await imageRepository.GetByIdAsync(id);
                var result = await imageRepository.Remove(id);
                var imageAfter = await imageRepository.GetByIdAsync(id);

                // Assert
                Assert.Null(imageBefore);               // Verify the ID does not exist
                Assert.False(result);                   // Verify remove was not successful
                Assert.Null(imageAfter);                // Verify the ID does not exist
            }
        }

        /// <summary>
        /// Insert an image.
        /// This will call the repository to get the images.
        /// This will use Moq to only test the repository.
        /// </summary>
        [Fact]
        public async void InsertAsync_Moq()
        {

            // Setup
            var newImage = new UserImage
            {
                id = 100,
                UserId = "66",
                S3Path = "path/to/image/66.jpg",
                S3ThumbPath = "path/to/image/Thumb_66.jpg",
                IsPublic = true,
            };
            var userImageRepoMock = new Mock<IUserImagesRepository>();
            userImageRepoMock.Setup(s => s.InsertAsync(It.IsAny<UserImage>())).ReturnsAsync((UserImage ui) => ui.id);

            // Execute
            var newImgId = await userImageRepoMock.Object.InsertAsync(newImage);

            // Assert
            Assert.Equal(newImage.id, newImgId);        // Verify remove was successful
        }

        /// <summary>
        /// Fail removing an image that does not exist.
        /// This will call the repository to get the images.
        /// This will use an In-Memory DbContext to simulate the database.
        /// </summary>
        [Fact]
        public async void InsertAsync_InMemory()
        {
            // Setup
            // Setup the DbContext and Options to use In memory DB
            var dbContextOptions = new DbContextOptionsBuilder<RicoaiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
                var newImage = new UserImage
                {
                    id = 100,
                    UserId = "66",
                    S3Path = "path/to/image/66.jpg",
                    S3ThumbPath = "path/to/image/Thumb_66.jpg",
                    IsPublic = true,
                };
                var imageRepository = new UserImagesRepository(dbContext);
                var imageBefore = await imageRepository.GetByIdAsync(newImage.id);
                var result = await imageRepository.InsertAsync(newImage);
                var imageAfter = await imageRepository.GetByIdAsync(newImage.id);

                // Assert
                Assert.Null(imageBefore);               // Verify the ID does not exist
                Assert.Equal(newImage.id, result);      // Verify the image was added
                Assert.NotNull(imageAfter);                // Verify the ID does exist
            }
        }

    }
}
