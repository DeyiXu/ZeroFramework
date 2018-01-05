using Zero.Web.Session;

namespace Zero.Operator
{
    public partial class OperatorProvider
    {
        public static OperatorProvider Provider
        {
            get { return new OperatorProvider(); }
        }
        private string LoginUserKey = "zero_loginuserkey_2017";
        /// <summary>
        /// 获取操作员
        /// </summary>
        /// <returns></returns>
        public OperatorModel GetCurrent()
        {
            OperatorModel operatorModel = SessionHelper.GetSession<OperatorModel>(LoginUserKey);

            return operatorModel;
        }
        /// <summary>
        /// 添加操作员
        /// </summary>
        /// <param name="operatorModel"></param>
        public void AddCurrent(OperatorModel operatorModel)
        {
            SessionHelper.WriteSession<OperatorModel>(LoginUserKey, operatorModel);
        }
        /// <summary>
        /// 移除操作员
        /// </summary>
        public void RemoveCurrent()
        {
            SessionHelper.RemoveSession(LoginUserKey);
        }
    }
}
