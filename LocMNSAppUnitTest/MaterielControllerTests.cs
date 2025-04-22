using LocMNSApp.Controllers;
using LocMNSApp.Data;
using LocMNSApp.DTOs;
using LocMNSApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocMNSApp.Tests.Controllers
{
    [TestClass]
    public class MaterielControllerTests
    {
        private MaterielController _controller;
        private LocMNSAppDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<LocMNSAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new LocMNSAppDbContext(options);

            _context.Materiels.AddRange(
                new Materiel { Id = 1, NomMateriel = "Test1", ArchivateAt = null },
                new Materiel { Id = 2, NomMateriel = "Test2", ArchivateAt = DateTime.Now },
                new Materiel { Id = 3, NomMateriel = "Test3", ArchivateAt = null }
            );
            _context.SaveChanges();

            _controller = new MaterielController(_context);
        }

        /* [TestCleanup]
         public void Cleanup()
         {
             _context.Database.EnsureDeleted();
             _context.Dispose();
         }*/

        [TestMethod]
        public void Index_ReturnsViewResult_WithListOfMateriels()
        {
            // Act
            var result = _controller.Index() as ViewResult;
            var model = result?.Model as List<Materiel>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count); // Only 2 items with ArchivateAt == null
        }

        [TestMethod]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }



        [TestMethod]
        public void Edit_Get_ValidId_ReturnsViewResult_WithMaterielDto()
        {
            // Act
            var result = _controller.Edit(1) as ViewResult;
            var model = result?.Model as MaterielDto;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual("Test1", model.NomMateriel);
        }

        [TestMethod]
        public void Edit_Get_InvalidId_RedirectsToIndex()
        {
            // Act
            var result = _controller.Edit(999) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }


    }
}
