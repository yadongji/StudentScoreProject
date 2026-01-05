using System;

namespace Models
{
    /// <summary>
    /// 登录请求数据模型
    /// </summary>
    [Serializable]
    public class LoginRequest
    {
        public string phonenumber;
        public string password;
    }

    /// <summary>
    /// 登录响应数据模型
    /// </summary>
    [Serializable]
    public class LoginResponse
    {
        public string token;
        public string message;
        public string user_id;
    }

    /// <summary>
    /// 通用响应数据模型
    /// </summary>
    [Serializable]
    public class BaseResponse
    {
        public int code;
        public string message;
        public string data;
    }

    /// <summary>
    /// 错误响应数据模型
    /// </summary>
    [Serializable]
    public class ErrorResponse
    {
        public string message;
        public string error;
    }

    /// <summary>
    /// 用户信息数据模型
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        public string user_id;
        public string username;
        public string phonenumber;
        public string email;
    }

    /// <summary>
    /// 登录状态枚举
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 未登录
        /// </summary>
        NotLoggedIn,

        /// <summary>
        /// 登录中
        /// </summary>
        LoggingIn,

        /// <summary>
        /// 已登录
        /// </summary>
        LoggedIn,

        /// <summary>
        /// 登录失败
        /// </summary>
        Failed
    }
}
