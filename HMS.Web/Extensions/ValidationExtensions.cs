using System.ComponentModel.DataAnnotations;

namespace HMS.Web.Extensions
{
    public static class ValidationExtensions
    {
        public static bool TryValidate(object value, out List<ValidationResult> validationResults)
        {
            validationResults = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);
            return Validator.TryValidateObject(value, context, validationResults, true);
        }

        public static Dictionary<string, string[]> GetErrorDictionary(this List<ValidationResult> results)
        {
            var dictionary = new Dictionary<string, string[]>();

            foreach (var error in results)
            {
                foreach (var member in error.MemberNames)
                {
                    var key = member.Length == 0 ? "General" : member;
                    if (dictionary.ContainsKey(key))
                    {
                        var list = dictionary[key].ToList();
                        list.Add(error.ErrorMessage);
                        dictionary[key] = list.ToArray();
                    }
                    else
                    {
                        dictionary[key] = new[] { error.ErrorMessage };
                    }
                }
            }

            return dictionary;
        }
    }
}
