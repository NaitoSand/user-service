using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace UserService.Api.V1.Controllers;

/// <summary>
/// Provides REST API endpoints for managing users.
/// </summary>
/// <remarks>
/// Supports CRUD operations:
/// - Retrieve all users
/// - Retrieve a user by ID
/// - Create a new user
/// - Update an existing user
/// - Delete a user
/// </remarks>
[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="service">The user service providing business logic.</param>
    public UsersController(IUserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Returns all registered users.
    /// </summary>
    /// <returns>List of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="204">No users found (empty list).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAllAsync()
        => ToActionResult(await _service.GetAllAsync());

    /// <summary>
    /// Returns a user by unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>User if found.</returns>
    /// <response code="200">User found and returned.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => ToActionResult(await _service.GetByIdAsync(id));

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>Created user with assigned identifier.</returns>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">Invalid input data.</response>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] User user)
        => ToActionResult(await _service.CreateAsync(user));

    /// <summary>
    /// Updates an existing user.  
    /// The user <c>Id</c> must be provided in the request body.
    /// </summary>
    /// <param name="user">The user entity with updated fields, including <c>Id</c>.</param>
    /// <returns>The updated user entity.</returns>
    /// <response code="200">User successfully updated.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="404">User not found.</response>
    [HttpPut]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync([FromBody] User user)
        => ToActionResult(await _service.UpdateAsync(user));

    /// <summary>
    /// Deletes a user by unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => ToActionResult(await _service.DeleteAsync(id));
}
