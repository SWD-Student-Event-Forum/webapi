﻿using AutoMapper;
using EventZone.Domain.DTOs.BookedTicketDTOs;
using EventZone.Domain.DTOs.EventOrderDTOs;
using EventZone.Domain.DTOs.UserDTOs;
using EventZone.Repositories.Commons;
using EventZone.Repositories.Interfaces;
using EventZone.Repositories.Models.UserAuthenModels;
using EventZone.Services.Interface;

namespace EventZone.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<List<UserDetailsModel>> GetAllUsers()
        {
            //var Users = await _unitOfWork.UserRepository.GetAllUsersAsync();
            //var result = new List<UserDetailsModel>();
            //foreach (var User in Users)
            //{
            //    var roleName = await _unitOfWork.UserRepository.GetRoleName(User);
            //    var lmao = _mapper.Map<UserDetailsModel>(User);
            //    lmao.RoleName = roleName;
            //    result.Add(lmao);
            //}

            var result = await _unitOfWork.UserRepository.GetAllUsersWithRoleAsync();

            return result;
        }

        public Task<UserDetailsModel> GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDetailsModel> GetUserById(Guid id)
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            var result = _mapper.Map<UserDetailsModel>(existingUser);
            return result;
        }

        public async Task<ApiResult<UserDetailsModel>> ResigerAsync(UserSignupModel UserLogin, string role)
        {
            var result = await _unitOfWork.UserRepository.AddUser(UserLogin, role);
            if (result == null)
            {
                return new ApiResult<UserDetailsModel>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "User have been existed"
                };
            }

            //  var token = await _unitOfWork.UserRepository.GenerateEmailConfirmationToken(result);

            return new ApiResult<UserDetailsModel>
            {
                Data = _mapper.Map<UserDetailsModel>(result),
                IsSuccess = true,
                Message = "Register Successfuly"
            };
        }

        public async Task<ApiResult<ResponseLoginModel>> RefreshToken(TokenModel token)
        {
            var result = await _unitOfWork.UserRepository.RefreshToken(token);
            if (result == null)
            {
                return ApiResult<ResponseLoginModel>.Error(null, "Invalid token or account");
            }
            return ApiResult<ResponseLoginModel>.Succeed(result, "Refresh token successfully");
        }

        public async Task<ApiResult<UserDetailsModel>> UpdateStudentProfileAsync(Guid userId, UserUpdateModel userUpdateMode)
        {
            var existingUser = await _unitOfWork.UserRepository.GetAccountDetailsAsync(userId);
            if (existingUser != null)
            {
                existingUser = _mapper.Map(userUpdateMode, existingUser);
                var updatedAccount = await _unitOfWork.UserRepository.UpdateAccountAsync(existingUser);

                if (updatedAccount != null)
                {
                    var response = new ApiResult<UserDetailsModel>();
                    response.Data = _mapper.Map<UserDetailsModel>(existingUser);
                    response.Message = "Updated user successfully";
                    response.IsSuccess = true;
                    return response;
                }
            }

            return new ApiResult<UserDetailsModel>
            {
                Data = null,
                IsSuccess = false,
                Message = "This user is not existed"
            };
        }

        public async Task<ApiResult<UserDetailsModel>> UpdateAccountAsync(Guid userId, UserUpdateModel userUpdateMode, string role)
        {
            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetAccountDetailsAsync(userId);
                if (existingUser != null)
                {
                    existingUser = _mapper.Map(userUpdateMode, existingUser);
                    // exiistingUser.RoleId = EnumHelper.ConvertToRoleId(accountUpdateModel.Role);
                    var updatedAccount = await _unitOfWork.UserRepository.UpdateAccountAsync(existingUser);

                    if (updatedAccount != null)
                    {
                        var response = new ApiResult<UserDetailsModel>();
                        response.Data = _mapper.Map<UserDetailsModel>(existingUser);
                        response.Message = "Updated user successfully";
                        response.IsSuccess = true;
                        if (!string.IsNullOrEmpty(role))
                        {
                            var result = await _unitOfWork.UserRepository.UpdateUserRole(existingUser, role);
                            if (result.Equals(role))
                            {
                                // response.Data.RoleName = role;
                                response.Message = "Updated user and role Successfuly";
                            }
                            else
                            {
                                response.Message = "Updated user successfully but role are not changed";
                            }
                        }
                        return response;
                    }
                }

                return new ApiResult<UserDetailsModel>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "This user is not existed"
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResult<UserDetailsModel>> CreateManagerAsync(UserSignupModel UserLogin)
        {
            var result = await _unitOfWork.UserRepository.AddUser(UserLogin, "MANAGER");
            if (result == null)
            {
                return new ApiResult<UserDetailsModel>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "User have been existed"
                };
            }

            //  var token = await _unitOfWork.UserRepository.GenerateEmailConfirmationToken(result);

            return new ApiResult<UserDetailsModel>
            {
                Data = _mapper.Map<UserDetailsModel>(result),
                IsSuccess = true,
                Message = ""
            };
        }

        public async Task<ApiResult<UserDetailsModel>> GetCurrentUserAsync()
        {
            var result = await _unitOfWork.UserRepository.GetCurrentUserAsync();
            if (result != null)
            {
                var role = await _unitOfWork.UserRepository.GetRole(result);
                var data = _mapper.Map<UserDetailsModel>(result);
                data.Role = role;
                return ApiResult<UserDetailsModel>.Succeed(data, "This is current user");
            }

            return ApiResult<UserDetailsModel>.Error(null, "User is not found due to error or expiration token");
        }

        public async Task<ApiResult<List<UserDetailsModel>>> DeleteRangeUsers(List<Guid> userIds)
        {
            var users = await _unitOfWork.UserRepository.GetAllUsersAsync();
            var existingIds = users.Where(e => userIds.Contains(e.Id)).Select(e => e.Id).ToList();
            var nonExistingIds = userIds.Except(existingIds).ToList();

            if (existingIds.Count > 0)
            {
                await _unitOfWork.UserRepository.SoftRemoveRangeUserAsync(existingIds);
                var result = await _unitOfWork.SaveChangeAsync();
                if (result > 0)
                {
                    return new ApiResult<List<UserDetailsModel>>()
                    {
                        IsSuccess = true,
                        Message = " Added successfully",
                        Data = _mapper.Map<List<UserDetailsModel>>(users.Where(e => existingIds.Contains(e.Id)))
                    };
                }
            }
            else
            {
                if (nonExistingIds.Count > 0)
                {
                    string nonExistingIdsString = string.Join(", ", nonExistingIds);

                    return new ApiResult<List<UserDetailsModel>>()
                    {
                        IsSuccess = false,
                        Message = "There are few ids that is not existed user: " + nonExistingIdsString,
                        Data = _mapper.Map<List<UserDetailsModel>>(users.Where(e => existingIds.Contains(e.Id)))
                    };
                }
            }
            return new ApiResult<List<UserDetailsModel>>()
            {
                IsSuccess = false,
                Message = "failed",
                Data = null
            };
        }

        public async Task<ApiResult<UserDetailsModel>> DeleteUser(Guid id)
        {
            var package = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            if (package != null)
            {
                package = await _unitOfWork.UserRepository.SoftRemoveUserAsync(id);
                //save changes
                var result = await _unitOfWork.SaveChangeAsync();
                if (result > 0)
                {
                    return new ApiResult<UserDetailsModel>()
                    {
                        IsSuccess = true,
                        Message = "Package " + id + " Removed successfully",
                        Data = _mapper.Map<UserDetailsModel>(package)
                    };
                }
            }
            return new ApiResult<UserDetailsModel>()
            {
                IsSuccess = false,
                Message = "There are no existed user id: " + id,
                Data = null
            };
        }

        public async Task<ApiResult<object>> LoginAsync(UserLoginModel User)
        {
            var result = await _unitOfWork.UserRepository.LoginByEmailAndPassword(User);
            if (result == null)
            {
                return ApiResult<object>.Error(User.Email, "This email does not exist, please sign up for an account.");
            }
            return ApiResult<object>.Succeed(result, "Login successfully");
        }

        public async Task<ApiResult<UserDetailsModel>> UserChangePasswordAsync(string email, string token, string newPassword)
        {
            var result = await _unitOfWork.UserRepository.ChangeUserPasswordAsync(email, token, newPassword);
            if (result == null)
            {
                return new ApiResult<UserDetailsModel> { Data = null, IsSuccess = false, Message = "The update process has been cooked, pleas try again" };
            }

            return new ApiResult<UserDetailsModel>
            {
                Data = _mapper.Map<UserDetailsModel>(result),
                IsSuccess = true,
                Message = "Update Sucessfully"
            };
        }

        public async Task<Pagination<UserDetailsModel>> GetUsersByFiltersAsync(PaginationParameter paginationParameter, UserFilterModel userFilterModel)
        {
            var accounts = await _unitOfWork.UserRepository.GetUsersByFiltersAsync(paginationParameter, userFilterModel);
            var mappedResult = new List<UserDetailsModel>();
            //var roleNames = await _unitOfWork.UserRepository.GetAllRoleNamesAsync();
            if (accounts != null)
            {
                foreach (var model in accounts)
                {
                    var mappedModel = _mapper.Map<UserDetailsModel>(model);
                    mappedModel.Gender = model.Gender == true ? "Male" : "Female";
                    //mappedModel.RoleName = (await _unitOfWork.UserRepository.GetRoleName(model)).First();
                    mappedResult.Add(mappedModel);
                }
                return new Pagination<UserDetailsModel>(mappedResult, accounts.TotalCount, accounts.CurrentPage, accounts.PageSize);
            }
            return null;
        }

        public async Task<List<EventOrderReponseDTO>> GetCurrentUserOrders()
        {
            try
            {
                var orders = await _unitOfWork.EventOrderRepository.getCurrentUserOrder();

                return _mapper.Map<List<EventOrderReponseDTO>>(orders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResult<string>> ForgotPassword(string email)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return new ApiResult<string> { Data = null, IsSuccess = false, Message = "User is not existed" };
            }
            else
            {
                return new ApiResult<string>
                {
                    Data = await _unitOfWork.UserRepository.GenerateTokenForResetPassword(user),
                    IsSuccess = true,
                    Message = "Token to your email " + user.Email + " have been sent for reset password"
                };
            }
        }

        public async Task<List<BookedTicketDetailDTO>> GetAllBookedTicketOfCurrentUser()
        {
            var user = await _unitOfWork.UserRepository.GetCurrentUserAsync();
            if (user == null)
            {
                throw new Exception("User is not sign-in");
            }
            return _mapper.Map<List<BookedTicketDetailDTO>>(await _unitOfWork.AttendeeRepository.GetAllBookedTicketsOfUser(user.Id));
        }

        public async Task<bool> ConfirmEmail(string email, string token)
        {
            return await _unitOfWork.UserRepository.ConfirmEmail(email, token);
        }
    }
}