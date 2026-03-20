using Aplication.Ports.In;
using Aplication.Ports.Out;
using Domain.Models;
using Domain.Services;

namespace Aplication.UseCases
{
    /// <summary>
    ///     This class implements the use case for managing products.
    /// </summary>
    public class ProductUseCase : IProductUseCasePort
    {
        private readonly IProductRepositoryPort _repository;
        private readonly ProductService _productService;

        public ProductUseCase(
            IProductRepositoryPort repository,
            ProductService productService)
        {
            _repository = repository;
            _productService = productService;
        }

        /// <summary>
        ///     This method creates a new product and saves it to the database.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product?> CreateAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdateAt = DateTime.UtcNow; 

            _productService.EvaluateStockStatus(product);

            return await _repository.SaveAsync(product);
        }

        /// <summary>
        ///     This method gets a product by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Product?> GetByIdAsync(Guid id) =>
            await _repository.GetByIdAsync(id);

        /// <summary>
        ///     This method gets all products from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _repository.GetAllAsync();

        /// <summary>
        ///     This method updates a product by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task<Product?> UpdateAsync(Guid id, Product product)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing is null)
                return null;

            product.Id = id;
            product.UpdateAt = DateTime.UtcNow;

            _productService.EvaluateStockStatus(product);

            return await _repository.UpdateAsync(product);
        }

        /// <summary>
        ///     This method deletes a product by id from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Guid id) =>
            await _repository.DeleteAsync(id);

        /// <summary>
        ///     This method registers an exit of a product.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<Product?> RegisterExitAsync(Guid id, int quantity)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product is null) return null;

            var updated = _productService.RegisterExit(quantity, product);
            return await _repository.UpdateAsync(updated);
        }
    }
}
