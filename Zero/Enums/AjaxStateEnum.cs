namespace Zero.Enums
{
    /// <summary>
    /// 负责标记ajax请求以后的json数据状态
    /// </summary>
    public enum AjaxStateEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 失败
        /// </summary>
        Error = 1,
        /// <summary>
        /// 未登录
        /// </summary>
        NoLogin = 2
    }
}
