namespace PiggyBank.Models.Data
{
    public class PiggyBankEFUtility
    {
        public static void UpdateModel<T>(T modelToUpdate, T model)
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                if (!prop.IsDefined(typeof(PiggyBankEFIgnore), true))
                {
                    prop.SetValue(modelToUpdate, prop.GetValue(model));
                }
            }
        }
    }
}
