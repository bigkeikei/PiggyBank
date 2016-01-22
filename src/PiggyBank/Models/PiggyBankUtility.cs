using System;

namespace PiggyBank.Models
{
    public class PiggyBankIgnore : Attribute { }
    public class PiggyBankMandatory : Attribute { }

    public class PiggyBankDataException : Exception
    {
        public PiggyBankDataException(string message) : base(message) { }
    }
    public class PiggyBankNotImplementedException : Exception
    {
        public PiggyBankNotImplementedException(string message) : base(message) { }
    }

    public class PiggyBankUtility
    {
        public static void UpdateModel<T>(T modelToUpdate, T model)
        {
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                if (!prop.IsDefined(typeof(PiggyBankIgnore), true))
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
                    if (prop.GetValue(model) == null) throw new PiggyBankDataException( prop.Name + " is missing");
                }
            }
        }
    }
}
