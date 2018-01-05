namespace Zero.Operator.Extensions
{
    public static class OperatorProviderExtensions
    {
        public static bool IsNullOrEmpty(this OperatorModel model)
        {
            if (model == null || model.UserId <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
