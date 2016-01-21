using System;

namespace PiggyBank.Models.Data
{
    public class PiggyBankEFIgnore : Attribute { }
    public class PiggyBankEFMandatory : Attribute { }

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

        public static void CheckMandatory<T>(T model)
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                if (prop.IsDefined(typeof(PiggyBankEFMandatory), true))
                {
                    if (prop.GetValue(model) == null) throw new PiggyBankDataException( prop.Name + " is missing");
                }
            }
        }
    }
}
