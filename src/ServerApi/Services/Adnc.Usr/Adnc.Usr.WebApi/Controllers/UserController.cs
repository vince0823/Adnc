﻿namespace Adnc.Usr.WebApi.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[Route("usr/users")]
[ApiController]
public class UserController : AdncControllerBase
{
    private readonly IUserAppService _userService;
    private readonly UserContext _userContext;

    public UserController(IUserAppService userService, UserContext userContext)
    {
        _userService = userService;
        _userContext = userContext;
    }

    /// <summary>
    /// 新增用户
    /// </summary>
    /// <param name="input">用户信息</param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConsts.User.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> CreateAsync([FromBody] UserCreationDto input)
        => CreatedResult(await _userService.CreateAsync(input));

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="input">用户信息</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Permission(PermissionConsts.User.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateAsync([FromRoute] long id, [FromBody] UserUpdationDto input)
        => Result(await _userService.UpdateAsync(id, input));

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Permission(PermissionConsts.User.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] long id)
        => Result(await _userService.DeleteAsync(id));

    /// <summary>
    /// 设置用户角色
    /// </summary>
    /// <param name="id">用户Id</param>
    /// <param name="roleIds">角色</param>
    /// <returns></returns>
    [HttpPut("{id}/roles")]
    [Permission(PermissionConsts.User.SetRole)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> SetRoleAsync([FromRoute] long id, [FromBody] long[] roleIds)
        => Result(await _userService.SetRoleAsync(id, new UserSetRoleDto { RoleIds = roleIds }));

    /// <summary>
    /// 变更用户状态
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="status">状态</param>
    /// <returns></returns>
    [HttpPut("{id}/status")]
    [Permission(PermissionConsts.User.ChangeStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ChangeStatus([FromRoute] long id, [FromBody] SimpleDto<int> status)
        => Result(await _userService.ChangeStatusAsync(id, status.Value));

    /// <summary>
    /// 批量变更用户状态
    /// </summary>
    /// <param name="input">用户Ids与状态</param>
    /// <returns></returns>
    [HttpPut("batch/status")]
    [Permission(PermissionConsts.User.ChangeStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> ChangeStatus([FromBody] UserChangeStatusDto input)
        => Result(await _userService.ChangeStatusAsync(input.UserIds, input.Status));

    /// <summary>
    /// 获取当前用户是否拥有指定权限
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="permissions"></param>
    /// <param name="validationVersion"></param>
    /// <returns></returns>
    [HttpGet("{id}/permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetCurrenUserPermissions([FromRoute] long id, [FromQuery] IEnumerable<string> permissions, string validationVersion)
    {
        var result = await _userService.GetPermissionsAsync(_userContext.Id, permissions, validationVersion);
        return result ?? new List<string>();
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="search">查询条件</param>
    /// <returns></returns>
    [HttpGet()]
    [Permission(PermissionConsts.User.GetList)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PageModelDto<UserDto>>> GetPagedAsync([FromQuery] UserSearchPagedDto search)
        => await _userService.GetPagedAsync(search);

    /// <summary>
    /// 获取登录用户个人信息
    /// </summary>
    /// <param name="id">id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserInfoDto>> GetCurrentUserInfoAsync([FromRoute] long id)
    {
        if (id != _userContext.Id)
            return NotFound();

        var result = await _userService.GetUserInfoAsync(_userContext.Id);

        if (result != null)
            return result;

        return NotFound();
    }
}