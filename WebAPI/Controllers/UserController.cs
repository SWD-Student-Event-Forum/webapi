﻿using EventZone.Domain.DTOs.BookedTicketDTOs;
using EventZone.Domain.DTOs.EventOrderDTOs;
using EventZone.Domain.DTOs.UserDTOs;
using EventZone.Domain.Enums;
using EventZone.Repositories.Commons;
using EventZone.Repositories.Models.UserAuthenModels;
using EventZone.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventZone.WebAPI.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IImageService _imageService;

        public UserController(IUserService userService, IEmailService emailService, IImageService imageService)
        {
            _userService = userService;
            _emailService = emailService;
            _imageService = imageService;
        }

        /// <summary>
        /// Registers a new user with the STUDENT role.
        ///
        /// INPUT THE IMAGE ONLY, DO NOT INPUT THE STRING URL
        /// </summary>
        /// <param name="userSignup">The user signup data.</param>
        /// <returns>A result object indicating success or failure, with additional information.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/users/register
        ///     {
        ///         "Email": "example@gmail.com",
        ///         "Password": "StrongPassword123",
        ///         "FullName": "John Doe",
        ///         "Dob": "2000-01-01",
        ///         "Gender": "Male",
        ///         "Image": "base64_encoded_image_data",
        ///         "University": "Example University"
        ///     }
        /// </remarks>
        /// <response code="200">Returns a success message with user data if registration is successful.</response>
        /// <response code="400">Returns an error message if registration fails (e.g., email already exists, invalid data).</response>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserSignupModel userSignup)
        {
            try
            {
                //string imageUrl = null;
                //if (userSignup.Image != null)
                //{
                //    imageUrl = await _imageService.UploadImageAsync(userSignup.Image, "event-thumbnails");
                //    userSignup.ImageUrl = imageUrl;
                //}
                var data = await _userService.ResigerAsync(userSignup, "STUDENT");
                if (data.IsSuccess)
                {
                    // var confirmationLink = Url.Action(nameof(ConfirmEmail), "users", new { email = userLogin.Email, token = data.Message }, Request.Scheme);
                    //var message = new Message(new string[] { data.Data.Email }, "Confirmation email link", confirmationLink!);
                    // await _emailService.SendEmail(message);
                    data.Message = "Added sucessfully <3";
                    return Ok(data);
                }

                return BadRequest(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Refreshes an access token using a refresh token.
        /// </summary>
        /// <param name="model">The token model containing the refresh token.</param>
        /// <returns>A result object with a new access token and refresh token if successful.</returns>
        /// <response code="200">Returns a new access token and refresh token.</response>
        /// <response code="400">Returns an error message if token refresh fails (e.g., invalid refresh token).</response>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel model)
        {
            try
            {
                var result = await _userService.RefreshToken(model);
                if (!result.IsSuccess)
                {
                    return Unauthorized(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Updates user account information.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="userUpdatemodel">The updated user data.</param>
        /// <param name="role">Optional new role for the user (if applicable).</param>
        /// <returns>A result object indicating success or failure.</returns>
        /// <remarks>
        /// Sample request body:
        ///
        ///     {
        ///         "FullName": "Updated Name",
        ///         "Dob": "2001-01-01",
        ///         "Gender": "Female",
        ///         "Image": "base64_encoded_image_data",
        ///         "University": "Updated University"
        ///     }
        /// </remarks>
        /// <response code="200">Returns a success message if the update is successful.</response>
        /// <response code="404">If the user with the specified ID is not found.</response>
        /// <response code="400">Returns an error message if the update fails (e.g., invalid data).</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount([FromRoute] Guid id, [FromBody] UserUpdateModel userUpdatemodel, [FromQuery] RoleEnums? role)
        {
            try
            {
                var newRole = RoleEnums.ADMIN.Equals(role) ? "" : role.ToString();

                var result = await _userService.UpdateAccountAsync(id, userUpdatemodel, newRole);
                if (result.IsSuccess == false)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a list of users by their IDs.
        /// </summary>
        /// <param name="id">A list of user IDs to delete.</param>
        /// <returns>A result object indicating success or failure, with the list of deleted user IDs if successful.</returns>
        /// <response code="200">Returns a success message with the list of deleted user IDs.</response>
        /// <response code="404">If none of the specified users are found.</response>
        /// <response code="400">Returns an error message if the deletion fails.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsersAsync(Guid id)
        {
            try
            {
                var result = await _userService.DeleteUser(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new manager user.
        /// </summary>
        /// <param name="newUser">The signup data for the new manager.</param>
        /// <returns>A result object indicating success or failure, with additional information.</returns>
        /// <response code="200">Returns a success message with user data if creation is successful.</response>
        /// <response code="400">Returns an error message if creation fails (e.g., email already exists, invalid data).</response>
        [HttpPost("manager")]
        public async Task<IActionResult> CreateUserManagerRoleAsync(UserSignupModel newUser)
        {
            try
            {
                var data = await _userService.CreateManagerAsync(newUser);
                if (data.IsSuccess)
                {
                    // var confirmationLink = Url.Action(nameof(ConfirmEmail), "users", new { email = userLogin.Email, token = data.Message }, Request.Scheme);
                    //var message = new Message(new string[] { data.Data.Email }, "Confirmation email link", confirmationLink!);
                    // await _emailService.SendEmail(message);
                    data.Message = "Created sucessfully <3";
                    return Ok(data);
                }

                return BadRequest(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Authenticates a user and returns an access token if successful.
        /// </summary>
        /// <param name="user">The user login credentials.</param>
        /// <remarks>
        /// Sample request body:
        ///     {
        ///         "Email": "admin@email.com",
        ///         "Password": "123456"
        ///     }
        /// </remarks>
        /// <response code="200">Returns an access token if authentication is successful.</response>
        /// <response code="401">Returns an error message if authentication fails (e.g., invalid credentials).</response>
        /// <response code="400">Returns an error message if the request is invalid.</response>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(UserLoginModel user)
        {
            try
            {
                var result = await _userService.LoginAsync(user);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Gets all users based on specified filters and pagination parameters.
        /// </summary>
        /// <param name="paginationParameter">Pagination parameters (page number, page size).</param>
        /// <param name="userFilterModel">Filters to apply (e.g., name, email).</param>
        /// <response code="200">Returns a paginated list of users and pagination metadata in the headers.</response>
        [HttpGet()]
        [HttpGet()] // lấy tất cả user theo paging và filter
        public async Task<IActionResult> GetAccountByFilters([FromQuery] PaginationParameter paginationParameter, [FromQuery] UserFilterModel userFilterModel)
        {
            try
            {
                var result = await _userService.GetUsersByFiltersAsync(paginationParameter, userFilterModel);
                if (result == null)
                {
                    return NotFound("No accounts found with the specified filters.");
                }
                var metadata = new
                {
                    result.TotalCount,
                    result.PageSize,
                    result.CurrentPage,
                    result.TotalPages,
                    result.HasNext,
                    result.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return Ok(ApiResult<Pagination<UserDetailsModel>>.Succeed(result, "Get list users successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get users by their specific id
        /// </summary>
        /// <response code="200">Returns an existing user</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventByIdAsync(Guid id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                return Ok(ApiResult<UserDetailsModel>.Succeed(user, "Get User Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        /// <summary>
        /// Gets the currently logged-in user's information.
        /// </summary>
        /// <returns>The currently logged-in user's data.</returns>
        /// <response code="200">Returns the currently logged-in user's data.</response>
        /// <response code="404">If the user is not found or not authenticated.</response>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var result = await _userService.GetCurrentUserAsync();
            if (result.IsSuccess || result.Data != null)
            {
                return Ok(result);
            }
            return Unauthorized(result);
        }

        [HttpGet("me/event-orders")]
        public async Task<IActionResult> GetCurrentUserOrders()
        {
            try
            {
                var result = await _userService.GetCurrentUserOrders();

                return Ok(ApiResult<List<EventOrderReponseDTO>>.Succeed(result, "Get List Order Of Event Current User Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        [HttpGet("me/booked-tickets")]
        public async Task<IActionResult> GetCurrentUserTickets()
        {
            try
            {
                var result = await _userService.GetAllBookedTicketOfCurrentUser();

                return Ok(ApiResult<List<BookedTicketDetailDTO>>.Succeed(result, "Get List Order Of Event Current User Successfully!"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<object>.Fail(ex));
            }
        }

        [HttpGet("confirm-email")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await _userService.ConfirmEmail(email, token);
            if (result)
            {
                return Ok(new { status = true, message = "oh yeah lmao u did it" });
            }
            return BadRequest(new { status = false, message = "bruh we ded 💀" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            var result = await _userService.ForgotPassword(email);
            if (result.IsSuccess)
            {
                var confirmationLink = "Code:\n\"" + result.Data + "\"";
                var message = new Domain.DTOs.EmailModels.Message(new string[] { email }, "Reset password token", confirmationLink!);
                await _emailService.SendEmail(message);
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email, string newPassword)
        {
            return Ok(await _userService.UserChangePasswordAsync(email, token, newPassword));
        }
    }
}

//[HttpGet("test-email")]
//public async Task<IActionResult> TestEmail()
//{
//    var message = new Message(new string[]
//    {
//        "manhdung5289@gmail.com"
//    },
//     "Test",
//     "<h1> Wassup bro ! </h1>"
//    );

//    await _emailService.SendEmail(message);

//    return Ok(new { status = "success", Message = " email sent" });
//}