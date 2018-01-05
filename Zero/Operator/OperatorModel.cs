using System;

namespace Zero.Operator
{
    [Serializable]
    public partial class OperatorModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string LoginIPAddress { get; set; }
        /// <summary>
        /// 登录IP地址名
        /// </summary>
        public string LoginIPAddressName { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
    }
}
