using System;

namespace PiggyBank.Models
{
    public class PiggyBankIgnoreWhenUpdate : Attribute { }
    public class PiggyBankMandatory : Attribute { }

    public class PiggyBankUtility
    {
        public static void UpdateModel<T>(T modelToUpdate, T model)
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                if (!prop.IsDefined(typeof(PiggyBankIgnoreWhenUpdate), true))
                {
                    prop.SetValue(modelToUpdate, prop.GetValue(model));
                }
            }
        }

        public static void CheckMandatory<T>(T model)
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                if (prop.IsDefined(typeof(PiggyBankMandatory), true))
                {
                    if (prop.GetValue(model) == null) throw new PiggyBankDataException( typeof(T).Name + "." + prop.Name + " is missing");
                }
            }
        }
    }
}
